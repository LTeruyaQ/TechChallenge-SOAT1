using Core.DTOs.UseCases.Eventos;
using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
using Core.UseCases.OrdensServico.RecusarOrcamento;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.Handlers;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.OrdensServico
{
    public class RecusarOrcamentoHandlerTests
    {
        private readonly OrdemServicoHandlerFixture _fixture;

        public RecusarOrcamentoHandlerTests()
        {
            _fixture = new OrdemServicoHandlerFixture();
        }

        [Fact]
        public async Task Handle_ComOrdemServicoValida_DeveAtualizarStatusParaCancelada()
        {
            // Arrange
            var ordemServico = OrdemServicoHandlerFixture.CriarOrdemServicoValida(StatusOrdemServico.AguardandoAprovação);
            ordemServico.DataEnvioOrcamento = DateTime.UtcNow.AddDays(-1); // Orçamento enviado ontem

            _fixture.ConfigurarMockOrdemServicoGatewayParaObterPorId(ordemServico.Id, ordemServico);
            _fixture.ConfigurarMockUdtParaCommitSucesso();

            var handler = _fixture.CriarRecusarOrcamentoHandler();

            // Act
            var resultado = await handler.Handle(ordemServico.Id);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();

            // Verificar que o gateway foi chamado para editar com status atualizado
            await _fixture.OrdemServicoGateway.ReceivedWithAnyArgs().EditarAsync(
                Arg.Is<OrdemServico>(os => 
                    os.Id == ordemServico.Id && 
                    os.Status == StatusOrdemServico.Cancelada));

            // Verificar que o evento foi publicado
            await _fixture.EventosGateway.ReceivedWithAnyArgs().Publicar(Arg.Any<OrdemServicoCanceladaEventDTO>());

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.ReceivedWithAnyArgs().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoRecusarOrcamento.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<Guid>());
            _fixture.LogServicoRecusarOrcamento.Received(1).LogFim(Arg.Any<string>(), Arg.Any<OrdemServico>());
        }

        [Fact]
        public async Task Handle_ComOrdemServicoInexistente_DeveLancarDadosNaoEncontradosException()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();

            _fixture.ConfigurarMockOrdemServicoGatewayParaObterPorIdNull(ordemServicoId);

            var handler = _fixture.CriarRecusarOrcamentoHandler();

            // Act & Assert
            var act = async () => await handler.Handle(ordemServicoId);

            await act.Should().ThrowAsync<DadosNaoEncontradosException>()
                .WithMessage("Ordem de serviço não encontrada");

            // Verificar que o gateway não foi chamado para editar
            await _fixture.OrdemServicoGateway.DidNotReceive().EditarAsync(Arg.Any<OrdemServico>());

            // Verificar que o evento não foi publicado
            await _fixture.EventosGateway.DidNotReceive().Publicar(Arg.Any<OrdemServicoCanceladaEventDTO>());

            // Verificar que o commit não foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceive().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoRecusarOrcamento.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<Guid>());
            _fixture.LogServicoRecusarOrcamento.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosNaoEncontradosException>());
        }

        [Fact]
        public async Task Handle_ComStatusInvalido_DeveLancarDadosInvalidosException()
        {
            // Arrange
            var ordemServico = OrdemServicoHandlerFixture.CriarOrdemServicoValida(StatusOrdemServico.Recebida);

            _fixture.ConfigurarMockOrdemServicoGatewayParaObterPorId(ordemServico.Id, ordemServico);

            var handler = _fixture.CriarRecusarOrcamentoHandler();

            // Act & Assert
            var act = async () => await handler.Handle(ordemServico.Id);

            await act.Should().ThrowAsync<DadosInvalidosException>()
                .WithMessage("Ordem de serviço não está aguardando aprovação do orçamento");

            // Verificar que o gateway não foi chamado para editar
            await _fixture.OrdemServicoGateway.DidNotReceive().EditarAsync(Arg.Any<OrdemServico>());

            // Verificar que o evento não foi publicado
            await _fixture.EventosGateway.DidNotReceive().Publicar(Arg.Any<OrdemServicoCanceladaEventDTO>());

            // Verificar que o commit não foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceive().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoRecusarOrcamento.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<Guid>());
            _fixture.LogServicoRecusarOrcamento.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosInvalidosException>());
        }

        [Fact]
        public async Task Handle_QuandoCommitFalha_DeveLancarPersistirDadosException()
        {
            // Arrange
            var ordemServico = OrdemServicoHandlerFixture.CriarOrdemServicoValida(StatusOrdemServico.AguardandoAprovação);
            ordemServico.DataEnvioOrcamento = DateTime.UtcNow.AddDays(-1); // Orçamento enviado ontem

            _fixture.ConfigurarMockOrdemServicoGatewayParaObterPorId(ordemServico.Id, ordemServico);
            _fixture.ConfigurarMockUdtParaCommitFalha();

            var handler = _fixture.CriarRecusarOrcamentoHandler();

            // Act & Assert
            var act = async () => await handler.Handle(ordemServico.Id);

            await act.Should().ThrowAsync<PersistirDadosException>()
                .WithMessage("Erro ao recusar orçamento");

            // Verificar que o gateway foi chamado para editar
            await _fixture.OrdemServicoGateway.Received(1).EditarAsync(Arg.Any<OrdemServico>());

            // Verificar que o evento foi publicado (mesmo com falha no commit)
            await _fixture.EventosGateway.Received(1).Publicar(Arg.Any<OrdemServicoCanceladaEventDTO>());

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoRecusarOrcamento.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<Guid>());
            _fixture.LogServicoRecusarOrcamento.Received(1).LogErro(Arg.Any<string>(), Arg.Any<PersistirDadosException>());
        }
    }
}
