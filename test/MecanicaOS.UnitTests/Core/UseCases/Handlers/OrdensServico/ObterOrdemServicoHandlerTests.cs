using Core.DTOs.Entidades.OrdemServicos;
using Core.Entidades;
using Core.Especificacoes.Base.Interfaces;
using Core.Exceptions;
using Core.UseCases.OrdensServico.ObterOrdemServico;
using MecanicaOS.UnitTests.Fixtures.Handlers;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.OrdensServico
{
    public class ObterOrdemServicoHandlerTests
    {
        private readonly OrdemServicoHandlerFixture _fixture;

        public ObterOrdemServicoHandlerTests()
        {
            _fixture = new OrdemServicoHandlerFixture();
        }

        [Fact]
        public async Task Handle_ComIdExistente_DeveRetornarOrdemServico()
        {
            // Arrange
            var ordemServico = OrdemServicoHandlerFixture.CriarOrdemServicoValida();

            _fixture.ConfigurarMockRepositorioOrdemServicoParaObterPorId(ordemServico.Id, ordemServico);

            var handler = _fixture.CriarObterOrdemServicoHandler();

            // Act
            var resultado = await handler.Handle(ordemServico.Id);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(ordemServico.Id);
            resultado.ClienteId.Should().Be(ordemServico.ClienteId);
            resultado.VeiculoId.Should().Be(ordemServico.VeiculoId);
            resultado.ServicoId.Should().Be(ordemServico.ServicoId);
            resultado.Status.Should().Be(ordemServico.Status);
            resultado.Descricao.Should().Be(ordemServico.Descricao);

            // Verificar que o repositório foi chamado
            await _fixture.RepositorioOrdemServico.Received(1).ObterUmProjetadoAsync<OrdemServico>(Arg.Any<IEspecificacao<OrdemServicoEntityDto>>());

            // Verificar que os logs foram registrados
            _fixture.LogServicoObter.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<Guid>());
            _fixture.LogServicoObter.Received(1).LogFim(Arg.Any<string>(), Arg.Any<object>());
        }

        [Fact]
        public async Task Handle_ComIdInexistente_DeveLancarDadosNaoEncontradosException()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();

            _fixture.ConfigurarMockRepositorioOrdemServicoParaObterPorId(ordemServicoId, null);

            // Modificar o handler para lançar exceção quando ordemServico for null
            var handler = new ObterOrdemServicoHandler(
                _fixture.OrdemServicoGateway,
                _fixture.LogServicoObter,
                _fixture.UnidadeDeTrabalho,
                _fixture.UsuarioLogadoServico)
            {
                ThrowWhenNull = true
            };

            // Act & Assert
            var act = async () => await handler.Handle(ordemServicoId);

            await act.Should().ThrowAsync<DadosNaoEncontradosException>()
                .WithMessage("Ordem de serviço não encontrada");

            // Verificar que o repositório foi chamado
            await _fixture.RepositorioOrdemServico.Received(1).ObterUmProjetadoAsync<OrdemServico>(Arg.Any<IEspecificacao<OrdemServicoEntityDto>>());

            // Verificar que os logs foram registrados
            _fixture.LogServicoObter.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<Guid>());
            _fixture.LogServicoObter.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosNaoEncontradosException>());
        }

        [Fact]
        public async Task Handle_QuandoRepositorioLancaExcecao_DeveRegistrarLogEPropagar()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var excecaoEsperada = new InvalidOperationException("Erro simulado");

            // Configurar o repositório para lançar uma exceção
            _fixture.RepositorioOrdemServico.ObterUmProjetadoAsync<OrdemServico>(Arg.Any<IEspecificacao<OrdemServicoEntityDto>>())
                .Returns(Task.FromException<OrdemServico>(excecaoEsperada));

            var handler = _fixture.CriarObterOrdemServicoHandler();

            // Act & Assert
            var act = async () => await handler.Handle(ordemServicoId);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Erro simulado");

            // Verificar que os logs foram registrados
            _fixture.LogServicoObter.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<Guid>());
            _fixture.LogServicoObter.Received(1).LogErro(Arg.Any<string>(), Arg.Any<InvalidOperationException>());
        }
    }
}
