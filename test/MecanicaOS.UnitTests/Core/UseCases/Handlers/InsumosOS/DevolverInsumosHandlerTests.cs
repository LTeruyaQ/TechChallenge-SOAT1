using Core.DTOs.Requests.OrdemServico.InsumoOS;
using Core.DTOs.UseCases.Estoque;
using Core.Entidades;
using Core.Exceptions;
using Core.Interfaces.Handlers.Estoques;
using Core.UseCases.Estoques.AtualizarEstoque;
using Core.UseCases.Estoques.ObterEstoque;
using Core.UseCases.InsumosOS.DevolverInsumos;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.Handlers;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.InsumosOS
{
    public class DevolverInsumosHandlerTests
    {
        private readonly InsumosOSHandlerFixture _fixture;
        private readonly IObterEstoqueHandler _obterEstoqueHandler;
        private readonly IAtualizarEstoqueHandler _atualizarEstoqueHandler;

        public DevolverInsumosHandlerTests()
        {
            _fixture = new InsumosOSHandlerFixture();
            _obterEstoqueHandler = Substitute.For<IObterEstoqueHandler>();
            _atualizarEstoqueHandler = Substitute.For<IAtualizarEstoqueHandler>();
        }

        [Fact]
        public async Task Handle_ComDadosValidos_DeveDevolverInsumos()
        {
            // Arrange
            var estoque = InsumosOSHandlerFixture.CriarEstoqueValido(10, 2);
            var insumos = new List<DevolverInsumoOSRequest>
            {
                new DevolverInsumoOSRequest
                {
                    EstoqueId = estoque.Id,
                    Quantidade = 2
                }
            };

            // Configurar o mock do ObterEstoqueHandler
            _obterEstoqueHandler.Handle(estoque.Id).Returns(new ObterEstoqueResponse { Estoque = estoque });

            // Configurar o mock do AtualizarEstoqueHandler
            _atualizarEstoqueHandler.Handle(
                Arg.Is<Guid>(id => id == estoque.Id),
                Arg.Any<AtualizarEstoqueUseCaseDto>())
                .Returns(new AtualizarEstoqueResponse { Estoque = estoque });

            // Configurar o mock do UnidadeDeTrabalho
            _fixture.ConfigurarMockUdtParaCommitSucesso();

            var handler = new DevolverInsumosHandler(
                _obterEstoqueHandler,
                _atualizarEstoqueHandler,
                _fixture.LogServicoDevolverInsumos,
                _fixture.UnidadeDeTrabalho,
                _fixture.UsuarioLogadoServico);

            // Act
            var resultado = await handler.Handle(insumos);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();

            // Verificar que o ObterEstoqueHandler foi chamado
            await _obterEstoqueHandler.Received(1).Handle(estoque.Id);

            // Verificar que o AtualizarEstoqueHandler foi chamado com a quantidade atualizada
            await _atualizarEstoqueHandler.Received(1).Handle(
                estoque.Id,
                Arg.Is<AtualizarEstoqueUseCaseDto>(dto => dto.QuantidadeDisponivel == 12));

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoDevolverInsumos.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoDevolverInsumos.Received(1).LogFim(Arg.Any<string>(), Arg.Any<bool>());
        }

        [Fact]
        public async Task Handle_ComMultiplosInsumos_DeveDevolverTodosInsumos()
        {
            // Arrange
            var estoque1 = InsumosOSHandlerFixture.CriarEstoqueValido(10, 2);
            var estoque2 = new Estoque
            {
                Id = Guid.NewGuid(),
                Insumo = "Filtro de Óleo",
                Descricao = "Filtro de óleo compatível",
                Preco = 25.00m,
                QuantidadeDisponivel = 5,
                QuantidadeMinima = 1
            };

            var insumos = new List<DevolverInsumoOSRequest>
            {
                new DevolverInsumoOSRequest
                {
                    EstoqueId = estoque1.Id,
                    Quantidade = 2
                },
                new DevolverInsumoOSRequest
                {
                    EstoqueId = estoque2.Id,
                    Quantidade = 1
                }
            };

            // Configurar o mock do ObterEstoqueHandler
            _obterEstoqueHandler.Handle(estoque1.Id).Returns(new ObterEstoqueResponse { Estoque = estoque1 });
            _obterEstoqueHandler.Handle(estoque2.Id).Returns(new ObterEstoqueResponse { Estoque = estoque2 });

            // Configurar o mock do AtualizarEstoqueHandler
            _atualizarEstoqueHandler.Handle(
                Arg.Is<Guid>(id => id == estoque1.Id),
                Arg.Any<AtualizarEstoqueUseCaseDto>())
                .Returns(new AtualizarEstoqueResponse { Estoque = estoque1 });

            _atualizarEstoqueHandler.Handle(
                Arg.Is<Guid>(id => id == estoque2.Id),
                Arg.Any<AtualizarEstoqueUseCaseDto>())
                .Returns(new AtualizarEstoqueResponse { Estoque = estoque2 });

            // Configurar o mock do UnidadeDeTrabalho
            _fixture.ConfigurarMockUdtParaCommitSucesso();

            var handler = new DevolverInsumosHandler(
                _obterEstoqueHandler,
                _atualizarEstoqueHandler,
                _fixture.LogServicoDevolverInsumos,
                _fixture.UnidadeDeTrabalho,
                _fixture.UsuarioLogadoServico);

            // Act
            var resultado = await handler.Handle(insumos);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();

            // Verificar que o ObterEstoqueHandler foi chamado para cada estoque
            await _obterEstoqueHandler.Received(1).Handle(estoque1.Id);
            await _obterEstoqueHandler.Received(1).Handle(estoque2.Id);

            // Verificar que o AtualizarEstoqueHandler foi chamado com as quantidades atualizadas
            await _atualizarEstoqueHandler.Received(1).Handle(
                estoque1.Id,
                Arg.Is<AtualizarEstoqueUseCaseDto>(dto => dto.QuantidadeDisponivel == 12));

            await _atualizarEstoqueHandler.Received(1).Handle(
                estoque2.Id,
                Arg.Is<AtualizarEstoqueUseCaseDto>(dto => dto.QuantidadeDisponivel == 6));

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoDevolverInsumos.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoDevolverInsumos.Received(1).LogFim(Arg.Any<string>(), Arg.Any<bool>());
        }

        [Fact]
        public async Task Handle_ComEstoqueInexistente_DeveLancarDadosNaoEncontradosException()
        {
            // Arrange
            var estoqueId = Guid.NewGuid();
            var insumos = new List<DevolverInsumoOSRequest>
            {
                new DevolverInsumoOSRequest
                {
                    EstoqueId = estoqueId,
                    Quantidade = 2
                }
            };

            // Configurar o mock do ObterEstoqueHandler para retornar null
            _obterEstoqueHandler.Handle(estoqueId).Returns(new ObterEstoqueResponse { Estoque = null });

            var handler = new DevolverInsumosHandler(
                _obterEstoqueHandler,
                _atualizarEstoqueHandler,
                _fixture.LogServicoDevolverInsumos,
                _fixture.UnidadeDeTrabalho,
                _fixture.UsuarioLogadoServico);

            // Act & Assert
            var act = async () => await handler.Handle(insumos);

            await act.Should().ThrowAsync<DadosNaoEncontradosException>()
                .WithMessage($"Estoque com ID {estoqueId} não encontrado");

            // Verificar que o ObterEstoqueHandler foi chamado
            await _obterEstoqueHandler.Received(1).Handle(estoqueId);

            // Verificar que o AtualizarEstoqueHandler não foi chamado
            await _atualizarEstoqueHandler.DidNotReceive().Handle(
                Arg.Any<Guid>(),
                Arg.Any<AtualizarEstoqueUseCaseDto>());

            // Verificar que o commit não foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceive().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoDevolverInsumos.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoDevolverInsumos.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosNaoEncontradosException>());
        }

        [Fact]
        public async Task Handle_QuandoCommitFalha_DeveLancarPersistirDadosException()
        {
            // Arrange
            var estoque = InsumosOSHandlerFixture.CriarEstoqueValido(10, 2);
            var insumos = new List<DevolverInsumoOSRequest>
            {
                new DevolverInsumoOSRequest
                {
                    EstoqueId = estoque.Id,
                    Quantidade = 2
                }
            };

            // Configurar o mock do ObterEstoqueHandler
            _obterEstoqueHandler.Handle(estoque.Id).Returns(new ObterEstoqueResponse { Estoque = estoque });

            // Configurar o mock do AtualizarEstoqueHandler
            _atualizarEstoqueHandler.Handle(
                Arg.Is<Guid>(id => id == estoque.Id),
                Arg.Any<AtualizarEstoqueUseCaseDto>())
                .Returns(new AtualizarEstoqueResponse { Estoque = estoque });

            // Configurar o mock do UnidadeDeTrabalho para falhar no commit
            _fixture.ConfigurarMockUdtParaCommitFalha();

            var handler = new DevolverInsumosHandler(
                _obterEstoqueHandler,
                _atualizarEstoqueHandler,
                _fixture.LogServicoDevolverInsumos,
                _fixture.UnidadeDeTrabalho,
                _fixture.UsuarioLogadoServico);

            // Act & Assert
            var act = async () => await handler.Handle(insumos);

            await act.Should().ThrowAsync<PersistirDadosException>()
                .WithMessage("Erro ao devolver insumos ao estoque");

            // Verificar que o ObterEstoqueHandler foi chamado
            await _obterEstoqueHandler.Received(1).Handle(estoque.Id);

            // Verificar que o AtualizarEstoqueHandler foi chamado
            await _atualizarEstoqueHandler.Received(1).Handle(
                estoque.Id,
                Arg.Is<AtualizarEstoqueUseCaseDto>(dto => dto.QuantidadeDisponivel == 12));

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoDevolverInsumos.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoDevolverInsumos.Received(1).LogErro(Arg.Any<string>(), Arg.Any<PersistirDadosException>());
        }

        [Fact]
        public async Task Handle_DevePassarDadosCorretamenteEntreHandlerERepositorio()
        {
            // Arrange
            var estoque = InsumosOSHandlerFixture.CriarEstoqueValido(10, 2);
            var quantidadeDevolver = 3;
            var insumos = new List<DevolverInsumoOSRequest>
            {
                new DevolverInsumoOSRequest
                {
                    EstoqueId = estoque.Id,
                    Quantidade = quantidadeDevolver
                }
            };

            // Configurar o mock do ObterEstoqueHandler
            _obterEstoqueHandler.Handle(estoque.Id).Returns(new ObterEstoqueResponse { Estoque = estoque });

            // Capturar o DTO passado para o AtualizarEstoqueHandler
            AtualizarEstoqueUseCaseDto dtoCapturado = null;
            _atualizarEstoqueHandler.Handle(
                Arg.Is<Guid>(id => id == estoque.Id),
                Arg.Do<AtualizarEstoqueUseCaseDto>(dto => dtoCapturado = dto))
                .Returns(new AtualizarEstoqueResponse { Estoque = estoque });

            // Configurar o mock do UnidadeDeTrabalho
            _fixture.ConfigurarMockUdtParaCommitSucesso();

            var handler = new DevolverInsumosHandler(
                _obterEstoqueHandler,
                _atualizarEstoqueHandler,
                _fixture.LogServicoDevolverInsumos,
                _fixture.UnidadeDeTrabalho,
                _fixture.UsuarioLogadoServico);

            // Act
            var resultado = await handler.Handle(insumos);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();

            // Verificar que o DTO capturado tem os valores corretos
            dtoCapturado.Should().NotBeNull();
            dtoCapturado.Insumo.Should().Be(estoque.Insumo);
            dtoCapturado.QuantidadeDisponivel.Should().Be(13); // 10 (inicial) + 3 (devolvidos)
            dtoCapturado.QuantidadeMinima.Should().Be(estoque.QuantidadeMinima);
            dtoCapturado.Preco.Should().Be(estoque.Preco);

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();
        }
    }
}
