using Aplicacao.DTOs.Requests.Estoque;
using Aplicacao.DTOs.Responses.Estoque;
using Aplicacao.Servicos;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using FluentAssertions;
using Moq;
using Xunit;

namespace Aplicacao.Servicos.Tests
{
    public class EstoqueServicoTests
    {
        private readonly Mock<IRepositorio<Estoque>> _repositorioMock;
        private readonly Mock<ILogServico<EstoqueServico>> _logServicoMock;
        private readonly Mock<IUnidadeDeTrabalho> _uotMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly EstoqueServico _estoqueServico;

        public EstoqueServicoTests()
        {
            _repositorioMock = new Mock<IRepositorio<Estoque>>();
            _logServicoMock = new Mock<ILogServico<EstoqueServico>>();
            _uotMock = new Mock<IUnidadeDeTrabalho>();
            _mapperMock = new Mock<IMapper>();

            _estoqueServico = new EstoqueServico(
                _repositorioMock.Object,
                _logServicoMock.Object,
                _uotMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task Dado_RequestValido_Quando_CadastrarAsync_Entao_RetornaResponse()
        {
            // Arrange
            var request = new CadastrarEstoqueRequest { ProdutoId = Guid.NewGuid(), Quantidade = 10 };
            var estoque = new Estoque { Id = Guid.NewGuid(), ProdutoId = request.ProdutoId, Quantidade = request.Quantidade };
            var response = new EstoqueResponse { Id = estoque.Id, ProdutoId = estoque.ProdutoId, Quantidade = estoque.Quantidade };

            _mapperMock.Setup(m => m.Map<Estoque>(request)).Returns(estoque);
            _repositorioMock.Setup(r => r.CadastrarAsync(estoque)).ReturnsAsync(estoque);
            _uotMock.Setup(u => u.SalvarAlteracoesAsync()).ReturnsAsync(true);
            _mapperMock.Setup(m => m.Map<EstoqueResponse>(estoque)).Returns(response);

            // Act
            var resultado = await _estoqueServico.CadastrarAsync(request);

            // Assert
            resultado.Should().NotBeNull("porque o cadastro deve retornar o estoque cadastrado");
            resultado.Id.Should().Be(estoque.Id, "porque o ID deve ser o mesmo do estoque cadastrado");
            resultado.ProdutoId.Should().Be(request.ProdutoId, "porque o ProdutoId deve ser o mesmo da requisição");
            resultado.Quantidade.Should().Be(request.Quantidade, "porque a quantidade deve ser a mesma da requisição");
            
            _repositorioMock.Verify(
                r => r.CadastrarAsync(It.IsAny<Estoque>()), 
                Times.Once, 
                "porque deve chamar o método de cadastro do repositório");
                
            _uotMock.Verify(
                u => u.SalvarAlteracoesAsync(), 
                Times.Once, 
                "porque deve salvar as alterações na unidade de trabalho");
        }

        [Fact]
        public async Task Dado_RequestInvalido_Quando_CadastrarAsync_Entao_LancaExcecao()
        {
            // Arrange
            CadastrarEstoqueRequest request = null;

            // Act
            Func<Task> act = async () => await _estoqueServico.CadastrarAsync(request);

            // Assert
            await act.Should()
                .ThrowAsync<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'request')");
        }

        [Fact]
        public async Task Dado_EstoqueExistente_Quando_ObterPorIdAsync_Entao_RetornaEstoque()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            var estoque = new Estoque { Id = estoqueId, ProdutoId = Guid.NewGuid(), Quantidade = 10 };
            var estoqueResponse = new EstoqueResponse { Id = estoqueId, ProdutoId = estoque.ProdutoId, Quantidade = estoque.Quantidade };

            _repositorioMock.Setup(r => r.ObterPorIdAsync(estoqueId))
                .ReturnsAsync(estoque);
            _mapperMock.Setup(m => m.Map<EstoqueResponse>(estoque))
                .Returns(estoqueResponse);

            // Act
            var resultado = await _estoqueServico.ObterPorIdAsync(estoqueId);

            // Assert
            resultado.Should().NotBeNull("porque o estoque existe no repositório");
            resultado.Id.Should().Be(estoqueId, "porque deve retornar o estoque com o ID especificado");
            resultado.ProdutoId.Should().Be(estoque.ProdutoId, "porque deve retornar o ProdutoId correto");
            resultado.Quantidade.Should().Be(estoque.Quantidade, "porque deve retornar a quantidade correta");
        }

        [Fact]
        public async Task Dado_EstoqueInexistente_Quando_ObterPorIdAsync_Entao_LancaExcecao()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            _repositorioMock.Setup(r => r.ObterPorIdAsync(estoqueId))
                .ReturnsAsync((Estoque)null);

            // Act
            Func<Task> act = async () => await _estoqueServico.ObterPorIdAsync(estoqueId);

            // Assert
            await act.Should()
                .ThrowAsync<EntidadeNaoEncontradaException>()
                .WithMessage($"Estoque com ID {estoqueId} não encontrado");
        }

        [Fact]
        public async Task Dado_EstoquesExistentes_Quando_ListarTodosAsync_Entao_RetornaListaEstoques()
        {
            // Arrange
            var estoques = new List<Estoque>
            {
                new Estoque { Id = Guid.NewGuid(), ProdutoId = Guid.NewGuid(), Quantidade = 5 },
                new Estoque { Id = Guid.NewGuid(), ProdutoId = Guid.NewGuid(), Quantidade = 10 }
            };

            var estoquesResponse = estoques.Select(e => new EstoqueResponse 
            { 
                Id = e.Id, 
                ProdutoId = e.ProdutoId,
                Quantidade = e.Quantidade
            }).ToList();

            _repositorioMock.Setup(r => r.ListarTodosAsync())
                .ReturnsAsync(estoques);
            _mapperMock.Setup(m => m.Map<IEnumerable<EstoqueResponse>>(estoques))
                .Returns(estoquesResponse);

            // Act
            var resultado = await _estoqueServico.ListarTodosAsync();

            // Assert
            resultado.Should().NotBeNull("porque existem estoques cadastrados");
            resultado.Should().HaveCount(2, "porque existem dois itens de estoque cadastrados");
            resultado.Should().Contain(e => e.Quantidade == 5, "porque o primeiro item deve ter quantidade 5");
            resultado.Should().Contain(e => e.Quantidade == 10, "porque o segundo item deve ter quantidade 10");
        }
    }
}
