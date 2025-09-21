using Core.DTOs.UseCases.Estoque;
using Core.DTOs.UseCases.OrdemServico.InsumoOS;
using Core.Entidades;
using Core.Exceptions;
using Core.UseCases.InsumosOS.CadastrarInsumos;
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
    public class CadastrarInsumosHandlerTests
    {
        private readonly InsumosOSHandlerFixture _fixture;

        public CadastrarInsumosHandlerTests()
        {
            _fixture = new InsumosOSHandlerFixture();
        }

        [Fact]
        public async Task Handle_ComDadosValidos_DeveCadastrarInsumos()
        {
            // Arrange
            var ordemServico = InsumosOSHandlerFixture.CriarOrdemServicoValida();
            var estoque = InsumosOSHandlerFixture.CriarEstoqueValido(10, 2);
            var insumos = InsumosOSHandlerFixture.CriarListaInsumosValida(estoque.Id, 2);

            _fixture.ConfigurarMockOrdemServicoUseCasesParaObterPorId(ordemServico.Id, ordemServico);
            _fixture.ConfigurarMockEstoqueUseCasesParaObterPorId(estoque.Id, estoque);
            _fixture.ConfigurarMockUdtParaCommitSucesso();

            var handler = _fixture.CriarCadastrarInsumosHandler();

            // Act
            var resultado = await handler.Handle(ordemServico.Id, insumos);

            // Assert
            resultado.Should().NotBeNull();
            resultado.InsumosOS.Should().NotBeNull();
            resultado.InsumosOS.Should().HaveCount(1);

            var insumoOS = resultado.InsumosOS.First();
            insumoOS.OrdemServicoId.Should().Be(ordemServico.Id);
            insumoOS.EstoqueId.Should().Be(estoque.Id);
            insumoOS.Quantidade.Should().Be(2);

            // Verificar que o estoque foi atualizado
            await _fixture.EstoqueUseCases.Received(1).AtualizarUseCaseAsync(
                estoque.Id,
                Arg.Is<AtualizarEstoqueUseCaseDto>(dto => dto.QuantidadeDisponivel == 8));

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoCadastrar.Received(1).LogFim(Arg.Any<string>(), Arg.Any<CadastrarInsumosResponse>());
        }

        [Fact]
        public async Task Handle_ComOrdemServicoInexistente_DeveLancarDadosNaoEncontradosException()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var estoque = InsumosOSHandlerFixture.CriarEstoqueValido();
            var insumos = InsumosOSHandlerFixture.CriarListaInsumosValida(estoque.Id);

            _fixture.OrdemServicoUseCases.ObterPorIdUseCaseAsync(ordemServicoId).Returns((OrdemServico)null);

            var handler = _fixture.CriarCadastrarInsumosHandler();

            // Act & Assert
            var act = async () => await handler.Handle(ordemServicoId, insumos);

            await act.Should().ThrowAsync<DadosNaoEncontradosException>()
                .WithMessage("Ordem de serviço não encontrada");

            // Verificar que o commit não foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceive().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoCadastrar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosNaoEncontradosException>());
        }

        [Fact]
        public async Task Handle_ComEstoqueInsuficiente_DeveLancarDadosInvalidosException()
        {
            // Arrange
            var ordemServico = InsumosOSHandlerFixture.CriarOrdemServicoValida();
            var estoque = InsumosOSHandlerFixture.CriarEstoqueValido(5, 2);
            var insumos = InsumosOSHandlerFixture.CriarListaInsumosValida(estoque.Id, 10); // Quantidade maior que disponível

            _fixture.ConfigurarMockOrdemServicoUseCasesParaObterPorId(ordemServico.Id, ordemServico);
            _fixture.ConfigurarMockEstoqueUseCasesParaObterPorId(estoque.Id, estoque);

            var handler = _fixture.CriarCadastrarInsumosHandler();

            // Act & Assert
            var act = async () => await handler.Handle(ordemServico.Id, insumos);

            await act.Should().ThrowAsync<DadosInvalidosException>()
                .WithMessage($"Estoque insuficiente para o insumo {estoque.Insumo}");

            // Verificar que o estoque não foi atualizado
            await _fixture.EstoqueUseCases.DidNotReceive().AtualizarUseCaseAsync(
                Arg.Any<Guid>(),
                Arg.Any<AtualizarEstoqueUseCaseDto>());

            // Verificar que o commit não foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceive().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoCadastrar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosInvalidosException>());
        }

        [Fact]
        public async Task Handle_QuandoCommitFalha_DeveLancarPersistirDadosException()
        {
            // Arrange
            var ordemServico = InsumosOSHandlerFixture.CriarOrdemServicoValida();
            var estoque = InsumosOSHandlerFixture.CriarEstoqueValido(10, 2);
            var insumos = InsumosOSHandlerFixture.CriarListaInsumosValida(estoque.Id, 2);

            _fixture.ConfigurarMockOrdemServicoUseCasesParaObterPorId(ordemServico.Id, ordemServico);
            _fixture.ConfigurarMockEstoqueUseCasesParaObterPorId(estoque.Id, estoque);
            _fixture.ConfigurarMockUdtParaCommitFalha();

            var handler = _fixture.CriarCadastrarInsumosHandler();

            // Act & Assert
            var act = async () => await handler.Handle(ordemServico.Id, insumos);

            await act.Should().ThrowAsync<PersistirDadosException>()
                .WithMessage("Erro ao cadastrar insumos na ordem de serviço");

            // Verificar que o estoque foi atualizado
            await _fixture.EstoqueUseCases.Received(1).AtualizarUseCaseAsync(
                estoque.Id,
                Arg.Is<AtualizarEstoqueUseCaseDto>(dto => dto.QuantidadeDisponivel == 8));

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoCadastrar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<PersistirDadosException>());
        }

        [Fact]
        public async Task Handle_ComMultiplosInsumos_DeveCadastrarTodosInsumos()
        {
            // Arrange
            var ordemServico = InsumosOSHandlerFixture.CriarOrdemServicoValida();
            
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

            var estoqueIds = new List<Guid> { estoque1.Id, estoque2.Id };
            var quantidades = new List<int> { 2, 1 };
            var insumos = InsumosOSHandlerFixture.CriarListaInsumosMultiplosValida(estoqueIds, quantidades);

            _fixture.ConfigurarMockOrdemServicoUseCasesParaObterPorId(ordemServico.Id, ordemServico);
            _fixture.ConfigurarMockEstoqueUseCasesParaObterPorId(estoque1.Id, estoque1);
            _fixture.ConfigurarMockEstoqueUseCasesParaObterPorId(estoque2.Id, estoque2);
            _fixture.ConfigurarMockUdtParaCommitSucesso();

            var handler = _fixture.CriarCadastrarInsumosHandler();

            // Act
            var resultado = await handler.Handle(ordemServico.Id, insumos);

            // Assert
            resultado.Should().NotBeNull();
            resultado.InsumosOS.Should().NotBeNull();
            resultado.InsumosOS.Should().HaveCount(2);

            // Verificar que os estoques foram atualizados
            await _fixture.EstoqueUseCases.Received(1).AtualizarUseCaseAsync(
                estoque1.Id,
                Arg.Is<AtualizarEstoqueUseCaseDto>(dto => dto.QuantidadeDisponivel == 8));

            await _fixture.EstoqueUseCases.Received(1).AtualizarUseCaseAsync(
                estoque2.Id,
                Arg.Is<AtualizarEstoqueUseCaseDto>(dto => dto.QuantidadeDisponivel == 4));

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoCadastrar.Received(1).LogFim(Arg.Any<string>(), Arg.Any<CadastrarInsumosResponse>());
        }

        [Fact]
        public async Task Handle_ComEstoqueAbaixoDoMinimo_DeveVerificarEstoque()
        {
            // Arrange
            var ordemServico = InsumosOSHandlerFixture.CriarOrdemServicoValida();
            var estoque = InsumosOSHandlerFixture.CriarEstoqueValido(5, 4); // Ficará abaixo do mínimo após retirar 2
            var insumos = InsumosOSHandlerFixture.CriarListaInsumosValida(estoque.Id, 2);

            _fixture.ConfigurarMockOrdemServicoUseCasesParaObterPorId(ordemServico.Id, ordemServico);
            _fixture.ConfigurarMockEstoqueUseCasesParaObterPorId(estoque.Id, estoque);
            _fixture.ConfigurarMockUdtParaCommitSucesso();

            var handler = _fixture.CriarCadastrarInsumosHandler();

            // Act
            var resultado = await handler.Handle(ordemServico.Id, insumos);

            // Assert
            resultado.Should().NotBeNull();
            resultado.InsumosOS.Should().NotBeNull();
            resultado.InsumosOS.Should().HaveCount(1);

            // Verificar que o estoque foi atualizado
            await _fixture.EstoqueUseCases.Received(1).AtualizarUseCaseAsync(
                estoque.Id,
                Arg.Is<AtualizarEstoqueUseCaseDto>(dto => dto.QuantidadeDisponivel == 3));

            // Verificar que o verificador de estoque foi chamado
            await _fixture.VerificarEstoqueJobGateway.Received(1).VerificarEstoqueAsync();

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoCadastrar.Received(1).LogFim(Arg.Any<string>(), Arg.Any<CadastrarInsumosResponse>());
        }

        [Fact]
        public async Task Handle_DevePassarDadosCorretamenteEntreHandlerEResponse()
        {
            // Arrange
            var ordemServico = InsumosOSHandlerFixture.CriarOrdemServicoValida();
            var estoque = InsumosOSHandlerFixture.CriarEstoqueValido(10, 2);
            var insumos = InsumosOSHandlerFixture.CriarListaInsumosValida(estoque.Id, 2);

            _fixture.ConfigurarMockOrdemServicoUseCasesParaObterPorId(ordemServico.Id, ordemServico);
            _fixture.ConfigurarMockEstoqueUseCasesParaObterPorId(estoque.Id, estoque);
            _fixture.ConfigurarMockUdtParaCommitSucesso();

            var handler = _fixture.CriarCadastrarInsumosHandler();

            // Act
            var resultado = await handler.Handle(ordemServico.Id, insumos);

            // Assert
            resultado.Should().NotBeNull();
            resultado.InsumosOS.Should().NotBeNull();
            resultado.InsumosOS.Should().HaveCount(1);

            var insumoOS = resultado.InsumosOS.First();
            insumoOS.OrdemServicoId.Should().Be(ordemServico.Id);
            insumoOS.EstoqueId.Should().Be(estoque.Id);
            insumoOS.Quantidade.Should().Be(2);
            insumoOS.Estoque.Should().NotBeNull();
            insumoOS.Estoque.Id.Should().Be(estoque.Id);
            insumoOS.Estoque.Insumo.Should().Be(estoque.Insumo);
            insumoOS.Estoque.Preco.Should().Be(estoque.Preco);

            // Verificar que o estoque foi atualizado corretamente
            await _fixture.EstoqueUseCases.Received(1).AtualizarUseCaseAsync(
                estoque.Id,
                Arg.Is<AtualizarEstoqueUseCaseDto>(dto => 
                    dto.Insumo == estoque.Insumo &&
                    dto.QuantidadeDisponivel == 8 &&
                    dto.QuantidadeMinima == estoque.QuantidadeMinima &&
                    dto.Preco == estoque.Preco));

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();
        }
    }
}
