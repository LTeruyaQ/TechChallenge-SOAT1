using Core.DTOs.Responses.Cliente;
using Core.DTOs.Responses.OrdemServico;
using Core.DTOs.Responses.Servico;
using Core.DTOs.Responses.Veiculo;
using Core.Interfaces.Controllers;
using Core.Interfaces.Servicos;

namespace MecanicaOS.UnitTests.API.Notificacoes.OS
{
    public class OrdemServicoFinalizadaHandlerIntegrationTests
    {
        private readonly OrdemServicoFinalizadaHandlerFixture _fixture;
        private readonly IOrdemServicoController _ordemServicoController;
        private readonly IServicoEmail _servicoEmail;
        private readonly OrdemServicoFinalizadaHandlerMock _handler;

        public OrdemServicoFinalizadaHandlerIntegrationTests()
        {
            _fixture = new OrdemServicoFinalizadaHandlerFixture();
            _ordemServicoController = _fixture.OrdemServicoController;
            _servicoEmail = _fixture.ServicoEmail;
            _handler = _fixture.Handler;
        }

        [Fact]
        public async Task Handle_DeveProcessarFluxoCompleto_EndToEnd()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var evento = _fixture.CriarEvento(ordemServicoId);

            var ordemServico = _fixture.CriarOrdemServicoFinalizada(ordemServicoId);
            _ordemServicoController.ObterPorId(ordemServicoId).Returns(ordemServico);

            string emailCapturado = null;

            _servicoEmail.EnviarAsync(
                Arg.Any<string[]>(),
                Arg.Any<string>(),
                Arg.Do<string>(conteudo => emailCapturado = conteudo)
            ).Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(evento, CancellationToken.None);

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

            // Verificar logs
            _fixture.LogServicoMock.Received(1).LogInicio("Handle", ordemServicoId);
            _fixture.LogServicoMock.Received(1).LogFim("Handle", Arg.Any<object>());
        }

        [Fact]
        public async Task Handle_ComClienteSemEmail_DeveTratarCorretamente()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var evento = _fixture.CriarEvento(ordemServicoId);

            var ordemServico = _fixture.CriarOrdemServicoSemEmail(ordemServicoId);
            _ordemServicoController.ObterPorId(ordemServicoId).Returns(ordemServico);

            // Configurar o serviço de email para lançar uma exceção
            _servicoEmail.EnviarAsync(
                Arg.Any<string[]>(),
                Arg.Any<string>(),
                Arg.Any<string>()
            ).Returns(x => { throw new Exception("Erro ao enviar email"); });

            // Act & Assert
            var act = async () => await _handler.Handle(evento, CancellationToken.None);

            // Deve lançar uma exceção porque não há email para enviar
            await act.Should().ThrowAsync<Exception>();

            // Verificar que os detalhes da OS foram obtidos
            await _ordemServicoController.Received(1).ObterPorId(ordemServicoId);

            // Verificar logs
            _fixture.LogServicoMock.Received(1).LogInicio(Arg.Any<string>(), ordemServicoId);
            _fixture.LogServicoMock.Received(1).LogErro(Arg.Any<string>(), Arg.Any<Exception>());
        }

        [Fact]
        public async Task Handle_DevePassarDadosCorretamenteEntreHandlerERepositorio()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var evento = _fixture.CriarEvento(ordemServicoId);

            // Criar ordem de serviço com valores específicos para identificar no teste
            var ordemServico = new OrdemServicoResponse
            {
                Id = ordemServicoId,
                Cliente = new ClienteResponse
                {
                    Id = Guid.NewGuid(),
                    Nome = "Cliente Teste Especial",
                    Contato = new ContatoResponse
                    {
                        Email = "cliente.especial@teste.com",
                        Telefone = "(11) 88888-8888"
                    }
                },
                Servico = new ServicoResponse
                {
                    Id = Guid.NewGuid(),
                    Nome = "Serviço Especial de Teste",
                    Valor = 200
                },
                Veiculo = new VeiculoResponse
                {
                    Id = Guid.NewGuid(),
                    Marca = "Honda",
                    Modelo = "Civic Especial",
                    Placa = "XYZ-9876",
                    Ano = "2023"
                }
            };

            // Configurar o controller para retornar a ordem de serviço com valores específicos
            _ordemServicoController.ObterPorId(ordemServicoId).Returns(ordemServico);

            // Capturar o conteúdo do email para verificar se os dados foram passados corretamente
            string emailCapturado = null;

            _servicoEmail.EnviarAsync(
                Arg.Any<string[]>(),
                Arg.Any<string>(),
                Arg.Do<string>(conteudo => emailCapturado = conteudo)
            ).Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(evento, CancellationToken.None);

            // Assert
            // Verificar que os detalhes da OS foram obtidos com o ID correto
            await _ordemServicoController.Received(1).ObterPorId(ordemServicoId);

            // Verificar que o email foi enviado
            await _servicoEmail.Received(1).EnviarAsync(
                Arg.Any<string[]>(),
                Arg.Any<string>(),
                Arg.Any<string>()
            );

            // Verificar que o conteúdo do email contém os dados corretos da ordem de serviço
            emailCapturado.Should().NotBeNull();
            emailCapturado.Should().Contain("Cliente Teste Especial");
            emailCapturado.Should().Contain("Serviço Especial de Teste");
            emailCapturado.Should().Contain("Civic Especial");
            emailCapturado.Should().Contain("XYZ-9876");

            // Verificar logs
            _fixture.LogServicoMock.Received(1).LogInicio("Handle", ordemServicoId);
            _fixture.LogServicoMock.Received(1).LogFim("Handle", Arg.Any<object>());
        }
    }
}
