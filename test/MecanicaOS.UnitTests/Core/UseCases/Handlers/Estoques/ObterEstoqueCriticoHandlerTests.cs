using Core.DTOs.Entidades.Estoque;
using Core.Entidades;
using Core.Especificacoes.Base.Interfaces;
using MecanicaOS.UnitTests.Fixtures.Handlers;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.Estoques
{
    public class ObterEstoqueCriticoHandlerTests
    {
        private readonly EstoqueHandlerFixture _fixture;

        public ObterEstoqueCriticoHandlerTests()
        {
            _fixture = new EstoqueHandlerFixture();
        }

        [Fact]
        public async Task Handle_DeveRetornarEstoquesCriticos()
        {
            // Arrange
            var estoque1 = new Estoque
            {
                Id = Guid.NewGuid(),
                Insumo = "Produto Crítico 1",
                Descricao = "Descrição do produto crítico 1",
                QuantidadeDisponivel = 5,
                QuantidadeMinima = 10,
                Preco = 50.00m,
                Ativo = true
            };

            var estoque2 = new Estoque
            {
                Id = Guid.NewGuid(),
                Insumo = "Produto Crítico 2",
                Descricao = "Descrição do produto crítico 2",
                QuantidadeDisponivel = 2,
                QuantidadeMinima = 5,
                Preco = 75.00m,
                Ativo = true
            };

            var estoquesCriticos = new List<Estoque> { estoque1, estoque2 };

            // Configurar o repositório para retornar a lista de estoques críticos
            _fixture.ConfigurarMockRepositorioEstoqueParaObterEstoqueCritico(estoquesCriticos);

            var handler = _fixture.CriarObterEstoqueCriticoHandler();

            // Act
            var resultado = await handler.Handle();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.Should().BeEquivalentTo(estoquesCriticos);

            // Verificar que o repositório foi chamado
            await _fixture.RepositorioEstoque.Received(1).ListarProjetadoAsync<Estoque>(Arg.Any<IEspecificacao<EstoqueEntityDto>>());

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterCritico.Received(1).LogInicio(Arg.Any<string>());
            _fixture.LogServicoObterCritico.Received(1).LogFim(Arg.Any<string>(), Arg.Any<object>());
        }

        [Fact]
        public async Task Handle_ComListaVazia_DeveRetornarListaVazia()
        {
            // Arrange
            var listaVazia = new List<Estoque>();

            // Configurar o repositório para retornar uma lista vazia
            _fixture.ConfigurarMockRepositorioEstoqueParaObterEstoqueCritico(listaVazia);

            var handler = _fixture.CriarObterEstoqueCriticoHandler();

            // Act
            var resultado = await handler.Handle();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();

            // Verificar que o repositório foi chamado
            await _fixture.RepositorioEstoque.Received(1).ListarProjetadoAsync<Estoque>(Arg.Any<IEspecificacao<EstoqueEntityDto>>());

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterCritico.Received(1).LogInicio(Arg.Any<string>());
            _fixture.LogServicoObterCritico.Received(1).LogFim(Arg.Any<string>(), Arg.Any<object>());
        }

        [Fact]
        public async Task Handle_QuandoGatewayLancaExcecao_DeveRegistrarLogEPropagar()
        {
            // Arrange
            var excecaoEsperada = new Exception("Erro no banco de dados");

            // Configurar o repositório para lançar uma exceção
            _fixture.RepositorioEstoque.ListarProjetadoAsync<Estoque>(Arg.Any<IEspecificacao<EstoqueEntityDto>>())
                .Returns(Task.FromException<IEnumerable<Estoque>>(excecaoEsperada));

            var handler = _fixture.CriarObterEstoqueCriticoHandler();

            // Act & Assert
            var act = async () => await handler.Handle();

            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Erro no banco de dados");

            // Verificar que o repositório foi chamado
            await _fixture.RepositorioEstoque.Received(1).ListarProjetadoAsync<Estoque>(Arg.Any<IEspecificacao<EstoqueEntityDto>>());

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterCritico.Received(1).LogInicio(Arg.Any<string>());
            _fixture.LogServicoObterCritico.Received(1).LogErro(Arg.Any<string>(), excecaoEsperada);
        }

        [Fact]
        public async Task Handle_DevePassarDadosCorretamenteEntreHandlerERepositorio()
        {
            // Arrange
            var estoque1 = new Estoque
            {
                Id = Guid.NewGuid(),
                Insumo = "Produto Crítico 1",
                Descricao = "Descrição do produto crítico 1",
                QuantidadeDisponivel = 5,
                QuantidadeMinima = 10,
                Preco = 50.00m,
                Ativo = true,
                DataCadastro = new DateTime(2023, 1, 15),
                DataAtualizacao = new DateTime(2023, 6, 30)
            };

            var estoque2 = new Estoque
            {
                Id = Guid.NewGuid(),
                Insumo = "Produto Crítico 2",
                Descricao = "Descrição do produto crítico 2",
                QuantidadeDisponivel = 2,
                QuantidadeMinima = 5,
                Preco = 75.00m,
                Ativo = true,
                DataCadastro = new DateTime(2023, 2, 20),
                DataAtualizacao = new DateTime(2023, 7, 15)
            };

            var estoquesCriticos = new List<Estoque> { estoque1, estoque2 };

            // Configurar o repositório para retornar a lista específica
            _fixture.ConfigurarMockRepositorioEstoqueParaObterEstoqueCritico(estoquesCriticos);

            var handler = _fixture.CriarObterEstoqueCriticoHandler();

            // Act
            var resultado = await handler.Handle();

            // Assert
            // Verificar que o repositório foi chamado
            await _fixture.RepositorioEstoque.Received(1).ListarProjetadoAsync<Estoque>(Arg.Any<IEspecificacao<EstoqueEntityDto>>());

            // Verificar que o resultado contém exatamente os mesmos dados retornados pelo gateway
            resultado.Should().NotBeNull();
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.Should().BeEquivalentTo(estoquesCriticos);

            // Verificar os dados do primeiro estoque
            var primeiroCritico = resultado.First(e => e.Insumo == "Produto Crítico 1");
            primeiroCritico.Insumo.Should().Be("Produto Crítico 1");
            primeiroCritico.Descricao.Should().Be("Descrição do produto crítico 1");
            primeiroCritico.QuantidadeDisponivel.Should().Be(5);
            primeiroCritico.QuantidadeMinima.Should().Be(10);
            primeiroCritico.Preco.Should().Be(50.00m);
            primeiroCritico.Ativo.Should().BeTrue();
            primeiroCritico.DataCadastro.Should().Be(new DateTime(2023, 1, 15));
            primeiroCritico.DataAtualizacao.Should().Be(new DateTime(2023, 6, 30));

            // Verificar os dados do segundo estoque
            var segundoCritico = resultado.First(e => e.Insumo == "Produto Crítico 2");
            segundoCritico.Insumo.Should().Be("Produto Crítico 2");
            segundoCritico.Descricao.Should().Be("Descrição do produto crítico 2");
            segundoCritico.QuantidadeDisponivel.Should().Be(2);
            segundoCritico.QuantidadeMinima.Should().Be(5);
            segundoCritico.Preco.Should().Be(75.00m);
            segundoCritico.Ativo.Should().BeTrue();
            segundoCritico.DataCadastro.Should().Be(new DateTime(2023, 2, 20));
            segundoCritico.DataAtualizacao.Should().Be(new DateTime(2023, 7, 15));
        }
    }
}
