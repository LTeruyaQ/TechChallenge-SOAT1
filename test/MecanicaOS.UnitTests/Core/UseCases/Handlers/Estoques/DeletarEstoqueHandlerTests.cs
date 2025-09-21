using Core.Entidades;
using Core.Exceptions;
using Core.UseCases.Estoques.DeletarEstoque;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.Handlers;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.Estoques
{
    public class DeletarEstoqueHandlerTests
    {
        private readonly EstoqueHandlerFixture _fixture;

        public DeletarEstoqueHandlerTests()
        {
            _fixture = new EstoqueHandlerFixture();
        }

        [Fact]
        public async Task Handle_ComEstoqueExistente_DeveDeletarERetornarTrue()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            var estoqueExistente = EstoqueHandlerFixture.CriarEstoqueValido();
            estoqueExistente.Id = estoqueId;
            
            // Configurar o gateway para retornar o estoque existente
            _fixture.EstoqueGateway.ObterPorIdAsync(estoqueId).Returns(estoqueExistente);
            
            // Configurar o gateway para simular a deleção com sucesso
            _fixture.ConfigurarMockEstoqueGatewayParaDeletar(estoqueExistente, true);
            
            var handler = _fixture.CriarDeletarEstoqueHandler();

            // Act
            var resultado = await handler.Handle(estoqueId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            
            // Verificar que o gateway foi chamado com os dados corretos
            await _fixture.EstoqueGateway.Received(1).ObterPorIdAsync(estoqueId);
            await _fixture.EstoqueGateway.Received(1).DeletarAsync(estoqueExistente);
            
            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoDeletar.Received(1).LogInicio(Arg.Any<string>(), estoqueId);
            _fixture.LogServicoDeletar.Received(1).LogFim(Arg.Any<string>(), Arg.Any<bool>());
        }

        [Fact]
        public async Task Handle_ComEstoqueInexistente_DeveLancarDadosNaoEncontradosException()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            
            // Configurar o gateway para retornar null
            _fixture.EstoqueGateway.ObterPorIdAsync(estoqueId).Returns((Estoque)null);
            
            var handler = _fixture.CriarDeletarEstoqueHandler();

            // Act & Assert
            var act = async () => await handler.Handle(estoqueId);
            
            await act.Should().ThrowAsync<DadosNaoEncontradosException>()
                .WithMessage("Estoque não encontrado");
            
            // Verificar que o gateway foi chamado para obter o estoque
            await _fixture.EstoqueGateway.Received(1).ObterPorIdAsync(estoqueId);
            
            // Verificar que o gateway NÃO foi chamado para deletar
            await _fixture.EstoqueGateway.DidNotReceive().DeletarAsync(Arg.Any<Estoque>());
            
            // Verificar que o commit NÃO foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceive().Commit();
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoDeletar.Received(1).LogInicio(Arg.Any<string>(), estoqueId);
            _fixture.LogServicoDeletar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosNaoEncontradosException>());
        }

        [Fact]
        public async Task Handle_QuandoCommitFalha_DeveLancarPersistirDadosException()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            var estoqueExistente = EstoqueHandlerFixture.CriarEstoqueValido();
            estoqueExistente.Id = estoqueId;
            
            // Configurar o gateway para retornar o estoque existente
            _fixture.EstoqueGateway.ObterPorIdAsync(estoqueId).Returns(estoqueExistente);
            
            // Configurar o gateway para simular a deleção
            _fixture.ConfigurarMockEstoqueGatewayParaDeletar(estoqueExistente, true);
            
            // Configurar o UDT para falhar no commit
            _fixture.ConfigurarMockUdtParaCommitFalha();
            
            var handler = _fixture.CriarDeletarEstoqueHandler();

            // Act & Assert
            var act = async () => await handler.Handle(estoqueId);
            
            await act.Should().ThrowAsync<PersistirDadosException>()
                .WithMessage("Erro ao deletar estoque");
            
            // Verificar que o gateway foi chamado para obter e deletar o estoque
            await _fixture.EstoqueGateway.Received(1).ObterPorIdAsync(estoqueId);
            await _fixture.EstoqueGateway.Received(1).DeletarAsync(estoqueExistente);
            
            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoDeletar.Received(1).LogInicio(Arg.Any<string>(), estoqueId);
            _fixture.LogServicoDeletar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<PersistirDadosException>());
        }

        [Fact]
        public async Task Handle_DevePassarDadosCorretamenteEntreHandlerERepositorio()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            
            // Criar um estoque com valores específicos para identificar no teste
            var estoqueExistente = new Estoque
            {
                Id = estoqueId,
                Insumo = "Produto para Deleção",
                Descricao = "Descrição do produto para teste de deleção",
                QuantidadeDisponivel = 5,
                QuantidadeMinima = 2,
                Preco = 50.00m,
                Ativo = true,
                DataCadastro = new DateTime(2023, 1, 15),
                DataAtualizacao = new DateTime(2023, 6, 30)
            };
            
            // Capturar o estoque que será passado para o gateway
            Estoque estoqueDeletado = null;
            
            // Configurar o gateway para retornar o estoque existente
            _fixture.EstoqueGateway.ObterPorIdAsync(estoqueId).Returns(estoqueExistente);
            
            // Configurar o gateway para capturar o objeto passado
            _fixture.EstoqueGateway.When(x => x.DeletarAsync(Arg.Any<Estoque>()))
                .Do(callInfo => 
                {
                    estoqueDeletado = callInfo.Arg<Estoque>();
                });
            
            var handler = _fixture.CriarDeletarEstoqueHandler();

            // Act
            var resultado = await handler.Handle(estoqueId);

            // Assert
            // Verificar que o gateway foi chamado
            await _fixture.EstoqueGateway.Received(1).ObterPorIdAsync(estoqueId);
            await _fixture.EstoqueGateway.Received(1).DeletarAsync(Arg.Any<Estoque>());
            
            // Verificar que os dados foram passados corretamente para o gateway
            estoqueDeletado.Should().NotBeNull();
            estoqueDeletado.Should().BeSameAs(estoqueExistente);
            estoqueDeletado.Id.Should().Be(estoqueId);
            estoqueDeletado.Insumo.Should().Be("Produto para Deleção");
            estoqueDeletado.Descricao.Should().Be("Descrição do produto para teste de deleção");
            
            // Verificar que o resultado contém o status correto
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
        }
    }
}
