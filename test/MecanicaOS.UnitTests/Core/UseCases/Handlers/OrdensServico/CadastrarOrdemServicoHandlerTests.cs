using Core.DTOs.Entidades.OrdemServicos;
using Core.DTOs.UseCases.OrdemServico;
using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
using MecanicaOS.UnitTests.Fixtures.Handlers;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.OrdensServico
{
    public class CadastrarOrdemServicoHandlerTests
    {
        private readonly OrdemServicoHandlerFixture _fixture;

        public CadastrarOrdemServicoHandlerTests()
        {
            _fixture = new OrdemServicoHandlerFixture();
        }

        [Fact]
        public async Task Handle_ComDadosValidos_DeveCadastrarOrdemServico()
        {
            // Arrange
            var cliente = OrdemServicoHandlerFixture.CriarClienteValido();
            var veiculo = OrdemServicoHandlerFixture.CriarVeiculoValido(cliente.Id);
            var servico = OrdemServicoHandlerFixture.CriarServicoValido();

            var dto = OrdemServicoHandlerFixture.CriarCadastrarOrdemServicoDto(
                cliente.Id, veiculo.Id, servico.Id);

            // Controller já resolveu cliente e serviço
            dto.Cliente = cliente;
            dto.Servico = servico;
            _fixture.ConfigurarMockRepositorioOrdemServicoParaCadastrar(OrdemServicoHandlerFixture.CriarOrdemServicoValida());
            _fixture.ConfigurarMockUdtParaCommitSucesso();

            var handler = _fixture.CriarCadastrarOrdemServicoHandler();

            // Act
            var resultado = await handler.Handle(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().NotBeNull();
            resultado.ClienteId.Should().Be(cliente.Id);
            resultado.VeiculoId.Should().Be(veiculo.Id);
            resultado.ServicoId.Should().Be(servico.Id);
            resultado.Status.Should().Be(StatusOrdemServico.Recebida);

            // Verificar que o repositório foi chamado
            await _fixture.RepositorioOrdemServico.Received(1).CadastrarAsync(
                Arg.Is<OrdemServicoEntityDto>(os =>
                    os.ClienteId == cliente.Id &&
                    os.VeiculoId == veiculo.Id &&
                    os.ServicoId == servico.Id));

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<CadastrarOrdemServicoUseCaseDto>());
            _fixture.LogServicoCadastrar.Received(1).LogFim(Arg.Any<string>(), Arg.Any<OrdemServico>());
        }

        [Fact]
        public async Task Handle_QuandoCommitFalha_DeveLancarPersistirDadosException()
        {
            // Arrange
            var cliente = OrdemServicoHandlerFixture.CriarClienteValido();
            var veiculo = OrdemServicoHandlerFixture.CriarVeiculoValido(cliente.Id);
            var servico = OrdemServicoHandlerFixture.CriarServicoValido();

            var dto = OrdemServicoHandlerFixture.CriarCadastrarOrdemServicoDto(
                cliente.Id, veiculo.Id, servico.Id);

            // Controller já resolveu cliente e serviço
            dto.Cliente = cliente;
            dto.Servico = servico;

            _fixture.ConfigurarMockRepositorioOrdemServicoParaCadastrar(OrdemServicoHandlerFixture.CriarOrdemServicoValida());
            _fixture.ConfigurarMockUdtParaCommitFalha();

            var handler = _fixture.CriarCadastrarOrdemServicoHandler();

            // Act & Assert
            var act = async () => await handler.Handle(dto);

            await act.Should().ThrowAsync<PersistirDadosException>()
                .WithMessage("Erro ao cadastrar ordem de serviço");

            // Verificar que o repositório foi chamado
            await _fixture.RepositorioOrdemServico.Received(1).CadastrarAsync(Arg.Any<OrdemServicoEntityDto>());

            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<CadastrarOrdemServicoUseCaseDto>());
            _fixture.LogServicoCadastrar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<PersistirDadosException>());
        }

        [Fact]
        public async Task Handle_ComExcecaoInesperada_DevePropagaExcecao()
        {
            // Arrange
            var cliente = OrdemServicoHandlerFixture.CriarClienteValido();
            var veiculo = OrdemServicoHandlerFixture.CriarVeiculoValido(cliente.Id);
            var servico = OrdemServicoHandlerFixture.CriarServicoValido();

            var dto = OrdemServicoHandlerFixture.CriarCadastrarOrdemServicoDto(
                cliente.Id, veiculo.Id, servico.Id);

            // Controller já resolveu cliente e serviço
            dto.Cliente = cliente;
            dto.Servico = servico;

            // Configurar o repositório para lançar uma exceção
            _fixture.RepositorioOrdemServico.CadastrarAsync(Arg.Any<OrdemServicoEntityDto>())
                .Returns(Task.FromException<OrdemServicoEntityDto>(new InvalidOperationException("Erro simulado")));

            var handler = _fixture.CriarCadastrarOrdemServicoHandler();

            // Act & Assert
            var act = async () => await handler.Handle(dto);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Erro simulado");

            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<CadastrarOrdemServicoUseCaseDto>());
            _fixture.LogServicoCadastrar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<InvalidOperationException>());
        }
    }
}
