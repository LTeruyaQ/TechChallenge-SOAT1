using Core.DTOs.UseCases.OrdemServico;
using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
using Core.UseCases.OrdensServico.CadastrarOrdemServico;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.Handlers;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

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

            _fixture.ConfigurarMockClienteUseCasesParaObterPorId(cliente.Id, cliente);
            _fixture.ConfigurarMockServicoUseCasesParaObterPorId(servico.Id, servico);
            _fixture.ConfigurarMockUdtParaCommitSucesso();

            var handler = _fixture.CriarCadastrarOrdemServicoHandler();

            // Act
            var resultado = await handler.Handle(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.OrdemServico.Should().NotBeNull();
            resultado.OrdemServico.ClienteId.Should().Be(cliente.Id);
            resultado.OrdemServico.VeiculoId.Should().Be(veiculo.Id);
            resultado.OrdemServico.ServicoId.Should().Be(servico.Id);
            resultado.OrdemServico.Status.Should().Be(StatusOrdemServico.Recebida);

            // Verificar que o gateway foi chamado
            await _fixture.OrdemServicoGateway.Received(1).CadastrarAsync(
                Arg.Is<OrdemServico>(os => 
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
        public async Task Handle_ComClienteInexistente_DeveLancarDadosNaoEncontradosException()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var servicoId = Guid.NewGuid();

            var dto = OrdemServicoHandlerFixture.CriarCadastrarOrdemServicoDto(
                clienteId, veiculoId, servicoId);

            _fixture.ConfigurarMockClienteUseCasesParaObterPorIdNull(clienteId);

            var handler = _fixture.CriarCadastrarOrdemServicoHandler();

            // Act & Assert
            var act = async () => await handler.Handle(dto);

            await act.Should().ThrowAsync<DadosNaoEncontradosException>()
                .WithMessage("Cliente não encontrado");

            // Verificar que o gateway não foi chamado
            await _fixture.OrdemServicoGateway.DidNotReceive().CadastrarAsync(Arg.Any<OrdemServico>());

            // Verificar que o commit não foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceive().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<CadastrarOrdemServicoUseCaseDto>());
            _fixture.LogServicoCadastrar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosNaoEncontradosException>());
        }

        [Fact]
        public async Task Handle_ComServicoInexistente_DeveLancarDadosNaoEncontradosException()
        {
            // Arrange
            var cliente = OrdemServicoHandlerFixture.CriarClienteValido();
            var veiculoId = Guid.NewGuid();
            var servicoId = Guid.NewGuid();

            var dto = OrdemServicoHandlerFixture.CriarCadastrarOrdemServicoDto(
                cliente.Id, veiculoId, servicoId);

            _fixture.ConfigurarMockClienteUseCasesParaObterPorId(cliente.Id, cliente);
            _fixture.ConfigurarMockServicoUseCasesParaObterPorIdNull(servicoId);

            var handler = _fixture.CriarCadastrarOrdemServicoHandler();

            // Act & Assert
            var act = async () => await handler.Handle(dto);

            await act.Should().ThrowAsync<DadosNaoEncontradosException>()
                .WithMessage("Serviço não encontrado");

            // Verificar que o gateway não foi chamado
            await _fixture.OrdemServicoGateway.DidNotReceive().CadastrarAsync(Arg.Any<OrdemServico>());

            // Verificar que o commit não foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceive().Commit();

            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<CadastrarOrdemServicoUseCaseDto>());
            _fixture.LogServicoCadastrar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosNaoEncontradosException>());
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

            _fixture.ConfigurarMockClienteUseCasesParaObterPorId(cliente.Id, cliente);
            _fixture.ConfigurarMockServicoUseCasesParaObterPorId(servico.Id, servico);
            _fixture.ConfigurarMockUdtParaCommitFalha();

            var handler = _fixture.CriarCadastrarOrdemServicoHandler();

            // Act & Assert
            var act = async () => await handler.Handle(dto);

            await act.Should().ThrowAsync<PersistirDadosException>()
                .WithMessage("Erro ao cadastrar ordem de serviço");

            // Verificar que o gateway foi chamado
            await _fixture.OrdemServicoGateway.Received(1).CadastrarAsync(Arg.Any<OrdemServico>());

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

            _fixture.ConfigurarMockClienteUseCasesParaObterPorId(cliente.Id, cliente);
            _fixture.ConfigurarMockServicoUseCasesParaObterPorId(servico.Id, servico);

            // Configurar o gateway para lançar uma exceção
            _fixture.OrdemServicoGateway.When(x => x.CadastrarAsync(Arg.Any<OrdemServico>()))
                .Do(x => { throw new InvalidOperationException("Erro simulado"); });

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
