using API.Notificacoes.OS;
using Core.DTOs.Responses.OrdemServico;
using Core.Interfaces.Controllers;
using Core.Interfaces.Servicos;
using FluentAssertions;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOS.UnitTests.API.Notificacoes.OS
{
    public class OrdemServicoEmOrcamentoHandlerTests
    {
        private readonly OrdemServicoEmOrcamentoHandlerFixture _fixture;

        public OrdemServicoEmOrcamentoHandlerTests()
        {
            _fixture = new OrdemServicoEmOrcamentoHandlerFixture();
        }

        [Fact]
        public async Task Handle_DeveCalcularOrcamentoEEnviarEmail()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var evento = _fixture.CriarEvento(ordemServicoId);
            
            var ordemServico = _fixture.CriarOrdemServicoComOrcamento(ordemServicoId);
            _fixture.OrdemServicoController.ObterPorId(ordemServicoId).Returns(ordemServico);

            // Act
            await _fixture.Handler.Handle(evento, CancellationToken.None);

            // Assert
            // Verificar que o orçamento foi calculado
            await _fixture.OrdemServicoController.Received(1).CalcularOrcamentoAsync(ordemServicoId);
            
            // Verificar que os detalhes da OS foram obtidos
            await _fixture.OrdemServicoController.Received(1).ObterPorId(ordemServicoId);
            
            // Verificar que o email foi enviado
            await _fixture.ServicoEmail.Received(1).EnviarAsync(
                Arg.Is<string[]>(emails => emails.Length == 1 && emails[0] == ordemServico.Cliente.Contato.Email),
                Arg.Is<string>(assunto => assunto == "Orçamento de Serviço"),
                Arg.Any<string>()
            );
            
            // Verificar logs
            _fixture.LogServicoMock.Received(1).LogInicio(Arg.Any<string>(), ordemServicoId);
            _fixture.LogServicoMock.Received(1).LogFim(Arg.Any<string>(), Arg.Any<object>());
        }

        [Fact]
        public async Task Handle_QuandoCalcularOrcamentoLancaExcecao_DeveLogarErroEPropagar()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var evento = _fixture.CriarEvento(ordemServicoId);
            
            var exception = new Exception("Erro ao calcular orçamento");
            _fixture.OrdemServicoController.CalcularOrcamentoAsync(ordemServicoId).Returns(Task.FromException<bool>(exception));

            // Act & Assert
            var act = async () => await _fixture.Handler.Handle(evento, CancellationToken.None);
            
            await act.Should().ThrowAsync<Exception>().WithMessage("Erro ao calcular orçamento");
            
            // Verificar logs
            _fixture.LogServicoMock.Received(1).LogInicio(Arg.Any<string>(), ordemServicoId);
            _fixture.LogServicoMock.Received(1).LogErro(Arg.Any<string>(), exception);
            _fixture.LogServicoMock.DidNotReceive().LogFim(Arg.Any<string>(), Arg.Any<object>());
            
            // Verificar que o email não foi enviado
            await _fixture.ServicoEmail.DidNotReceive().EnviarAsync(
                Arg.Any<string[]>(),
                Arg.Any<string>(),
                Arg.Any<string>()
            );
        }

        [Fact]
        public async Task Handle_QuandoObterOrdemServicoLancaExcecao_DeveLogarErroEPropagar()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var evento = _fixture.CriarEvento(ordemServicoId);
            
            var exception = new Exception("Erro ao obter ordem de serviço");
            _fixture.OrdemServicoController.ObterPorId(ordemServicoId).Returns(Task.FromException<OrdemServicoResponse>(exception));

            // Act & Assert
            var act = async () => await _fixture.Handler.Handle(evento, CancellationToken.None);
            
            await act.Should().ThrowAsync<Exception>().WithMessage("Erro ao obter ordem de serviço");
            
            // Verificar logs
            _fixture.LogServicoMock.Received(1).LogInicio(Arg.Any<string>(), ordemServicoId);
            _fixture.LogServicoMock.Received(1).LogErro(Arg.Any<string>(), exception);
            _fixture.LogServicoMock.DidNotReceive().LogFim(Arg.Any<string>(), Arg.Any<object>());
            
            // Verificar que o email não foi enviado
            await _fixture.ServicoEmail.DidNotReceive().EnviarAsync(
                Arg.Any<string[]>(),
                Arg.Any<string>(),
                Arg.Any<string>()
            );
        }

        [Fact]
        public async Task Handle_QuandoEnviarEmailLancaExcecao_DeveLogarErroEPropagar()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var evento = _fixture.CriarEvento(ordemServicoId);
            
            var ordemServico = _fixture.CriarOrdemServicoComOrcamento(ordemServicoId);
            _fixture.OrdemServicoController.ObterPorId(ordemServicoId).Returns(ordemServico);
            
            var exception = new Exception("Erro ao enviar email");
            _fixture.ServicoEmail.EnviarAsync(
                Arg.Any<string[]>(),
                Arg.Any<string>(),
                Arg.Any<string>()
            ).Returns(Task.FromException(exception));

            // Act & Assert
            var act = async () => await _fixture.Handler.Handle(evento, CancellationToken.None);
            
            await act.Should().ThrowAsync<Exception>().WithMessage("Erro ao enviar email");
            
            // Verificar logs
            _fixture.LogServicoMock.Received(1).LogInicio(Arg.Any<string>(), ordemServicoId);
            _fixture.LogServicoMock.Received(1).LogErro(Arg.Any<string>(), exception);
            _fixture.LogServicoMock.DidNotReceive().LogFim(Arg.Any<string>(), Arg.Any<object>());
        }
    }
}
