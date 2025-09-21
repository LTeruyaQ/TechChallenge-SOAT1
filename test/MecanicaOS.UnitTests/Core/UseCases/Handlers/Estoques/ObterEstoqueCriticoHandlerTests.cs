using Core.Entidades;
using Core.UseCases.Estoques.ObterEstoqueCritico;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.Handlers;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

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
            var estoquesCriticos = new List<Estoque>
            {
                EstoqueHandlerFixture.CriarEstoqueCritico(),
                new Estoque
                {
                    Id = Guid.NewGuid(),
                    Insumo = "Pastilha de Freio",
                    Descricao = "Pastilha de freio dianteira",
                    QuantidadeDisponivel = 0,
                    QuantidadeMinima = 6,
                    Preco = 89.90m,
                    Ativo = true,
                    DataCadastro = DateTime.UtcNow.AddDays(-10),
                    DataAtualizacao = DateTime.UtcNow.AddHours(-6)
                }
            };
            
            // Configurar o gateway para retornar a lista de estoques críticos
            _fixture.EstoqueGateway.ObterEstoqueCriticoAsync().Returns(estoquesCriticos);
            
            var handler = _fixture.CriarObterEstoqueCriticoHandler();

            // Act
            var resultado = await handler.Handle();

            // Assert
            resultado.Should().NotBeNull();
            resultado.EstoquesCriticos.Should().NotBeNull();
            resultado.EstoquesCriticos.Should().HaveCount(2);
            resultado.EstoquesCriticos.Should().BeEquivalentTo(estoquesCriticos);
            
            // Verificar que o gateway foi chamado
            await _fixture.EstoqueGateway.Received(1).ObterEstoqueCriticoAsync();
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoObterCritico.Received(1).LogInicio(Arg.Any<string>());
            _fixture.LogServicoObterCritico.Received(1).LogFim(Arg.Any<string>(), Arg.Any<IEnumerable<Estoque>>());
        }

        [Fact]
        public async Task Handle_ComListaVazia_DeveRetornarListaVazia()
        {
            // Arrange
            var listaVazia = new List<Estoque>();
            
            // Configurar o gateway para retornar uma lista vazia
            _fixture.EstoqueGateway.ObterEstoqueCriticoAsync().Returns(listaVazia);
            
            var handler = _fixture.CriarObterEstoqueCriticoHandler();

            // Act
            var resultado = await handler.Handle();

            // Assert
            resultado.Should().NotBeNull();
            resultado.EstoquesCriticos.Should().NotBeNull();
            resultado.EstoquesCriticos.Should().BeEmpty();
            
            // Verificar que o gateway foi chamado
            await _fixture.EstoqueGateway.Received(1).ObterEstoqueCriticoAsync();
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoObterCritico.Received(1).LogInicio(Arg.Any<string>());
            _fixture.LogServicoObterCritico.Received(1).LogFim(Arg.Any<string>(), Arg.Any<IEnumerable<Estoque>>());
        }

        [Fact]
        public async Task Handle_QuandoGatewayLancaExcecao_DeveRegistrarLogEPropagar()
        {
            // Arrange
            var excecaoEsperada = new Exception("Erro ao obter estoques críticos");
            
            // Configurar o gateway para lançar uma exceção
            _fixture.EstoqueGateway.ObterEstoqueCriticoAsync().Returns<IEnumerable<Estoque>>(x => { throw excecaoEsperada; });
            
            var handler = _fixture.CriarObterEstoqueCriticoHandler();

            // Act & Assert
            var act = async () => await handler.Handle();
            
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Erro ao obter estoques críticos");
            
            // Verificar que o gateway foi chamado
            await _fixture.EstoqueGateway.Received(1).ObterEstoqueCriticoAsync();
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoObterCritico.Received(1).LogInicio(Arg.Any<string>());
            _fixture.LogServicoObterCritico.Received(1).LogErro(Arg.Any<string>(), excecaoEsperada);
        }

        [Fact]
        public async Task Handle_DevePassarDadosCorretamenteEntreHandlerERepositorio()
        {
            // Arrange
            // Criar uma lista de estoques críticos com valores específicos para identificar no teste
            var estoquesCriticos = new List<Estoque>
            {
                new Estoque
                {
                    Id = Guid.NewGuid(),
                    Insumo = "Produto Crítico 1",
                    Descricao = "Descrição do produto crítico 1",
                    QuantidadeDisponivel = 2,
                    QuantidadeMinima = 10,
                    Preco = 150.00m,
                    Ativo = true,
                    DataCadastro = new DateTime(2023, 1, 15),
                    DataAtualizacao = new DateTime(2023, 6, 30)
                },
                new Estoque
                {
                    Id = Guid.NewGuid(),
                    Insumo = "Produto Zerado",
                    Descricao = "Descrição do produto zerado",
                    QuantidadeDisponivel = 0,
                    QuantidadeMinima = 5,
                    Preco = 75.50m,
                    Ativo = true,
                    DataCadastro = new DateTime(2023, 2, 20),
                    DataAtualizacao = new DateTime(2023, 7, 15)
                }
            };
            
            // Configurar o gateway para retornar a lista específica
            _fixture.EstoqueGateway.ObterEstoqueCriticoAsync().Returns(estoquesCriticos);
            
            var handler = _fixture.CriarObterEstoqueCriticoHandler();

            // Act
            var resultado = await handler.Handle();

            // Assert
            // Verificar que o gateway foi chamado
            await _fixture.EstoqueGateway.Received(1).ObterEstoqueCriticoAsync();
            
            // Verificar que o resultado contém exatamente os mesmos dados retornados pelo gateway
            resultado.Should().NotBeNull();
            resultado.EstoquesCriticos.Should().NotBeNull();
            resultado.EstoquesCriticos.Should().HaveCount(2);
            
            // Verificar que os objetos são os mesmos (referência)
            resultado.EstoquesCriticos.Should().ContainInOrder(estoquesCriticos);
            
            // Verificar os dados do primeiro estoque
            var primeiroCritico = resultado.EstoquesCriticos.First();
            primeiroCritico.Insumo.Should().Be("Produto Crítico 1");
            primeiroCritico.Descricao.Should().Be("Descrição do produto crítico 1");
            primeiroCritico.QuantidadeDisponivel.Should().Be(2);
            primeiroCritico.QuantidadeMinima.Should().Be(10);
            primeiroCritico.Preco.Should().Be(150.00m);
            
            // Verificar os dados do segundo estoque
            var segundoCritico = resultado.EstoquesCriticos.Skip(1).First();
            segundoCritico.Insumo.Should().Be("Produto Zerado");
            segundoCritico.Descricao.Should().Be("Descrição do produto zerado");
            segundoCritico.QuantidadeDisponivel.Should().Be(0);
            segundoCritico.QuantidadeMinima.Should().Be(5);
            segundoCritico.Preco.Should().Be(75.50m);
            
            // Verificar que os campos técnicos foram preservados
            primeiroCritico.DataCadastro.Should().Be(new DateTime(2023, 1, 15));
            primeiroCritico.DataAtualizacao.Should().Be(new DateTime(2023, 6, 30));
            segundoCritico.DataCadastro.Should().Be(new DateTime(2023, 2, 20));
            segundoCritico.DataAtualizacao.Should().Be(new DateTime(2023, 7, 15));
        }
    }
}
