using Core.DTOs.UseCases.Estoque;
using Core.Entidades;
using Core.Exceptions;
using Core.UseCases.Estoques.CadastrarEstoque;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.Handlers;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.Estoques
{
    public class CadastrarEstoqueHandlerTests
    {
        private readonly EstoqueHandlerFixture _fixture;

        public CadastrarEstoqueHandlerTests()
        {
            _fixture = new EstoqueHandlerFixture();
        }

        [Fact]
        public async Task Handle_ComDadosValidos_DeveCadastrarEstoqueCorretamente()
        {
            // Arrange
            var request = EstoqueHandlerFixture.CriarCadastrarEstoqueUseCaseDtoValido();
            var estoqueEsperado = new Estoque
            {
                Id = Guid.NewGuid(),
                Insumo = request.Insumo,
                Descricao = request.Descricao,
                QuantidadeDisponivel = request.QuantidadeDisponivel,
                QuantidadeMinima = request.QuantidadeMinima,
                Preco = request.Preco,
                Ativo = true,
                DataCadastro = DateTime.UtcNow,
                DataAtualizacao = DateTime.UtcNow
            };
            
            // Configurar o gateway para simular o cadastro
            _fixture.ConfigurarMockEstoqueGatewayParaCadastrar(estoqueEsperado);
            _fixture.EstoqueGateway.ObterTodosAsync().Returns(new List<Estoque>());
            
            var handler = _fixture.CriarCadastrarEstoqueHandler();

            // Act
            var resultado = await handler.Handle(request);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Estoque.Should().NotBeNull();
            
            // Verificar que o gateway foi chamado com os dados corretos
            await _fixture.EstoqueGateway.Received(1).CadastrarAsync(Arg.Is<Estoque>(e => 
                e.Insumo == request.Insumo && 
                e.Descricao == request.Descricao &&
                e.QuantidadeDisponivel == request.QuantidadeDisponivel &&
                e.QuantidadeMinima == request.QuantidadeMinima &&
                e.Preco == request.Preco));
            
            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<CadastrarEstoqueUseCaseDto>());
            _fixture.LogServicoCadastrar.Received(1).LogFim(Arg.Any<string>(), Arg.Any<Estoque>());
        }

        [Fact]
        public async Task Handle_ComInsumoJaCadastrado_DeveLancarDadosJaCadastradosException()
        {
            // Arrange
            var request = EstoqueHandlerFixture.CriarCadastrarEstoqueUseCaseDtoValido();
            var estoqueExistente = new Estoque
            {
                Id = Guid.NewGuid(),
                Insumo = request.Insumo, // Mesmo nome
                Descricao = "Descrição existente",
                QuantidadeDisponivel = 5,
                QuantidadeMinima = 2,
                Preco = 10.00m,
                Ativo = true
            };
            
            // Configurar o gateway para retornar um estoque com o mesmo nome
            _fixture.EstoqueGateway.ObterTodosAsync().Returns(new List<Estoque> { estoqueExistente });
            
            var handler = _fixture.CriarCadastrarEstoqueHandler();

            // Act & Assert
            var act = async () => await handler.Handle(request);
            
            await act.Should().ThrowAsync<DadosJaCadastradosException>()
                .WithMessage("Produto já cadastrado");
            
            // Verificar que o gateway foi chamado para obter todos os estoques
            await _fixture.EstoqueGateway.Received(1).ObterTodosAsync();
            
            // Verificar que o gateway NÃO foi chamado para cadastrar
            await _fixture.EstoqueGateway.DidNotReceive().CadastrarAsync(Arg.Any<Estoque>());
            
            // Verificar que o commit NÃO foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceive().Commit();
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<CadastrarEstoqueUseCaseDto>());
            _fixture.LogServicoCadastrar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosJaCadastradosException>());
        }

        [Fact]
        public async Task Handle_QuandoCommitFalha_DeveLancarPersistirDadosException()
        {
            // Arrange
            var request = EstoqueHandlerFixture.CriarCadastrarEstoqueUseCaseDtoValido();
            
            // Configurar o gateway para simular o cadastro
            _fixture.EstoqueGateway.ObterTodosAsync().Returns(new List<Estoque>());
            
            // Configurar o UDT para falhar no commit
            _fixture.ConfigurarMockUdtParaCommitFalha();
            
            var handler = _fixture.CriarCadastrarEstoqueHandler();

            // Act & Assert
            var act = async () => await handler.Handle(request);
            
            await act.Should().ThrowAsync<PersistirDadosException>()
                .WithMessage("Erro ao cadastrar estoque");
            
            // Verificar que o gateway foi chamado para cadastrar
            await _fixture.EstoqueGateway.Received(1).CadastrarAsync(Arg.Any<Estoque>());
            
            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<CadastrarEstoqueUseCaseDto>());
            _fixture.LogServicoCadastrar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<PersistirDadosException>());
        }

        [Fact]
        public async Task Handle_DevePassarDadosCorretamenteEntreHandlerERepositorio()
        {
            // Arrange
            // Criar um DTO com valores específicos para identificar no teste
            var request = new CadastrarEstoqueUseCaseDto
            {
                Insumo = "Produto Específico para Teste",
                Descricao = "Descrição específica para teste de trânsito de dados",
                QuantidadeDisponivel = 42,
                QuantidadeMinima = 10,
                Preco = 123.45m
            };
            
            // Capturar o estoque que será passado para o gateway
            Estoque estoqueCadastrado = null;
            
            // Configurar o gateway para capturar o objeto passado
            _fixture.EstoqueGateway.ObterTodosAsync().Returns(new List<Estoque>());
            _fixture.EstoqueGateway.When(x => x.CadastrarAsync(Arg.Any<Estoque>()))
                .Do(callInfo => 
                {
                    estoqueCadastrado = callInfo.Arg<Estoque>();
                    estoqueCadastrado.Id = Guid.NewGuid();
                    estoqueCadastrado.DataCadastro = DateTime.UtcNow;
                    estoqueCadastrado.DataAtualizacao = DateTime.UtcNow;
                    estoqueCadastrado.Ativo = true;
                });
            
            var handler = _fixture.CriarCadastrarEstoqueHandler();

            // Act
            var resultado = await handler.Handle(request);

            // Assert
            // Verificar que o gateway foi chamado
            await _fixture.EstoqueGateway.Received(1).CadastrarAsync(Arg.Any<Estoque>());
            
            // Verificar que os dados foram passados corretamente para o gateway
            estoqueCadastrado.Should().NotBeNull();
            estoqueCadastrado.Insumo.Should().Be("Produto Específico para Teste");
            estoqueCadastrado.Descricao.Should().Be("Descrição específica para teste de trânsito de dados");
            estoqueCadastrado.QuantidadeDisponivel.Should().Be(42);
            estoqueCadastrado.QuantidadeMinima.Should().Be(10);
            estoqueCadastrado.Preco.Should().Be(123.45m);
            
            // Verificar que o resultado contém os mesmos dados
            resultado.Should().NotBeNull();
            resultado.Estoque.Should().NotBeNull();
            resultado.Estoque.Should().BeSameAs(estoqueCadastrado);
            resultado.Estoque.Insumo.Should().Be("Produto Específico para Teste");
            resultado.Estoque.Descricao.Should().Be("Descrição específica para teste de trânsito de dados");
            resultado.Estoque.QuantidadeDisponivel.Should().Be(42);
            resultado.Estoque.QuantidadeMinima.Should().Be(10);
            resultado.Estoque.Preco.Should().Be(123.45m);
        }
    }
}
