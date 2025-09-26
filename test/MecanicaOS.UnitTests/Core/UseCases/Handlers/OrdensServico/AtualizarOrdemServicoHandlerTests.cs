using Core.DTOs.Entidades.OrdemServicos;
using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
using MecanicaOS.UnitTests.Fixtures.Handlers;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.OrdensServico
{
    public class AtualizarOrdemServicoHandlerTests
    {
        private readonly OrdemServicoHandlerFixture _fixture;

        public AtualizarOrdemServicoHandlerTests()
        {
            _fixture = new OrdemServicoHandlerFixture();
        }

        [Fact]
        public async Task Handle_ComDadosValidos_DeveAtualizarOrdemServico()
        {
            // Arrange
            var ordemServico = OrdemServicoHandlerFixture.CriarOrdemServicoValida();
            var dto = OrdemServicoHandlerFixture.CriarAtualizarOrdemServicoDto(
                StatusOrdemServico.EmExecucao, "Descrição atualizada");

            _fixture.ConfigurarMockRepositorioOrdemServicoParaObterPorId(ordemServico.Id, ordemServico);
            _fixture.ConfigurarMockRepositorioOrdemServicoParaEditar();
            _fixture.ConfigurarMockUdtParaCommitSucesso();

            var handler = _fixture.CriarAtualizarOrdemServicoHandler();

            // Act
            var resultado = await handler.Handle(ordemServico.Id, dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().NotBeNull();
            resultado.Status.Should().Be(StatusOrdemServico.EmExecucao);
            resultado.Descricao.Should().Be("Descrição atualizada");

            // Verificar que o repositório foi chamado
            await _fixture.RepositorioOrdemServico.Received(1).EditarAsync(
                Arg.Is<OrdemServicoEntityDto>(os =>
                    os.Id == ordemServico.Id &&
                    os.Status == StatusOrdemServico.EmExecucao &&
                    os.Descricao == "Descrição atualizada"));

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoAtualizar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoAtualizar.Received(1).LogFim(Arg.Any<string>(), Arg.Any<OrdemServico>());
        }

        [Fact]
        public async Task Handle_ComApenasStatus_DeveAtualizarSomenteStatus()
        {
            // Arrange
            var ordemServico = OrdemServicoHandlerFixture.CriarOrdemServicoValida();
            var descricaoOriginal = ordemServico.Descricao;
            var dto = OrdemServicoHandlerFixture.CriarAtualizarOrdemServicoDto(
                StatusOrdemServico.EmExecucao);

            _fixture.ConfigurarMockRepositorioOrdemServicoParaObterPorId(ordemServico.Id, ordemServico);
            _fixture.ConfigurarMockRepositorioOrdemServicoParaEditar();
            _fixture.ConfigurarMockUdtParaCommitSucesso();

            var handler = _fixture.CriarAtualizarOrdemServicoHandler();

            // Act
            var resultado = await handler.Handle(ordemServico.Id, dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().NotBeNull();
            resultado.Status.Should().Be(StatusOrdemServico.EmExecucao);
            resultado.Descricao.Should().Be(descricaoOriginal);

            // Verificar que o repositório foi chamado
            await _fixture.RepositorioOrdemServico.Received(1).EditarAsync(
                Arg.Is<OrdemServicoEntityDto>(os =>
                    os.Id == ordemServico.Id &&
                    os.Status == StatusOrdemServico.EmExecucao &&
                    os.Descricao == descricaoOriginal));

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();
        }

        [Fact]
        public async Task Handle_ComApenasDescricao_DeveAtualizarSomenteDescricao()
        {
            // Arrange
            var ordemServico = OrdemServicoHandlerFixture.CriarOrdemServicoValida();
            var statusOriginal = ordemServico.Status;
            var dto = OrdemServicoHandlerFixture.CriarAtualizarOrdemServicoDto(
                null, "Descrição atualizada");

            _fixture.ConfigurarMockRepositorioOrdemServicoParaObterPorId(ordemServico.Id, ordemServico);
            _fixture.ConfigurarMockRepositorioOrdemServicoParaEditar();
            _fixture.ConfigurarMockUdtParaCommitSucesso();

            var handler = _fixture.CriarAtualizarOrdemServicoHandler();

            // Act
            var resultado = await handler.Handle(ordemServico.Id, dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().NotBeNull();
            resultado.Status.Should().Be(statusOriginal);
            resultado.Descricao.Should().Be("Descrição atualizada");

            // Verificar que o repositório foi chamado
            await _fixture.RepositorioOrdemServico.Received(1).EditarAsync(
                Arg.Is<OrdemServicoEntityDto>(os =>
                    os.Id == ordemServico.Id &&
                    os.Status == statusOriginal &&
                    os.Descricao == "Descrição atualizada"));

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();
        }

        [Fact]
        public async Task Handle_ComOrdemServicoInexistente_DeveLancarDadosNaoEncontradosException()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var dto = OrdemServicoHandlerFixture.CriarAtualizarOrdemServicoDto(
                StatusOrdemServico.EmExecucao, "Descrição atualizada");

            _fixture.ConfigurarMockRepositorioOrdemServicoParaObterPorId(ordemServicoId, null);

            var handler = _fixture.CriarAtualizarOrdemServicoHandler();

            // Act & Assert
            var act = async () => await handler.Handle(ordemServicoId, dto);

            await act.Should().ThrowAsync<DadosNaoEncontradosException>()
                .WithMessage("Ordem de serviço não encontrada");

            // Verificar que o repositório de edição não foi chamado
            await _fixture.RepositorioOrdemServico.DidNotReceive().EditarAsync(Arg.Any<OrdemServicoEntityDto>());

            // Verificar que o commit não foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceive().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoAtualizar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoAtualizar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosNaoEncontradosException>());
        }

        [Fact]
        public async Task Handle_QuandoCommitFalha_DeveLancarPersistirDadosException()
        {
            // Arrange
            var ordemServico = OrdemServicoHandlerFixture.CriarOrdemServicoValida();
            var dto = OrdemServicoHandlerFixture.CriarAtualizarOrdemServicoDto(
                StatusOrdemServico.EmExecucao, "Descrição atualizada");

            _fixture.ConfigurarMockRepositorioOrdemServicoParaObterPorId(ordemServico.Id, ordemServico);
            _fixture.ConfigurarMockRepositorioOrdemServicoParaEditar();
            _fixture.ConfigurarMockUdtParaCommitFalha();

            var handler = _fixture.CriarAtualizarOrdemServicoHandler();

            // Act & Assert
            var act = async () => await handler.Handle(ordemServico.Id, dto);

            await act.Should().ThrowAsync<PersistirDadosException>()
                .WithMessage("Erro ao atualizar ordem de serviço");

            // Verificar que o repositório foi chamado
            await _fixture.RepositorioOrdemServico.Received(1).EditarAsync(Arg.Any<OrdemServicoEntityDto>());

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoAtualizar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoAtualizar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<PersistirDadosException>());
        }

        [Fact]
        public async Task Handle_DeveAtualizarDataAtualizacao()
        {
            // Arrange
            var ordemServico = OrdemServicoHandlerFixture.CriarOrdemServicoValida();
            var dataAtualizacaoOriginal = ordemServico.DataAtualizacao;
            var dto = OrdemServicoHandlerFixture.CriarAtualizarOrdemServicoDto(
                StatusOrdemServico.EmExecucao);

            _fixture.ConfigurarMockRepositorioOrdemServicoParaObterPorId(ordemServico.Id, ordemServico);
            _fixture.ConfigurarMockRepositorioOrdemServicoParaEditar();
            _fixture.ConfigurarMockUdtParaCommitSucesso();

            var handler = _fixture.CriarAtualizarOrdemServicoHandler();

            // Act
            var resultado = await handler.Handle(ordemServico.Id, dto);

            // Assert
            resultado.DataAtualizacao.Should().NotBe(dataAtualizacaoOriginal);
            resultado.DataAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

            // Verificar que o repositório foi chamado com a data atualizada
            await _fixture.RepositorioOrdemServico.Received(1).EditarAsync(
                Arg.Is<OrdemServicoEntityDto>(os => os.DataAtualizacao > dataAtualizacaoOriginal));
        }
    }
}
