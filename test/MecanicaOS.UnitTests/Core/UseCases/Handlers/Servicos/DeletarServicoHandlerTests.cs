using Core.DTOs.Entidades.Servico;
using Core.Entidades;
using Core.Exceptions;
using MecanicaOS.UnitTests.Fixtures.Handlers;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.Servicos
{
    public class DeletarServicoHandlerTests
    {
        private readonly ServicoHandlerFixture _fixture;

        public DeletarServicoHandlerTests()
        {
            _fixture = new ServicoHandlerFixture();
        }

        [Fact]
        public async Task Handle_ComIdExistente_DeveExecutarSemErros()
        {
            // Arrange
            var servico = ServicoHandlerFixture.CriarServicoValido();

            _fixture.ConfigurarMockServicoGatewayParaObterPorId(servico.Id, servico);
            _fixture.ConfigurarMockUdtParaCommitSucesso();

            var handler = _fixture.CriarDeletarServicoHandler();

            // Act
            var resultado = await handler.Handle(servico.Id);

            // Assert
            resultado.Should().BeTrue();

            // Verificar que o repositório foi chamado para deletar (através do gateway real)
            await _fixture.RepositorioServico.Received(1).DeletarAsync(Arg.Any<ServicoEntityDto>());

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoDeletar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<Guid>());
            _fixture.LogServicoDeletar.Received(1).LogFim(Arg.Any<string>(), Arg.Any<Servico>());
        }

        [Fact]
        public async Task Handle_ComIdInexistente_DeveLancarDadosNaoEncontradosException()
        {
            // Arrange
            var id = Guid.NewGuid();

            _fixture.ConfigurarMockServicoGatewayParaObterPorIdNull(id);

            var handler = _fixture.CriarDeletarServicoHandler();

            // Act & Assert
            var act = async () => await handler.Handle(id);

            await act.Should().ThrowAsync<DadosNaoEncontradosException>()
                .WithMessage("Serviço não encontrado");

            // Verificar que o repositório não foi chamado para deletar (pois o serviço não foi encontrado)
            await _fixture.RepositorioServico.DidNotReceive().DeletarAsync(Arg.Any<ServicoEntityDto>());

            // Verificar que o commit não foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceive().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoDeletar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<Guid>());
            _fixture.LogServicoDeletar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosNaoEncontradosException>());
        }

        [Fact]
        public async Task Handle_QuandoCommitFalha_DeveLancarPersistirDadosException()
        {
            // Arrange
            var servico = ServicoHandlerFixture.CriarServicoValido();

            _fixture.ConfigurarMockServicoGatewayParaObterPorId(servico.Id, servico);
            _fixture.ConfigurarMockUdtParaCommitFalha();

            var handler = _fixture.CriarDeletarServicoHandler();

            // Act & Assert
            var act = async () => await handler.Handle(servico.Id);

            await act.Should().ThrowAsync<PersistirDadosException>()
                .WithMessage("Erro ao deletar serviço");

            // Verificar que o repositório foi chamado para deletar (através do gateway real)
            await _fixture.RepositorioServico.Received(1).DeletarAsync(Arg.Any<ServicoEntityDto>());

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoDeletar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<Guid>());
            _fixture.LogServicoDeletar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<PersistirDadosException>());
        }
    }
}
