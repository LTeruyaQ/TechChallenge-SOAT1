using Core.DTOs.Entidades.OrdemServicos;
using Core.Entidades;
using MecanicaOS.UnitTests.Fixtures.Handlers;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.OrdensServico
{
    public class ObterTodosOrdensServicoHandlerTests
    {
        private readonly OrdemServicoHandlerFixture _fixture;

        public ObterTodosOrdensServicoHandlerTests()
        {
            _fixture = new OrdemServicoHandlerFixture();
        }

        [Fact]
        public async Task Handle_DeveRetornarTodasOrdensServico()
        {
            // Arrange
            var ordensServico = new List<OrdemServico>
            {
                OrdemServicoHandlerFixture.CriarOrdemServicoValida(),
                OrdemServicoHandlerFixture.CriarOrdemServicoValida(),
                OrdemServicoHandlerFixture.CriarOrdemServicoValida()
            };

            _fixture.ConfigurarMockRepositorioOrdemServicoParaObterTodos(ordensServico);

            var handler = _fixture.CriarObterTodosOrdensServicoHandler();

            // Act
            var resultado = await handler.Handle();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(3);

            // Verificar que o repositório foi chamado
            await _fixture.RepositorioOrdemServico.Received(1).ObterTodosAsync();

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterTodos.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoObterTodos.Received(1).LogFim(Arg.Any<string>(), Arg.Any<IEnumerable<OrdemServico>>());
        }

        [Fact]
        public async Task Handle_ComListaVazia_DeveRetornarListaVazia()
        {
            // Arrange
            var ordensServico = new List<OrdemServico>();

            _fixture.ConfigurarMockRepositorioOrdemServicoParaObterTodos(ordensServico);

            var handler = _fixture.CriarObterTodosOrdensServicoHandler();

            // Act
            var resultado = await handler.Handle();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();

            // Verificar que o repositório foi chamado
            await _fixture.RepositorioOrdemServico.Received(1).ObterTodosAsync();

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterTodos.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoObterTodos.Received(1).LogFim(Arg.Any<string>(), Arg.Any<IEnumerable<OrdemServico>>());
        }

        [Fact]
        public async Task Handle_QuandoRepositorioLancaExcecao_DeveRegistrarLogEPropagar()
        {
            // Arrange
            // Configurar o repositório para lançar uma exceção
            _fixture.RepositorioOrdemServico.ObterTodosAsync()
                .Returns(Task.FromException<IEnumerable<OrdemServicoEntityDto>>(new InvalidOperationException("Erro simulado")));

            var handler = _fixture.CriarObterTodosOrdensServicoHandler();

            // Act & Assert
            var act = async () => await handler.Handle();

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Erro simulado");

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterTodos.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoObterTodos.Received(1).LogErro(Arg.Any<string>(), Arg.Any<InvalidOperationException>());
        }
    }
}
