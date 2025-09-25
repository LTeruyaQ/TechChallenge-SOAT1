using Core.DTOs.Entidades.OrdemServicos;
using Core.Entidades;
using Core.Enumeradores;
using Core.Especificacoes.Base.Interfaces;
using Core.UseCases.OrdensServico.ObterOrdemServicoPorStatus;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.Handlers;
using NSubstitute;
using System;
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

            _fixture.ConfigurarMockRepositorioOrdemServicoParaObterPorStatus(status, ordensServico);

            var handler = _fixture.CriarObterOrdemServicoPorStatusHandler();

            // Act
            var resultado = await handler.Handle(status);

            // Assert
            resultado.Should().NotBeNull();
            resultado.OrdensServico.Should().NotBeNull();
            resultado.OrdensServico.Should().HaveCount(2);
            resultado.OrdensServico.Should().AllSatisfy(os => os.Status.Should().Be(status));
            
            // Verificar que o repositório foi chamado
            await _fixture.RepositorioOrdemServico.Received(1).ListarProjetadoAsync<OrdemServico>(Arg.Any<IEspecificacao<OrdemServicoEntityDto>>());

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

            _fixture.ConfigurarMockRepositorioOrdemServicoParaObterPorStatus(status, ordensServico);

            var handler = _fixture.CriarObterOrdemServicoPorStatusHandler();

            // Act
            var resultado = await handler.Handle(status);

            // Assert
            resultado.Should().NotBeNull();
            resultado.OrdensServico.Should().NotBeNull();
            resultado.OrdensServico.Should().BeEmpty();
            
            // Verificar que o repositório foi chamado
            await _fixture.RepositorioOrdemServico.Received(1).ListarProjetadoAsync<OrdemServico>(Arg.Any<IEspecificacao<OrdemServicoEntityDto>>());

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterPorStatus.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<StatusOrdemServico>());
            _fixture.LogServicoObterPorStatus.Received(1).LogFim(Arg.Any<string>(), Arg.Any<ObterOrdemServicoPorStatusResponse>());
        }

        [Fact]
        public async Task Handle_QuandoRepositorioLancaExcecao_DeveRegistrarLogEPropagar()
        {
            // Arrange
            var status = StatusOrdemServico.Recebida;
            
            // Configurar o repositório para lançar uma exceção
            _fixture.RepositorioOrdemServico.ListarProjetadoAsync<OrdemServico>(Arg.Any<IEspecificacao<OrdemServicoEntityDto>>())
                .Returns(Task.FromException<IEnumerable<OrdemServico>>(new InvalidOperationException("Erro simulado")));

            var handler = _fixture.CriarObterOrdemServicoPorStatusHandler();

            // Act & Assert
            var act = async () => await handler.Handle(status);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Erro simulado");

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterPorStatus.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<StatusOrdemServico>());
            _fixture.LogServicoObterPorStatus.Received(1).LogErro(Arg.Any<string>(), Arg.Any<InvalidOperationException>());
        }
    }
}
