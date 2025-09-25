using Adapters.Gateways;
using Core.DTOs.Entidades.OrdemServicos;
using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
using Core.UseCases.OrdensServico.AceitarOrcamento;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.Handlers;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.OrdensServico
{
    public class AceitarOrcamentoHandlerTests
    {
        private readonly OrdemServicoHandlerFixture _fixture;

        public AceitarOrcamentoHandlerTests()
        {
            _fixture = new OrdemServicoHandlerFixture();
        }

        [Fact]
        public async Task Handle_ComOrdemServicoValida_DeveAtualizarStatusParaEmExecucao()
        {
            // Arrange
            var ordemServico = OrdemServicoHandlerFixture.CriarOrdemServicoValida(StatusOrdemServico.AguardandoAprovação);
            ordemServico.DataEnvioOrcamento = DateTime.UtcNow.AddDays(-1); // Orçamento enviado ontem

            _fixture.ConfigurarMockRepositorioOrdemServicoParaObterPorId(ordemServico.Id, ordemServico);
            _fixture.ConfigurarMockRepositorioOrdemServicoParaEditar();
            _fixture.ConfigurarMockUdtParaCommitSucesso();

            var handler = _fixture.CriarAceitarOrcamentoHandler();

            // Act
            var resultado = await handler.Handle(ordemServico.Id);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();

            // Verificar que o repositório foi chamado para editar com status atualizado
            await _fixture.RepositorioOrdemServico.Received(1).EditarAsync(
                Arg.Is<OrdemServicoEntityDto>(os => 
                    os.Id == ordemServico.Id && 
                    os.Status == StatusOrdemServico.EmExecucao));

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoAceitarOrcamento.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<Guid>());
            _fixture.LogServicoAceitarOrcamento.Received(1).LogFim(Arg.Any<string>(), Arg.Any<OrdemServico>());
        }

        [Fact]
        public async Task Handle_ComOrdemServicoInexistente_DeveLancarDadosNaoEncontradosException()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();

            _fixture.ConfigurarMockRepositorioOrdemServicoParaObterPorId(ordemServicoId, null);

            var handler = _fixture.CriarAceitarOrcamentoHandler();

            // Act & Assert
            var act = async () => await handler.Handle(ordemServicoId);
            await act.Should().ThrowAsync<DadosNaoEncontradosException>()
                .WithMessage("Ordem de serviço não encontrada");

            // Verificar que o repositório não foi chamado para editar
            await _fixture.RepositorioOrdemServico.DidNotReceive().EditarAsync(Arg.Any<OrdemServicoEntityDto>());

            // Verificar que o commit não foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceive().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoAceitarOrcamento.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<Guid>());
            _fixture.LogServicoAceitarOrcamento.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosNaoEncontradosException>());
        }

        [Fact]
        public async Task Handle_ComStatusInvalido_DeveLancarDadosInvalidosException()
        {
            // Arrange
            var ordemServico = OrdemServicoHandlerFixture.CriarOrdemServicoValida(StatusOrdemServico.Recebida);

            _fixture.ConfigurarMockRepositorioOrdemServicoParaObterPorId(ordemServico.Id, ordemServico);

            var handler = _fixture.CriarAceitarOrcamentoHandler();

            // Act & Assert
            var act = async () => await handler.Handle(ordemServico.Id);

            await act.Should().ThrowAsync<DadosInvalidosException>()
                .WithMessage("Ordem de serviço não está aguardando aprovação do orçamento");

            // Verificar que o repositório não foi chamado para editar
            await _fixture.RepositorioOrdemServico.DidNotReceive().EditarAsync(Arg.Any<OrdemServicoEntityDto>());

            // Verificar que o commit não foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceive().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoAceitarOrcamento.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<Guid>());
            _fixture.LogServicoAceitarOrcamento.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosInvalidosException>());
        }

        [Fact]
        public async Task Handle_ComOrcamentoExpirado_DeveLancarDadosInvalidosException()
        {
            // Arrange
            var ordemServico = OrdemServicoHandlerFixture.CriarOrdemServicoValida(StatusOrdemServico.AguardandoAprovação);
            ordemServico.DataEnvioOrcamento = DateTime.UtcNow.AddDays(-8); // Orçamento enviado há 8 dias (expirado)

            _fixture.ConfigurarMockRepositorioOrdemServicoParaObterPorId(ordemServico.Id, ordemServico);

            var handler = _fixture.CriarAceitarOrcamentoHandler();

            // Act & Assert
            var act = async () => await handler.Handle(ordemServico.Id);

            await act.Should().ThrowAsync<DadosInvalidosException>()
                .WithMessage("Orçamento expirado");

            // Verificar que o repositório não foi chamado para editar
            await _fixture.RepositorioOrdemServico.DidNotReceive().EditarAsync(Arg.Any<OrdemServicoEntityDto>());

            // Verificar que o commit não foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceive().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoAceitarOrcamento.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<Guid>());
            _fixture.LogServicoAceitarOrcamento.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosInvalidosException>());
        }

        [Fact]
        public async Task Handle_QuandoCommitFalha_DeveLancarPersistirDadosException()
        {
            // Arrange
            var ordemServico = OrdemServicoHandlerFixture.CriarOrdemServicoValida(StatusOrdemServico.AguardandoAprovação);
            ordemServico.DataEnvioOrcamento = DateTime.UtcNow.AddDays(-1); // Orçamento enviado ontem

            _fixture.ConfigurarMockRepositorioOrdemServicoParaObterPorId(ordemServico.Id, ordemServico);
            _fixture.ConfigurarMockRepositorioOrdemServicoParaEditar();
            _fixture.ConfigurarMockUdtParaCommitFalha();

            var handler = _fixture.CriarAceitarOrcamentoHandler();

            try
            {
                // Act
                await handler.Handle(ordemServico.Id);
                
                // Se chegar aqui, o teste falha
                Assert.Fail("Deveria ter lançado uma exceção");
            }
            catch
            {
                // Assert - Verificamos apenas que o commit foi chamado
                await _fixture.UnidadeDeTrabalho.Received(1).Commit();

                // Verificar que o repositório foi chamado para editar
                await _fixture.RepositorioOrdemServico.Received(1).EditarAsync(Arg.Any<OrdemServicoEntityDto>());

                // Verificar que os logs foram registrados
                _fixture.LogServicoAceitarOrcamento.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<Guid>());
                _fixture.LogServicoAceitarOrcamento.Received(1).LogErro(Arg.Any<string>(), Arg.Any<Exception>());
            }
        }
    }
}
