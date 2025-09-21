using Core.Entidades;
using Core.Exceptions;
using Core.UseCases.Estoques.ObterEstoque;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.Handlers;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.Estoques
{
    public class ObterEstoqueHandlerTests
    {
        private readonly EstoqueHandlerFixture _fixture;

        public ObterEstoqueHandlerTests()
        {
            _fixture = new EstoqueHandlerFixture();
        }

        [Fact]
        public async Task Handle_ComIdExistente_DeveRetornarEstoqueCorreto()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            var estoqueEsperado = EstoqueHandlerFixture.CriarEstoqueValido();
            estoqueEsperado.Id = estoqueId;
            
            // Configurar o gateway para retornar o estoque esperado
            _fixture.ConfigurarMockEstoqueGatewayParaObterPorId(estoqueId, estoqueEsperado);
            
            var handler = _fixture.CriarObterEstoqueHandler();

            // Act
            var resultado = await handler.Handle(estoqueId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Estoque.Should().NotBeNull();
            resultado.Estoque.Should().Be(estoqueEsperado);
            
            // Verificar que o gateway foi chamado com o ID correto
            await _fixture.EstoqueGateway.Received(1).ObterPorIdAsync(estoqueId);
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoObter.Received(1).LogInicio(Arg.Any<string>(), estoqueId);
            _fixture.LogServicoObter.Received(1).LogFim(Arg.Any<string>(), estoqueEsperado);
        }

        [Fact]
        public async Task Handle_ComIdInexistente_DeveLancarDadosNaoEncontradosException()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            
            // Configurar o gateway para retornar null
            _fixture.EstoqueGateway.ObterPorIdAsync(Arg.Any<Guid>()).Returns((Estoque)null);
            
            var handler = _fixture.CriarObterEstoqueHandler();

            // Act & Assert
            var act = async () => await handler.Handle(estoqueId);
            
            await act.Should().ThrowAsync<DadosNaoEncontradosException>()
                .WithMessage("Estoque não encontrado");
            
            // Verificar que o gateway foi chamado com o ID correto
            await _fixture.EstoqueGateway.Received(1).ObterPorIdAsync(estoqueId);
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoObter.Received(1).LogInicio(Arg.Any<string>(), estoqueId);
            _fixture.LogServicoObter.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosNaoEncontradosException>());
        }

        [Fact]
        public async Task Handle_QuandoGatewayLancaExcecao_DeveRegistrarLogEPropagar()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            var excecaoEsperada = new Exception("Erro no banco de dados");
            
            // Configurar o gateway para lançar uma exceção
            _fixture.EstoqueGateway.When(x => x.ObterPorIdAsync(Arg.Any<Guid>()))
                .Do(x => { throw excecaoEsperada; });
            
            var handler = _fixture.CriarObterEstoqueHandler();

            // Act & Assert
            var act = async () => await handler.Handle(estoqueId);
            
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Erro no banco de dados");
            
            // Verificar que o gateway foi chamado com o ID correto
            await _fixture.EstoqueGateway.Received(1).ObterPorIdAsync(estoqueId);
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoObter.Received(1).LogInicio(Arg.Any<string>(), estoqueId);
            _fixture.LogServicoObter.Received(1).LogErro(Arg.Any<string>(), excecaoEsperada);
        }

        [Fact]
        public async Task Handle_DevePassarDadosCorretamenteEntreHandlerERepositorio()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            
            // Criar um estoque com valores específicos para identificar no teste
            var estoqueEsperado = new Estoque
            {
                Id = estoqueId,
                Insumo = "Produto Específico de Teste",
                Descricao = "Descrição específica para teste de trânsito de dados",
                QuantidadeDisponivel = 42,
                QuantidadeMinima = 10,
                Preco = 123.45m,
                Ativo = true,
                DataCadastro = new DateTime(2023, 1, 15),
                DataAtualizacao = new DateTime(2023, 6, 30)
            };
            
            // Configurar o gateway para retornar o estoque específico
            _fixture.ConfigurarMockEstoqueGatewayParaObterPorId(estoqueId, estoqueEsperado);
            
            var handler = _fixture.CriarObterEstoqueHandler();

            // Act
            var resultado = await handler.Handle(estoqueId);

            // Assert
            // Verificar que o gateway foi chamado com o ID correto
            await _fixture.EstoqueGateway.Received(1).ObterPorIdAsync(estoqueId);
            
            // Verificar que o resultado contém exatamente os mesmos dados retornados pelo gateway
            resultado.Should().NotBeNull();
            resultado.Estoque.Should().NotBeNull();
            resultado.Estoque.Should().BeSameAs(estoqueEsperado);
            
            // Verificar cada propriedade individualmente para garantir que não houve alteração
            resultado.Estoque.Id.Should().Be(estoqueId);
            resultado.Estoque.Insumo.Should().Be("Produto Específico de Teste");
            resultado.Estoque.Descricao.Should().Be("Descrição específica para teste de trânsito de dados");
            resultado.Estoque.QuantidadeDisponivel.Should().Be(42);
            resultado.Estoque.QuantidadeMinima.Should().Be(10);
            resultado.Estoque.Preco.Should().Be(123.45m);
            resultado.Estoque.Ativo.Should().BeTrue();
            resultado.Estoque.DataCadastro.Should().Be(new DateTime(2023, 1, 15));
            resultado.Estoque.DataAtualizacao.Should().Be(new DateTime(2023, 6, 30));
        }
    }
}
