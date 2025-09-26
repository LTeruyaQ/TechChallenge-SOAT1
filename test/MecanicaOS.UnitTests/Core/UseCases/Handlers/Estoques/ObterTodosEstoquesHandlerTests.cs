using Core.DTOs.Entidades.Estoque;
using Core.Entidades;
using MecanicaOS.UnitTests.Fixtures.Handlers;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.Estoques
{
    public class ObterTodosEstoquesHandlerTests
    {
        private readonly EstoqueHandlerFixture _fixture;

        public ObterTodosEstoquesHandlerTests()
        {
            _fixture = new EstoqueHandlerFixture();
        }

        [Fact]
        public async Task Handle_DeveRetornarTodosOsEstoques()
        {
            // Arrange
            var estoquesEsperados = EstoqueHandlerFixture.CriarListaEstoquesVariados();

            // Configurar o repositório para retornar a lista de estoques
            var dtos = estoquesEsperados.Select(e => new EstoqueEntityDto
            {
                Id = e.Id,
                Insumo = e.Insumo,
                Descricao = e.Descricao,
                QuantidadeDisponivel = e.QuantidadeDisponivel,
                QuantidadeMinima = e.QuantidadeMinima,
                Preco = e.Preco,
                Ativo = e.Ativo,
                DataCadastro = e.DataCadastro,
                DataAtualizacao = e.DataAtualizacao
            }).ToList();
            _fixture.RepositorioEstoque.ObterTodosAsync().Returns(dtos);

            var handler = _fixture.CriarObterTodosEstoquesHandler();

            // Act
            var resultado = await handler.Handle();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(estoquesEsperados.Count);
            resultado.Should().BeEquivalentTo(estoquesEsperados);

            // Verificar que o gateway foi chamado
            await _fixture.RepositorioEstoque.Received(1).ObterTodosAsync();

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterTodos.Received(1).LogInicio(Arg.Any<string>());
            _fixture.LogServicoObterTodos.Received(1).LogFim(Arg.Any<string>(), Arg.Any<IEnumerable<Estoque>>());
        }

        [Fact]
        public async Task Handle_ComListaVazia_DeveRetornarListaVazia()
        {
            // Arrange
            var listaVazia = new List<Estoque>();

            // Configurar o repositório para retornar uma lista vazia
            _fixture.RepositorioEstoque.ObterTodosAsync().Returns(new List<EstoqueEntityDto>());

            var handler = _fixture.CriarObterTodosEstoquesHandler();

            // Act
            var resultado = await handler.Handle();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();

            // Verificar que o gateway foi chamado
            await _fixture.RepositorioEstoque.Received(1).ObterTodosAsync();

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterTodos.Received(1).LogInicio(Arg.Any<string>());
            _fixture.LogServicoObterTodos.Received(1).LogFim(Arg.Any<string>(), Arg.Any<IEnumerable<Estoque>>());
        }

        [Fact]
        public async Task Handle_QuandoGatewayLancaExcecao_DeveRegistrarLogEPropagar()
        {
            // Arrange
            var excecaoEsperada = new Exception("Erro ao obter estoques");

            // Configurar o repositório para lançar uma exceção
            _fixture.RepositorioEstoque.ObterTodosAsync().Returns(Task.FromException<IEnumerable<EstoqueEntityDto>>(excecaoEsperada));

            var handler = _fixture.CriarObterTodosEstoquesHandler();

            // Act & Assert
            var act = async () => await handler.Handle();

            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Erro ao obter estoques");

            // Verificar que o gateway foi chamado
            await _fixture.RepositorioEstoque.Received(1).ObterTodosAsync();

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterTodos.Received(1).LogInicio(Arg.Any<string>());
            _fixture.LogServicoObterTodos.Received(1).LogErro(Arg.Any<string>(), excecaoEsperada);
        }

        [Fact]
        public async Task Handle_DevePassarDadosCorretamenteEntreHandlerERepositorio()
        {
            // Arrange
            // Criar uma lista de estoques com valores específicos para identificar no teste
            var estoquesEspecificos = new List<Estoque>
            {
                new Estoque
                {
                    Id = Guid.NewGuid(),
                    Insumo = "Produto Específico 1",
                    Descricao = "Descrição específica 1",
                    QuantidadeDisponivel = 10,
                    QuantidadeMinima = 5,
                    Preco = 100.00m,
                    Ativo = true,
                    DataCadastro = new DateTime(2023, 1, 15),
                    DataAtualizacao = new DateTime(2023, 6, 30)
                },
                new Estoque
                {
                    Id = Guid.NewGuid(),
                    Insumo = "Produto Específico 2",
                    Descricao = "Descrição específica 2",
                    QuantidadeDisponivel = 20,
                    QuantidadeMinima = 8,
                    Preco = 200.00m,
                    Ativo = true,
                    DataCadastro = new DateTime(2023, 2, 20),
                    DataAtualizacao = new DateTime(2023, 7, 15)
                }
            };

            // Configurar o repositório para retornar a lista específica
            var dtos = estoquesEspecificos.Select(e => new EstoqueEntityDto
            {
                Id = e.Id,
                Insumo = e.Insumo,
                Descricao = e.Descricao,
                QuantidadeDisponivel = e.QuantidadeDisponivel,
                QuantidadeMinima = e.QuantidadeMinima,
                Preco = e.Preco,
                Ativo = e.Ativo,
                DataCadastro = e.DataCadastro,
                DataAtualizacao = e.DataAtualizacao
            }).ToList();
            _fixture.RepositorioEstoque.ObterTodosAsync().Returns(dtos);

            var handler = _fixture.CriarObterTodosEstoquesHandler();

            // Act
            var resultado = await handler.Handle();

            // Assert
            // Verificar que o gateway foi chamado
            await _fixture.RepositorioEstoque.Received(1).ObterTodosAsync();

            // Verificar que o resultado contém exatamente os mesmos dados retornados pelo gateway
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);

            // Verificar que os objetos são os mesmos (referência)
            resultado.Should().ContainInOrder(estoquesEspecificos);

            // Verificar os dados do primeiro estoque
            var primeiroEstoque = resultado.First();
            primeiroEstoque.Insumo.Should().Be("Produto Específico 1");
            primeiroEstoque.Descricao.Should().Be("Descrição específica 1");
            primeiroEstoque.QuantidadeDisponivel.Should().Be(10);
            primeiroEstoque.QuantidadeMinima.Should().Be(5);
            primeiroEstoque.Preco.Should().Be(100.00m);

            // Verificar os dados do segundo estoque
            var segundoEstoque = resultado.Skip(1).First();
            segundoEstoque.Insumo.Should().Be("Produto Específico 2");
            segundoEstoque.Descricao.Should().Be("Descrição específica 2");
            segundoEstoque.QuantidadeDisponivel.Should().Be(20);
            segundoEstoque.QuantidadeMinima.Should().Be(8);
            segundoEstoque.Preco.Should().Be(200.00m);

            // Verificar que os campos técnicos foram preservados
            primeiroEstoque.DataCadastro.Should().Be(new DateTime(2023, 1, 15));
            primeiroEstoque.DataAtualizacao.Should().Be(new DateTime(2023, 6, 30));
            segundoEstoque.DataCadastro.Should().Be(new DateTime(2023, 2, 20));
            segundoEstoque.DataAtualizacao.Should().Be(new DateTime(2023, 7, 15));
        }
    }
}
