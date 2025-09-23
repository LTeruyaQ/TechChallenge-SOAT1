using Core.Entidades;
using Core.Enumeradores;
using Core.UseCases.OrdensServico.ObterOrdemServicoPorStatus;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.Handlers;
using NSubstitute;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.OrdensServico
{
    public class ObterOrdemServicoPorStatusHandlerTests
    {
        private readonly OrdemServicoHandlerFixture _fixture;

        public ObterOrdemServicoPorStatusHandlerTests()
        {
            _fixture = new OrdemServicoHandlerFixture();
        }

        [Theory]
        [InlineData(StatusOrdemServico.Recebida)]
        [InlineData(StatusOrdemServico.EmDiagnostico)]
        [InlineData(StatusOrdemServico.EmExecucao)]
        [InlineData(StatusOrdemServico.Finalizada)]
        [InlineData(StatusOrdemServico.AguardandoAprovação)]
        [InlineData(StatusOrdemServico.Cancelada)]
        public async Task Handle_ComStatusValido_DeveRetornarOrdensComStatus(StatusOrdemServico status)
        {
            // Arrange
            var ordensServico = new List<OrdemServico>
            {
                OrdemServicoHandlerFixture.CriarOrdemServicoValida(status),
                OrdemServicoHandlerFixture.CriarOrdemServicoValida(status)
            };

            _fixture.ConfigurarMockOrdemServicoGatewayParaObterPorStatus(status, ordensServico);

            var handler = _fixture.CriarObterOrdemServicoPorStatusHandler();

            // Act
            var resultado = await handler.Handle(status);

            // Assert
            resultado.Should().NotBeNull();
            resultado.OrdensServico.Should().NotBeNull();
            resultado.OrdensServico.Should().HaveCount(2);
            resultado.OrdensServico.Should().AllSatisfy(os => os.Status.Should().Be(status));
            
            // Verificar que o gateway foi chamado
            await _fixture.OrdemServicoGateway.Received(1).ObterOrdemServicoPorStatusAsync(status);

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterPorStatus.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<StatusOrdemServico>());
            _fixture.LogServicoObterPorStatus.Received(1).LogFim(Arg.Any<string>(), Arg.Any<ObterOrdemServicoPorStatusResponse>());
        }

        [Fact]
        public async Task Handle_ComStatusSemOrdens_DeveRetornarListaVazia()
        {
            // Arrange
            var status = StatusOrdemServico.Cancelada;
            var ordensServico = new List<OrdemServico>();

            _fixture.ConfigurarMockOrdemServicoGatewayParaObterPorStatus(status, ordensServico);

            var handler = _fixture.CriarObterOrdemServicoPorStatusHandler();

            // Act
            var resultado = await handler.Handle(status);

            // Assert
            resultado.Should().NotBeNull();
            resultado.OrdensServico.Should().NotBeNull();
            resultado.OrdensServico.Should().BeEmpty();
            
            // Verificar que o gateway foi chamado
            await _fixture.OrdemServicoGateway.Received(1).ObterOrdemServicoPorStatusAsync(status);

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterPorStatus.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<StatusOrdemServico>());
            _fixture.LogServicoObterPorStatus.Received(1).LogFim(Arg.Any<string>(), Arg.Any<ObterOrdemServicoPorStatusResponse>());
        }

        [Fact]
        public async Task Handle_ComExcecaoInesperada_DevePropagaExcecao()
        {
            // Arrange
            var status = StatusOrdemServico.Recebida;
            
            // Configurar o gateway para lançar uma exceção
            _fixture.OrdemServicoGateway.ObterOrdemServicoPorStatusAsync(status)
                .Returns(Task.FromException<IEnumerable<OrdemServico>>(new System.InvalidOperationException("Erro simulado")));

            var handler = _fixture.CriarObterOrdemServicoPorStatusHandler();

            // Act & Assert
            var act = async () => await handler.Handle(status);

            await act.Should().ThrowAsync<System.InvalidOperationException>()
                .WithMessage("Erro simulado");

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterPorStatus.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<StatusOrdemServico>());
            _fixture.LogServicoObterPorStatus.Received(1).LogErro(Arg.Any<string>(), Arg.Any<System.InvalidOperationException>());
        }
    }
}
