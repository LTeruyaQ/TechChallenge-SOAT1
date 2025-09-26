using Core.DTOs.Entidades.Servico;
using Core.Entidades;
using MecanicaOS.UnitTests.Fixtures.Handlers;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.Servicos
{
    public class ObterTodosServicosHandlerTests
    {
        private readonly ServicoHandlerFixture _fixture;

        public ObterTodosServicosHandlerTests()
        {
            _fixture = new ServicoHandlerFixture();
        }

        [Fact]
        public async Task Handle_DeveRetornarListaDeServicos()
        {
            // Arrange
            var servicos = new List<Servico>
            {
                ServicoHandlerFixture.CriarServicoValido(nome: "Serviço 1"),
                ServicoHandlerFixture.CriarServicoValido(nome: "Serviço 2"),
                ServicoHandlerFixture.CriarServicoValido(nome: "Serviço 3")
            };

            _fixture.ConfigurarMockServicoGatewayParaObterTodos(servicos);

            var handler = _fixture.CriarObterTodosServicosHandler();

            // Act
            var resultado = await handler.Handle();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(3);

            // Verificar que o gateway foi chamado
            await _fixture.RepositorioServico.Received(1).ObterTodosAsync();

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterTodos.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoObterTodos.Received(1).LogFim(Arg.Any<string>(), Arg.Any<object>());
        }

        [Fact]
        public async Task Handle_ComListaVazia_DeveRetornarListaVazia()
        {
            // Arrange
            var servicos = new List<Servico>();

            _fixture.ConfigurarMockServicoGatewayParaObterTodos(servicos);

            var handler = _fixture.CriarObterTodosServicosHandler();

            // Act
            var resultado = await handler.Handle();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();

            // Verificar que o gateway foi chamado
            await _fixture.RepositorioServico.Received(1).ObterTodosAsync();

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterTodos.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoObterTodos.Received(1).LogFim(Arg.Any<string>(), Arg.Any<object>());
        }

        [Fact]
        public async Task Handle_ComExcecaoInesperada_DevePropagaExcecao()
        {
            // Arrange
            // Configurar o repositório para lançar uma exceção
            _fixture.RepositorioServico.ObterTodosAsync()
                .Returns(Task.FromException<IEnumerable<ServicoEntityDto>>(new InvalidOperationException("Erro simulado")));

            var handler = _fixture.CriarObterTodosServicosHandler();

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
