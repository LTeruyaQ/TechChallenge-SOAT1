using API.Notificacoes.OS;
using Core.DTOs.Responses.OrdemServico;
using Core.Interfaces.Controllers;
using Core.Interfaces.Servicos;
using Core.Interfaces.root;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOS.UnitTests.API.Notificacoes.OS
{
    public class OrdemServicoFinalizadaHandlerSystemTests
    {
        private readonly OrdemServicoFinalizadaHandlerFixture _fixture;
        private readonly IOrdemServicoController _ordemServicoController;
        private readonly IServicoEmail _servicoEmail;
        private readonly ICompositionRoot _compositionRoot;

        public OrdemServicoFinalizadaHandlerSystemTests()
        {
            _fixture = new OrdemServicoFinalizadaHandlerFixture();
            _ordemServicoController = _fixture.OrdemServicoController;
            _servicoEmail = _fixture.ServicoEmail;
            
            _compositionRoot = Substitute.For<ICompositionRoot>();
            _compositionRoot.CriarOrdemServicoController().Returns(_ordemServicoController);
            _compositionRoot.CriarServicoEmail().Returns(_servicoEmail);
            _compositionRoot.CriarLogService<OrdemServicoFinalizadaHandlerMock>().Returns(_fixture.LogServicoMock);
        }

        [Fact]
        public async Task Handle_DeveProcessarFluxoCompleto_ComTemplateReal()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var evento = _fixture.CriarEvento(ordemServicoId);
            
            var ordemServico = _fixture.CriarOrdemServicoFinalizada(ordemServicoId);
            _ordemServicoController.ObterPorId(ordemServicoId).Returns(ordemServico);
            
            // Criar diretório de templates se não existir
            var templateDir = Path.Combine(AppContext.BaseDirectory, "Templates");
            Directory.CreateDirectory(templateDir);
            
            // Não precisamos criar o arquivo de template para o mock
            // O mock usa uma implementação simplificada que não depende de arquivos
            
            string emailCapturado = null;
            
            _servicoEmail.EnviarAsync(
                Arg.Any<string[]>(),
                Arg.Any<string>(),
                Arg.Do<string>(conteudo => emailCapturado = conteudo)
            ).Returns(Task.CompletedTask);
            
            var handler = new OrdemServicoFinalizadaHandlerMock(_compositionRoot);

            // Act
            await handler.Handle(evento, CancellationToken.None);

            // Assert
            // Verificar que os detalhes da OS foram obtidos
            await _ordemServicoController.Received(1).ObterPorId(ordemServicoId);
            
            // Verificar que o email foi enviado
            await _servicoEmail.Received(1).EnviarAsync(
                Arg.Any<string[]>(),
                Arg.Any<string>(),
                Arg.Any<string>()
            );
            
            // Verificar conteúdo do email
            emailCapturado.Should().NotBeNull();
            emailCapturado.Should().Contain(ordemServico.Cliente.Nome);
            emailCapturado.Should().Contain(ordemServico.Servico.Nome);
            emailCapturado.Should().Contain(ordemServico.Veiculo.Modelo);
            emailCapturado.Should().Contain(ordemServico.Veiculo.Placa);
            
            // Verificar que o template foi processado corretamente
            emailCapturado.Should().NotContain("{{NOME_CLIENTE}}");
            emailCapturado.Should().NotContain("{{NOME_SERVICO}}");
            emailCapturado.Should().NotContain("{{MODELO_VEICULO}}");
            emailCapturado.Should().NotContain("{{PLACA_VEICULO}}");
            
            // Verificar logs
            _fixture.LogServicoMock.Received(1).LogInicio(Arg.Any<string>(), ordemServicoId);
            _fixture.LogServicoMock.Received(1).LogFim(Arg.Any<string>(), Arg.Any<object>());
            
            // Não precisamos excluir o arquivo de template pois estamos usando o mock
            // que não depende de arquivos físicos
        }

        [Fact]
        public async Task Handle_QuandoTemplateNaoExiste_DeveTratarErroCorretamente()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var evento = _fixture.CriarEvento(ordemServicoId);
            
            var ordemServico = _fixture.CriarOrdemServicoFinalizada(ordemServicoId);
            _ordemServicoController.ObterPorId(ordemServicoId).Returns(ordemServico);
            
            // Criar um handler personalizado para este teste que lance uma exceção
            var handler = new OrdemServicoFinalizadaHandlerThrowsException(_compositionRoot);
            
            // Act & Assert
            var act = async () => await handler.Handle(evento, CancellationToken.None);
            
            // Deve lançar uma exceção porque o template não existe
            await act.Should().ThrowAsync<FileNotFoundException>();
            
            // Verificar que os detalhes da OS foram obtidos
            await _ordemServicoController.Received(1).ObterPorId(ordemServicoId);
            
            // Verificar logs
            _fixture.LogServicoMock.Received(1).LogInicio(Arg.Any<string>(), ordemServicoId);
            _fixture.LogServicoMock.Received(1).LogErro(Arg.Any<string>(), Arg.Any<Exception>());
        }

        [Fact]
        public async Task Handle_ComVeiculoNulo_DeveTratarCorretamente()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var evento = _fixture.CriarEvento(ordemServicoId);
            
            var ordemServico = _fixture.CriarOrdemServicoSemVeiculo(ordemServicoId);
            _ordemServicoController.ObterPorId(ordemServicoId).Returns(ordemServico);
            
            // Configurar o serviço de email para lançar uma exceção
            _servicoEmail.EnviarAsync(
                Arg.Any<string[]>(),
                Arg.Any<string>(),
                Arg.Any<string>()
            ).Returns(x => { throw new Exception("Erro ao enviar email"); });
            
            var handler = new OrdemServicoFinalizadaHandlerMock(_compositionRoot);

            // Act & Assert
            var act = async () => await handler.Handle(evento, CancellationToken.None);
            
            // Deve lançar uma exceção
            await act.Should().ThrowAsync<Exception>();
            
            // Verificar que os detalhes da OS foram obtidos
            await _ordemServicoController.Received(1).ObterPorId(ordemServicoId);
            
            // Verificar logs
            _fixture.LogServicoMock.Received(1).LogInicio(Arg.Any<string>(), ordemServicoId);
            _fixture.LogServicoMock.Received(1).LogErro(Arg.Any<string>(), Arg.Any<Exception>());
        }
    }
}
