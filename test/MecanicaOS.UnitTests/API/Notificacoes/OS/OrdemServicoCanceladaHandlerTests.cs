using Core.DTOs.Requests.OrdemServico.InsumoOS;
using Core.DTOs.Responses.OrdemServico;
using Core.DTOs.Responses.OrdemServico.InsumoOrdemServico;
using NSubstitute.ExceptionExtensions;

namespace MecanicaOS.UnitTests.API.Notificacoes.OS
{
    public class OrdemServicoCanceladaHandlerTests
    {
        private readonly OrdemServicoCanceladaHandlerFixture _fixture;

        public OrdemServicoCanceladaHandlerTests()
        {
            _fixture = new OrdemServicoCanceladaHandlerFixture();
        }

        [Fact]
        public async Task Handle_ComDiferentesTiposDeOrdemServicoSemInsumos_NaoDeveDevolverInsumos()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var evento = _fixture.CriarEvento(ordemServicoId);

            // Configurar ordem de serviço sem insumos (3 cenários possíveis)
            var ordemServicoVazia = _fixture.CriarOrdemServicoSemInsumos(ordemServicoId);
            var ordemServicoComInsumosVazios = new OrdemServicoResponse
            {
                Id = ordemServicoId,
                Insumos = new List<InsumoOSResponse>()
            };
            var ordemServicoComInsumosNulos = new OrdemServicoResponse
            {
                Id = ordemServicoId,
                Insumos = null
            };

            // Testar os três cenários
            foreach (var ordemServico in new[] { ordemServicoVazia, ordemServicoComInsumosVazios, ordemServicoComInsumosNulos })
            {
                // Configurar o mock
                _fixture.OrdemServicoController.ObterPorId(ordemServicoId).Returns(ordemServico);

                // Act
                await _fixture.Handler.Handle(evento, CancellationToken.None);

                // Assert
                await _fixture.OrdemServicoController.Received(1).ObterPorId(ordemServicoId);
                _fixture.LogServico.Received(1).LogInicio(Arg.Any<string>(), ordemServicoId);
                _fixture.LogServico.Received(1).LogFim(Arg.Any<string>(), Arg.Any<object>());

                // Verificar que o método de devolução de insumos não foi chamado
                await _fixture.InsumoOSController.DidNotReceive().DevolverInsumosAoEstoque(Arg.Any<IEnumerable<DevolverInsumoOSRequest>>());
                
                // Limpar contadores de chamadas para o próximo cenário
                _fixture.OrdemServicoController.ClearReceivedCalls();
                _fixture.LogServico.ClearReceivedCalls();
                _fixture.InsumoOSController.ClearReceivedCalls();
            }
        }

        [Fact]
        public async Task Handle_QuandoOrdemServicoControllerLancaExcecao_DeveLogarErroEPropagar()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var evento = _fixture.CriarEvento(ordemServicoId);
            var exception = new Exception("Erro ao obter ordem de serviço");

            _fixture.OrdemServicoController.ObterPorId(ordemServicoId).Throws(exception);

            // Act & Assert
            var act = async () => await _fixture.Handler.Handle(evento, CancellationToken.None);

            await act.Should().ThrowAsync<Exception>().WithMessage("Erro ao obter ordem de serviço");

            _fixture.LogServico.Received(1).LogInicio(Arg.Any<string>(), ordemServicoId);
            _fixture.LogServico.Received(1).LogErro(Arg.Any<string>(), exception);
            _fixture.LogServico.DidNotReceive().LogFim(Arg.Any<string>());
        }

        [Fact]
        public async Task Handle_QuandoInsumoOSControllerLancaExcecao_DeveLogarErroEPropagar()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var evento = _fixture.CriarEvento(ordemServicoId);
            var exception = new Exception("Erro ao devolver insumos");

            // Configurar ordem de serviço com insumos
            var ordemServico = _fixture.CriarOrdemServicoComInsumos(ordemServicoId, 1);

            _fixture.OrdemServicoController.ObterPorId(ordemServicoId).Returns(ordemServico);
            _fixture.InsumoOSController.DevolverInsumosAoEstoque(Arg.Any<IEnumerable<DevolverInsumoOSRequest>>()).Throws(exception);

            // Act & Assert
            var act = async () => await _fixture.Handler.Handle(evento, CancellationToken.None);

            await act.Should().ThrowAsync<Exception>().WithMessage("Erro ao devolver insumos");

            _fixture.LogServico.Received(1).LogInicio(Arg.Any<string>(), ordemServicoId);
            _fixture.LogServico.Received(1).LogErro(Arg.Any<string>(), exception);
            _fixture.LogServico.DidNotReceive().LogFim(Arg.Any<string>());
        }
    }
}
