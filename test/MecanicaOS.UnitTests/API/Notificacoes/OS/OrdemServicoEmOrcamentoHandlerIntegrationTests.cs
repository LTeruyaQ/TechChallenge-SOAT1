using Core.DTOs.Responses.Estoque;
using Core.DTOs.Responses.OrdemServico.InsumoOrdemServico;
using Core.Interfaces.Controllers;
using Core.Interfaces.Servicos;

namespace MecanicaOS.UnitTests.API.Notificacoes.OS
{
    public class OrdemServicoEmOrcamentoHandlerIntegrationTests
    {
        private readonly OrdemServicoEmOrcamentoHandlerFixture _fixture;
        private readonly IOrdemServicoController _ordemServicoController;
        private readonly IServicoEmail _servicoEmail;
        private readonly OrdemServicoEmOrcamentoHandlerMock _handler;

        public OrdemServicoEmOrcamentoHandlerIntegrationTests()
        {
            _fixture = new OrdemServicoEmOrcamentoHandlerFixture();
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

            var valorServico = 120.50M;
            var valorOrcamento = 250.75M;
            var ordemServico = _fixture.CriarOrdemServicoComOrcamento(ordemServicoId, valorServico, valorOrcamento);

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
            // Verificar que o orçamento foi calculado
            await _ordemServicoController.Received(1).CalcularOrcamentoAsync(ordemServicoId);

            // Verificar que os detalhes da OS foram obtidos
            await _ordemServicoController.Received(1).ObterPorId(ordemServicoId);

            // Verificar que o email foi enviado para o cliente correto
            await _servicoEmail.Received(1).EnviarAsync(
                Arg.Is<string[]>(emails => emails.Length == 1 && emails[0] == ordemServico.Cliente.Contato.Email),
                Arg.Is<string>(assunto => assunto == "Orçamento de Serviço"),
                Arg.Any<string>()
            );

            // Verificar conteúdo do email
            emailCapturado.Should().NotBeNull();
            emailCapturado.Should().Contain(ordemServico.Cliente.Nome);
            emailCapturado.Should().Contain(ordemServico.Servico.Nome);
            emailCapturado.Should().Contain(ordemServico.Servico.Valor.ToString("N2"));
            emailCapturado.Should().Contain(ordemServico.Orcamento!.Value.ToString("N2"));

            // Verificar que todos os insumos estão no email
            foreach (var insumo in ordemServico.InsumosOS)
            {
                emailCapturado.Should().Contain(insumo.Estoque.Insumo);
                var precoTotal = insumo.Quantidade * insumo.Estoque.Preco;
                emailCapturado.Should().Contain(precoTotal.ToString("N2"));
            }

            // Verificar logs
            _fixture.LogServicoMock.Received(1).LogInicio("Handle", ordemServicoId);
            _fixture.LogServicoMock.Received(1).LogFim("Handle", Arg.Any<object>());
        }

        [Fact]
        public async Task Handle_ComOrdemServicoSemInsumos_DeveEnviarEmailSomenteComServico()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var evento = _fixture.CriarEvento(ordemServicoId);

            var ordemServico = _fixture.CriarOrdemServicoSemInsumos(ordemServicoId);
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
            emailCapturado.Should().Contain(ordemServico.Servico.Valor.ToString("N2"));
            emailCapturado.Should().Contain(ordemServico.Orcamento!.Value.ToString("N2"));

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

            // Act & Assert
            var act = async () => await _handler.Handle(evento, CancellationToken.None);

            // Deve lançar uma exceção porque não há email para enviar
            await act.Should().ThrowAsync<Exception>();

            // Verificar que o orçamento foi calculado
            await _ordemServicoController.Received(1).CalcularOrcamentoAsync(ordemServicoId);

            // Verificar que os detalhes da OS foram obtidos
            await _ordemServicoController.Received(1).ObterPorId(ordemServicoId);

            // Verificar que o email não foi enviado
            await _servicoEmail.DidNotReceive().EnviarAsync(
                Arg.Any<string[]>(),
                Arg.Any<string>(),
                Arg.Any<string>()
            );

            // Verificar logs
            _fixture.LogServicoMock.Received(1).LogInicio("Handle", ordemServicoId);
            _fixture.LogServicoMock.Received(1).LogErro("Handle", Arg.Any<Exception>());
        }

        [Fact]
        public async Task Handle_DevePassarDadosCorretamenteEntreHandlerERepositorio()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var evento = _fixture.CriarEvento(ordemServicoId);

            // Criar ordem de serviço com valores específicos para identificar no teste
            var valorServico = 175.50M;
            var valorOrcamento = 350.25M;

            var ordemServico = _fixture.CriarOrdemServicoComOrcamento(ordemServicoId, valorServico, valorOrcamento);
            ordemServico.Cliente.Nome = "Cliente Teste Especial";
            ordemServico.Servico.Nome = "Serviço Especial de Teste";

            // Configurar insumos com valores específicos
            var insumos = new List<InsumoOSResponse>
            {
                new InsumoOSResponse
                {
                    OrdemServicoId = ordemServicoId,
                    EstoqueId = Guid.NewGuid(),
                    Quantidade = 3,
                    Estoque = new EstoqueResponse
                    {
                        Id = Guid.NewGuid(),
                        Insumo = "Insumo Especial 1",
                        Preco = 45.25
                    }
                },
                new InsumoOSResponse
                {
                    OrdemServicoId = ordemServicoId,
                    EstoqueId = Guid.NewGuid(),
                    Quantidade = 2,
                    Estoque = new EstoqueResponse
                    {
                        Id = Guid.NewGuid(),
                        Insumo = "Insumo Especial 2",
                        Preco = 65.75
                    }
                }
            };

            ordemServico.InsumosOS = insumos;

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
            // Verificar que o orçamento foi calculado com o ID correto
            await _ordemServicoController.Received(1).CalcularOrcamentoAsync(ordemServicoId);

            // Verificar que os detalhes da OS foram obtidos com o ID correto
            await _ordemServicoController.Received(1).ObterPorId(ordemServicoId);

            // Verificar que o email foi enviado para o cliente correto
            await _servicoEmail.Received(1).EnviarAsync(
                Arg.Is<string[]>(emails => emails.Length == 1 && emails[0] == ordemServico.Cliente.Contato.Email),
                Arg.Is<string>(assunto => assunto == "Orçamento de Serviço"),
                Arg.Any<string>()
            );

            // Verificar que o conteúdo do email contém os dados corretos da ordem de serviço
            emailCapturado.Should().NotBeNull();
            emailCapturado.Should().Contain("Cliente Teste Especial");
            emailCapturado.Should().Contain("Serviço Especial de Teste");
            emailCapturado.Should().Contain(valorServico.ToString("N2"));
            emailCapturado.Should().Contain(valorOrcamento.ToString("N2"));

            // Verificar que os insumos estão presentes no email
            emailCapturado.Should().Contain("Insumo Especial 1");
            emailCapturado.Should().Contain("Insumo Especial 2");
            emailCapturado.Should().Contain((3 * 45.25).ToString("N2"));
            emailCapturado.Should().Contain((2 * 65.75).ToString("N2"));

            // Verificar logs
            _fixture.LogServicoMock.Received(1).LogInicio("Handle", ordemServicoId);
            _fixture.LogServicoMock.Received(1).LogFim("Handle", Arg.Any<object>());
        }
    }
}
