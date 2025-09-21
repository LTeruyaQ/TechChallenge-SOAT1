using Core.DTOs.UseCases.Estoque;
using Core.Entidades;
using Core.Exceptions;
using Core.UseCases.Estoques.AtualizarEstoque;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.Handlers;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.Estoques
{
    public class AtualizarEstoqueHandlerTests
    {
        private readonly EstoqueHandlerFixture _fixture;

        public AtualizarEstoqueHandlerTests()
        {
            _fixture = new EstoqueHandlerFixture();
        }

        [Fact]
        public async Task Handle_ComDadosValidos_DeveAtualizarEstoqueCorretamente()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            var request = EstoqueHandlerFixture.CriarAtualizarEstoqueUseCaseDtoValido();
            var estoqueExistente = EstoqueHandlerFixture.CriarEstoqueValido();
            estoqueExistente.Id = estoqueId;
            
            // Configurar o gateway para retornar o estoque existente
            _fixture.EstoqueGateway.ObterPorIdAsync(estoqueId).Returns(estoqueExistente);
            
            // Configurar o gateway para simular a atualização
            _fixture.ConfigurarMockEstoqueGatewayParaAtualizar(estoqueExistente);
            
            var handler = _fixture.CriarAtualizarEstoqueHandler();

            // Act
            var resultado = await handler.Handle(estoqueId, request);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Estoque.Should().NotBeNull();
            
            // Verificar que o gateway foi chamado com os dados corretos
            await _fixture.EstoqueGateway.Received(1).ObterPorIdAsync(estoqueId);
            await _fixture.EstoqueGateway.Received(1).EditarAsync(Arg.Is<Estoque>(e => 
                e.Id == estoqueId && 
                e.Insumo == request.Insumo && 
                e.Descricao == request.Descricao &&
                e.QuantidadeDisponivel == request.QuantidadeDisponivel &&
                e.QuantidadeMinima == request.QuantidadeMinima &&
                e.Preco == request.Preco));
            
            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoAtualizar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoAtualizar.Received(1).LogFim(Arg.Any<string>(), Arg.Any<Estoque>());
        }

        [Fact]
        public async Task Handle_ComEstoqueInexistente_DeveLancarDadosNaoEncontradosException()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            var request = EstoqueHandlerFixture.CriarAtualizarEstoqueUseCaseDtoValido();
            
            // Configurar o gateway para retornar null
            _fixture.EstoqueGateway.ObterPorIdAsync(estoqueId).Returns((Estoque)null);
            
            var handler = _fixture.CriarAtualizarEstoqueHandler();

            // Act & Assert
            var act = async () => await handler.Handle(estoqueId, request);
            
            await act.Should().ThrowAsync<DadosNaoEncontradosException>()
                .WithMessage("Estoque não encontrado");
            
            // Verificar que o gateway foi chamado para obter o estoque
            await _fixture.EstoqueGateway.Received(1).ObterPorIdAsync(estoqueId);
            
            // Verificar que o gateway NÃO foi chamado para editar
            await _fixture.EstoqueGateway.DidNotReceive().EditarAsync(Arg.Any<Estoque>());
            
            // Verificar que o commit NÃO foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceive().Commit();
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoAtualizar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoAtualizar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosNaoEncontradosException>());
        }

        [Fact]
        public async Task Handle_QuandoCommitFalha_DeveLancarPersistirDadosException()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            var request = EstoqueHandlerFixture.CriarAtualizarEstoqueUseCaseDtoValido();
            var estoqueExistente = EstoqueHandlerFixture.CriarEstoqueValido();
            estoqueExistente.Id = estoqueId;
            
            // Configurar o gateway para retornar o estoque existente
            _fixture.EstoqueGateway.ObterPorIdAsync(estoqueId).Returns(estoqueExistente);
            
            // Configurar o gateway para simular a atualização
            _fixture.ConfigurarMockEstoqueGatewayParaAtualizar(estoqueExistente);
            
            // Configurar o UDT para falhar no commit
            _fixture.ConfigurarMockUdtParaCommitFalha();
            
            var handler = _fixture.CriarAtualizarEstoqueHandler();

            // Act & Assert
            var act = async () => await handler.Handle(estoqueId, request);
            
            await act.Should().ThrowAsync<PersistirDadosException>()
                .WithMessage("Erro ao atualizar estoque");
            
            // Verificar que o gateway foi chamado para obter e editar o estoque
            await _fixture.EstoqueGateway.Received(1).ObterPorIdAsync(estoqueId);
            await _fixture.EstoqueGateway.Received(1).EditarAsync(Arg.Any<Estoque>());
            
            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoAtualizar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoAtualizar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<PersistirDadosException>());
        }

        [Fact]
        public async Task Handle_DevePassarDadosCorretamenteEntreHandlerERepositorio()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            
            // Criar um DTO com valores específicos para identificar no teste
            var request = new AtualizarEstoqueUseCaseDto
            {
                Insumo = "Produto Específico para Teste de Atualização",
                Descricao = "Descrição específica para teste de trânsito de dados na atualização",
                QuantidadeDisponivel = 99,
                QuantidadeMinima = 20,
                Preco = 456.78m
            };
            
            // Criar um estoque existente com valores diferentes
            var estoqueExistente = new Estoque
            {
                Id = estoqueId,
                Insumo = "Produto Original",
                Descricao = "Descrição original",
                QuantidadeDisponivel = 10,
                QuantidadeMinima = 5,
                Preco = 123.45m,
                Ativo = true,
                DataCadastro = new DateTime(2023, 1, 15),
                DataAtualizacao = new DateTime(2023, 6, 30)
            };
            
            // Capturar o estoque que será passado para o gateway
            Estoque estoqueAtualizado = null;
            
            // Configurar o gateway para retornar o estoque existente
            _fixture.EstoqueGateway.ObterPorIdAsync(estoqueId).Returns(estoqueExistente);
            
            // Configurar o gateway para capturar o objeto passado
            _fixture.EstoqueGateway.When(x => x.EditarAsync(Arg.Any<Estoque>()))
                .Do(callInfo => 
                {
                    estoqueAtualizado = callInfo.Arg<Estoque>();
                    estoqueAtualizado.DataAtualizacao = DateTime.UtcNow;
                });
            
            var handler = _fixture.CriarAtualizarEstoqueHandler();

            // Act
            var resultado = await handler.Handle(estoqueId, request);

            // Assert
            // Verificar que o gateway foi chamado
            await _fixture.EstoqueGateway.Received(1).ObterPorIdAsync(estoqueId);
            await _fixture.EstoqueGateway.Received(1).EditarAsync(Arg.Any<Estoque>());
            
            // Verificar que os dados foram passados corretamente para o gateway
            estoqueAtualizado.Should().NotBeNull();
            estoqueAtualizado.Id.Should().Be(estoqueId);
            estoqueAtualizado.Insumo.Should().Be("Produto Específico para Teste de Atualização");
            estoqueAtualizado.Descricao.Should().Be("Descrição específica para teste de trânsito de dados na atualização");
            estoqueAtualizado.QuantidadeDisponivel.Should().Be(99);
            estoqueAtualizado.QuantidadeMinima.Should().Be(20);
            estoqueAtualizado.Preco.Should().Be(456.78m);
            
            // Verificar que os campos técnicos foram preservados
            estoqueAtualizado.Ativo.Should().BeTrue(); // Mantido do original
            estoqueAtualizado.DataCadastro.Should().Be(new DateTime(2023, 1, 15)); // Mantido do original
            estoqueAtualizado.DataAtualizacao.Should().NotBe(new DateTime(2023, 6, 30)); // Atualizado
            
            // Verificar que o resultado contém os mesmos dados
            resultado.Should().NotBeNull();
            resultado.Estoque.Should().NotBeNull();
            resultado.Estoque.Should().BeSameAs(estoqueAtualizado);
        }
    }
}
