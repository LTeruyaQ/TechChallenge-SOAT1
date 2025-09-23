using Core.Exceptions;
using Core.UseCases.OrdensServico.ObterOrdemServico;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.Handlers;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

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

            _fixture.ConfigurarMockOrdemServicoGatewayParaObterPorId(ordemServico.Id, ordemServico);

            var handler = _fixture.CriarObterOrdemServicoHandler();

            // Act
            var resultado = await handler.Handle(ordemServico.Id);

            // Assert
            resultado.Should().NotBeNull();
            resultado.OrdemServico.Should().NotBeNull();
            resultado.OrdemServico.Id.Should().Be(ordemServico.Id);
            resultado.OrdemServico.ClienteId.Should().Be(ordemServico.ClienteId);
            resultado.OrdemServico.VeiculoId.Should().Be(ordemServico.VeiculoId);
            resultado.OrdemServico.ServicoId.Should().Be(ordemServico.ServicoId);
            resultado.OrdemServico.Status.Should().Be(ordemServico.Status);
            resultado.OrdemServico.Descricao.Should().Be(ordemServico.Descricao);

            // Verificar que o gateway foi chamado
            await _fixture.OrdemServicoGateway.Received(1).ObterPorIdAsync(ordemServico.Id);

            // Verificar que os logs foram registrados
            _fixture.LogServicoObter.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<Guid>());
            _fixture.LogServicoObter.Received(1).LogFim(Arg.Any<string>(), Arg.Any<ObterOrdemServicoResponse>());
        }

        [Fact]
        public async Task Handle_ComIdInexistente_DeveLancarDadosNaoEncontradosException()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();

            _fixture.ConfigurarMockOrdemServicoGatewayParaObterPorIdNull(ordemServicoId);

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

            // Verificar que o gateway foi chamado
            await _fixture.OrdemServicoGateway.Received(1).ObterPorIdAsync(ordemServicoId);

            // Verificar que os logs foram registrados
            _fixture.LogServicoObter.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<Guid>());
            _fixture.LogServicoObter.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosNaoEncontradosException>());
        }

        [Fact]
        public async Task Handle_ComExcecaoInesperada_DevePropagaExcecao()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();

            // Configurar o gateway para lançar uma exceção
            _fixture.OrdemServicoGateway.When(x => x.ObterPorIdAsync(ordemServicoId))
                .Do(x => { throw new InvalidOperationException("Erro simulado"); });

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
