using Core.Entidades;
using Core.UseCases.OrdensServico.ObterTodosOrdensServico;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.Handlers;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.OrdensServico
{
    public class ObterTodosOrdensServicoHandlerTests
    {
        private readonly OrdemServicoHandlerFixture _fixture;

        public ObterTodosOrdensServicoHandlerTests()
        {
            _fixture = new OrdemServicoHandlerFixture();
        }

        [Fact]
        public async Task Handle_DeveRetornarTodasOrdensServico()
        {
            // Arrange
            var ordensServico = new List<OrdemServico>
            {
                OrdemServicoHandlerFixture.CriarOrdemServicoValida(),
                OrdemServicoHandlerFixture.CriarOrdemServicoValida(),
                OrdemServicoHandlerFixture.CriarOrdemServicoValida()
            };

            _fixture.ConfigurarMockOrdemServicoGatewayParaObterTodos(ordensServico);

            var handler = _fixture.CriarObterTodosOrdensServicoHandler();

            // Act
            var resultado = await handler.Handle();

            // Assert
            resultado.Should().NotBeNull();
            resultado.OrdensServico.Should().NotBeNull();
            resultado.OrdensServico.Should().HaveCount(3);
            
            // Verificar que o gateway foi chamado
            await _fixture.OrdemServicoGateway.Received(1).ObterTodosAsync();

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterTodos.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoObterTodos.Received(1).LogFim(Arg.Any<string>(), Arg.Any<ObterTodosOrdensServicoResponse>());
        }

        [Fact]
        public async Task Handle_ComListaVazia_DeveRetornarListaVazia()
        {
            // Arrange
            var ordensServico = new List<OrdemServico>();

            _fixture.ConfigurarMockOrdemServicoGatewayParaObterTodos(ordensServico);

            var handler = _fixture.CriarObterTodosOrdensServicoHandler();

            // Act
            var resultado = await handler.Handle();

            // Assert
            resultado.Should().NotBeNull();
            resultado.OrdensServico.Should().NotBeNull();
            resultado.OrdensServico.Should().BeEmpty();
            
            // Verificar que o gateway foi chamado
            await _fixture.OrdemServicoGateway.Received(1).ObterTodosAsync();

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterTodos.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoObterTodos.Received(1).LogFim(Arg.Any<string>(), Arg.Any<ObterTodosOrdensServicoResponse>());
        }

        [Fact]
        public async Task Handle_ComExcecaoInesperada_DevePropagaExcecao()
        {
            // Arrange
            // Configurar o gateway para lançar uma exceção
            _fixture.OrdemServicoGateway.ObterTodosAsync()
                .Returns(Task.FromException<IEnumerable<OrdemServico>>(new InvalidOperationException("Erro simulado")));

            var handler = _fixture.CriarObterTodosOrdensServicoHandler();

            // Act & Assert
            var act = async () => await handler.Handle();

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Erro simulado");

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterTodos.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoObterTodos.Received(1).LogErro(Arg.Any<string>(), Arg.Any<InvalidOperationException>());
        }
    }
}
