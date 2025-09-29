using Core.DTOs.Entidades.OrdemServicos;
using Core.DTOs.UseCases.Eventos;
using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
using Core.UseCases.OrdensServico.RecusarOrcamento;
using MecanicaOS.UnitTests.Fixtures.Handlers;

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
            var ordemServico = OrdemServicoHandlerFixture.CriarOrdemServicoValida(StatusOrdemServico.AguardandoAprovacao);
            ordemServico.DataEnvioOrcamento = DateTime.UtcNow.AddDays(-1); // Orçamento enviado ontem

            _fixture.ConfigurarMockRepositorioOrdemServicoParaObterPorId(ordemServico.Id, ordemServico);
            _fixture.ConfigurarMockRepositorioOrdemServicoParaEditar();
            _fixture.ConfigurarMockUdtParaCommitSucesso();

            var handler = _fixture.CriarRecusarOrcamentoHandler();

            // Act
            var resultado = await handler.Handle(ordemServico.Id);

            // Assert
            resultado.Should().BeTrue();

            // Verificar que o repositório foi chamado para editar com status atualizado
            await _fixture.RepositorioOrdemServico.Received(1).EditarAsync(
                Arg.Is<OrdemServicoEntityDto>(os =>
                    os.Id == ordemServico.Id &&
                    os.Status == StatusOrdemServico.Cancelada));

            // Verificar que o evento foi publicado
            await _fixture.EventosGateway.Received(1).Publicar(Arg.Any<OrdemServicoCanceladaEventDTO>());

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoRecusarOrcamento.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<Guid>());
            _fixture.LogServicoRecusarOrcamento.Received(1).LogFim(Arg.Any<string>(), Arg.Any<OrdemServico>());
        }

        [Fact]
        public async Task Handle_ComOrdemServicoInexistente_DeveLancarDadosNaoEncontradosException()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();

            _fixture.ConfigurarMockRepositorioOrdemServicoParaObterPorId(ordemServicoId, null);

            var handler = _fixture.CriarRecusarOrcamentoHandler();

            // Act & Assert
            var act = async () => await handler.Handle(ordemServicoId);

            await act.Should().ThrowAsync<DadosNaoEncontradosException>()
                .WithMessage("Ordem de serviço não encontrada");

            // Verificar que o repositório não foi chamado para editar
            await _fixture.RepositorioOrdemServico.DidNotReceive().EditarAsync(Arg.Any<OrdemServicoEntityDto>());

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

            // Importante: Configurar o mock para retornar a ordem de serviço, não null
            _fixture.ConfigurarMockRepositorioOrdemServicoParaObterPorId(ordemServico.Id, ordemServico);

            // Configurar o handler diretamente para garantir o comportamento correto
            var handler = new RecusarOrcamentoHandler(
                _fixture.OrdemServicoGateway,
                _fixture.EventosGateway,
                _fixture.LogServicoRecusarOrcamento,
                _fixture.UnidadeDeTrabalho,
                _fixture.UsuarioLogadoServico);

            // Act & Assert
            var act = async () => await handler.Handle(ordemServico.Id);

            await act.Should().ThrowAsync<DadosInvalidosException>()
                .WithMessage("Ordem de serviço não está aguardando aprovação do orçamento");

            // Verificar que o repositório não foi chamado para editar
            await _fixture.RepositorioOrdemServico.DidNotReceive().EditarAsync(Arg.Any<OrdemServicoEntityDto>());

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
            var ordemServico = OrdemServicoHandlerFixture.CriarOrdemServicoValida(StatusOrdemServico.AguardandoAprovacao);
            ordemServico.DataEnvioOrcamento = DateTime.UtcNow.AddDays(-1); // Orçamento enviado ontem

            _fixture.ConfigurarMockRepositorioOrdemServicoParaObterPorId(ordemServico.Id, ordemServico);
            _fixture.ConfigurarMockRepositorioOrdemServicoParaEditar();
            _fixture.ConfigurarMockUdtParaCommitFalha();

            var handler = _fixture.CriarRecusarOrcamentoHandler();

            // Act & Assert
            var act = async () => await handler.Handle(ordemServico.Id);

            await act.Should().ThrowAsync<PersistirDadosException>()
                .WithMessage("Erro ao recusar orçamento");

            // Verificar que o repositório foi chamado para editar
            await _fixture.RepositorioOrdemServico.Received(1).EditarAsync(Arg.Any<OrdemServicoEntityDto>());

            // Verificar que o evento foi publicado (mesmo com falha no commit)
            await _fixture.EventosGateway.Received(1).Publicar(Arg.Any<OrdemServicoCanceladaEventDTO>());

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoRecusarOrcamento.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<Guid>());
            _fixture.LogServicoRecusarOrcamento.Received(1).LogErro(Arg.Any<string>(), Arg.Any<PersistirDadosException>());
        }

        [Fact]
        public async Task Handle_QuandoRepositorioLancaExcecao_DeveRegistrarLogEPropagar()
        {
            // Arrange
            var ordemServico = OrdemServicoHandlerFixture.CriarOrdemServicoValida(StatusOrdemServico.AguardandoAprovacao);
            ordemServico.DataEnvioOrcamento = DateTime.UtcNow.AddDays(-1); // Orçamento enviado ontem
            var excecaoEsperada = new Exception("Erro no banco de dados");

            _fixture.ConfigurarMockRepositorioOrdemServicoParaLancarExcecaoAoEditar(ordemServico.Id, excecaoEsperada);

            var handler = _fixture.CriarRecusarOrcamentoHandler();

            // Act & Assert
            var act = async () => await handler.Handle(ordemServico.Id);

            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Erro no banco de dados");

            // Verificar que os logs foram registrados
            _fixture.LogServicoRecusarOrcamento.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<Guid>());
            _fixture.LogServicoRecusarOrcamento.Received(1).LogErro(Arg.Any<string>(), excecaoEsperada);
        }
    }
}
