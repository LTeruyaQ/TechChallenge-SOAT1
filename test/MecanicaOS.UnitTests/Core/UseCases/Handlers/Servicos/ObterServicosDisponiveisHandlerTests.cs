using Core.DTOs.Entidades.Servico;
using Core.Entidades;
using Core.Especificacoes.Base.Interfaces;
using Core.UseCases.Servicos.ObterServicosDisponiveis;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.Handlers;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.Servicos
{
    public class ObterServicosDisponiveisHandlerTests
    {
        private readonly ServicoHandlerFixture _fixture;

        public ObterServicosDisponiveisHandlerTests()
        {
            _fixture = new ServicoHandlerFixture();
        }

        [Fact]
        public async Task Handle_DeveRetornarServicosDisponiveis()
        {
            // Arrange
            var servicos = new List<Servico>
            {
                ServicoHandlerFixture.CriarServicoValido(),
                ServicoHandlerFixture.CriarServicoValido()
            };
            
            // Configurar o repositório para retornar a lista de serviços
            _fixture.ConfigurarMockRepositorioParaObterDisponiveis(servicos);
            
            var handler = _fixture.CriarObterServicosDisponiveisHandler();

            // Act
            var resultado = await handler.Handle();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Servicos.Should().NotBeNull();
            resultado.Servicos.Should().HaveCount(2);
            resultado.Servicos.Should().BeEquivalentTo(servicos);
            
            // Verificar que o repositório foi chamado
            await _fixture.RepositorioServico.Received(1).ListarProjetadoAsync<Servico>(Arg.Any<IEspecificacao<ServicoEntityDto>>());
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoObterDisponiveis.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoObterDisponiveis.Received(1).LogFim(Arg.Any<string>(), Arg.Any<object>());
        }

        [Fact]
        public async Task Handle_ComListaVazia_DeveRetornarListaVazia()
        {
            // Arrange
            var listaVazia = new List<Servico>();
            
            // Configurar o repositório para retornar uma lista vazia
            _fixture.ConfigurarMockRepositorioParaObterDisponiveis(listaVazia);
            
            var handler = _fixture.CriarObterServicosDisponiveisHandler();

            // Act
            var resultado = await handler.Handle();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Servicos.Should().NotBeNull();
            resultado.Servicos.Should().BeEmpty();
            
            // Verificar que o repositório foi chamado
            await _fixture.RepositorioServico.Received(1).ListarProjetadoAsync<Servico>(Arg.Any<IEspecificacao<ServicoEntityDto>>());
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoObterDisponiveis.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoObterDisponiveis.Received(1).LogFim(Arg.Any<string>(), Arg.Any<object>());
        }

        [Fact]
        public async Task Handle_ComExcecaoInesperada_DevePropagaExcecao()
        {
            // Arrange
            var excecaoEsperada = new InvalidOperationException("Erro simulado");
            
            // Configurar o repositório para lançar uma exceção
            _fixture.RepositorioServico.ListarProjetadoAsync<Servico>(Arg.Any<IEspecificacao<ServicoEntityDto>>())
                .Returns(Task.FromException<IEnumerable<Servico>>(excecaoEsperada));
            
            var handler = _fixture.CriarObterServicosDisponiveisHandler();

            // Act & Assert
            var act = async () => await handler.Handle();
            
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Erro simulado");
            
            // Verificar que o repositório foi chamado
            await _fixture.RepositorioServico.Received(1).ListarProjetadoAsync<Servico>(Arg.Any<IEspecificacao<ServicoEntityDto>>());
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoObterDisponiveis.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<object>());
            _fixture.LogServicoObterDisponiveis.Received(1).LogErro(Arg.Any<string>(), excecaoEsperada);
        }
    }
}
