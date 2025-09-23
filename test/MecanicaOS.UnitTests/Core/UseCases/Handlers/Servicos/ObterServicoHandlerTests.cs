using Core.DTOs.Entidades.Servico;
using Core.DTOs.UseCases.Servico;
using Core.Entidades;
using Core.Exceptions;
using Core.UseCases.Servicos.ObterServico;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.Handlers;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.Servicos
{
    public class ObterServicoHandlerTests
    {
        private readonly ServicoHandlerFixture _fixture;

        public ObterServicoHandlerTests()
        {
            _fixture = new ServicoHandlerFixture();
        }

        [Fact]
        public async Task Handle_ComIdExistente_DeveRetornarServico()
        {
            // Arrange
            var servico = ServicoHandlerFixture.CriarServicoValido();

            _fixture.ConfigurarMockServicoGatewayParaObterPorId(servico.Id, servico);

            var handler = _fixture.CriarObterServicoHandler();

            // Act
            var resultado = await handler.Handle(servico.Id);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Servico.Should().NotBeNull();
            resultado.Servico.Id.Should().Be(servico.Id);
            resultado.Servico.Nome.Should().Be(servico.Nome);
            resultado.Servico.Descricao.Should().Be(servico.Descricao);
            resultado.Servico.Valor.Should().Be(servico.Valor);
            resultado.Servico.Disponivel.Should().Be(servico.Disponivel);

            // Verificar que o repositório foi chamado (através do gateway real)
            await _fixture.RepositorioServico.Received(1).ObterPorIdAsync(servico.Id);

            // Verificar que os logs foram registrados
            _fixture.LogServicoObter.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<Guid>());
            _fixture.LogServicoObter.Received(1).LogFim(Arg.Any<string>(), Arg.Any<ObterServicoResponse>());
        }

        [Fact]
        public async Task Handle_ComIdInexistente_DeveLancarDadosNaoEncontradosException()
        {
            // Arrange
            var id = Guid.NewGuid();

            _fixture.ConfigurarMockServicoGatewayParaObterPorIdNull(id);

            var handler = _fixture.CriarObterServicoHandler();

            // Act & Assert
            var act = async () => await handler.Handle(id);

            await act.Should().ThrowAsync<DadosNaoEncontradosException>()
                .WithMessage("Serviço não encontrado");

            // Verificar que o repositório foi chamado
            await _fixture.RepositorioServico.Received(1).ObterPorIdAsync(id);

            // Verificar que os logs foram registrados
            _fixture.LogServicoObter.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<Guid>());
            _fixture.LogServicoObter.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosNaoEncontradosException>());
        }

        [Fact]
        public async Task Handle_ComExcecaoInesperada_DevePropagaExcecao()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Configurar o repositório para lançar uma exceção
            _fixture.RepositorioServico.ObterPorIdAsync(id)
                .Returns(Task.FromException<ServicoEntityDto>(new InvalidOperationException("Erro simulado")));

            var handler = _fixture.CriarObterServicoHandler();

            // Act & Assert
            var act = async () => await handler.Handle(id);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Erro simulado");

            // Verificar que os logs foram registrados
            _fixture.LogServicoObter.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<Guid>());
            _fixture.LogServicoObter.Received(1).LogErro(Arg.Any<string>(), Arg.Any<InvalidOperationException>());
        }
    }
}
