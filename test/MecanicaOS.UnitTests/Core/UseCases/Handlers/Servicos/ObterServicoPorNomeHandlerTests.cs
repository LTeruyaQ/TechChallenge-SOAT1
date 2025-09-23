using Core.DTOs.Entidades.Servico;
using Core.DTOs.UseCases.Servico;
using Core.Entidades;
using Core.UseCases.Servicos.ObterServicoPorNome;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.Handlers;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.Servicos
{
    public class ObterServicoPorNomeHandlerTests
    {
        private readonly ServicoHandlerFixture _fixture;

        public ObterServicoPorNomeHandlerTests()
        {
            _fixture = new ServicoHandlerFixture();
        }

        [Fact]
        public async Task Handle_ComNomeExistente_DeveRetornarServico()
        {
            // Arrange
            var nome = "Troca de Óleo";
            var servico = ServicoHandlerFixture.CriarServicoValido(nome: nome);

            // Configurar o repositório para retornar o serviço quando chamado com a especificação correta
            _fixture.RepositorioServico
                .ObterUmProjetadoSemRastreamentoAsync<Servico>(Arg.Any<global::Core.Especificacoes.Base.Interfaces.IEspecificacao<ServicoEntityDto>>())
                .Returns(servico);

            var handler = _fixture.CriarObterServicoPorNomeHandler();

            // Act
            var resultado = await handler.Handle(nome);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Servico.Should().NotBeNull();
            resultado.Servico.Nome.Should().Be(nome);
            resultado.Servico.Id.Should().Be(servico.Id);
            resultado.Servico.Descricao.Should().Be(servico.Descricao);
            resultado.Servico.Valor.Should().Be(servico.Valor);
            resultado.Servico.Disponivel.Should().Be(servico.Disponivel);

            // Verificar que o repositório foi chamado com a especificação correta
            await _fixture.RepositorioServico.Received(1).ObterUmProjetadoSemRastreamentoAsync<Servico>(Arg.Any<global::Core.Especificacoes.Base.Interfaces.IEspecificacao<ServicoEntityDto>>());

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterPorNome.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<string>());
            _fixture.LogServicoObterPorNome.Received(1).LogFim(Arg.Any<string>(), Arg.Any<ObterServicoPorNomeResponse>());
        }

        [Fact]
        public async Task Handle_ComNomeInexistente_DeveRetornarNull()
        {
            // Arrange
            var nome = "Serviço Inexistente";

            // Configurar o repositório para retornar null quando chamado com a especificação correta
            _fixture.RepositorioServico
                .ObterUmProjetadoSemRastreamentoAsync<Servico>(Arg.Any<global::Core.Especificacoes.Base.Interfaces.IEspecificacao<ServicoEntityDto>>())
                .Returns((Servico)null);

            var handler = _fixture.CriarObterServicoPorNomeHandler();

            // Act
            var resultado = await handler.Handle(nome);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Servico.Should().BeNull();

            // Verificar que o repositório foi chamado com a especificação correta
            await _fixture.RepositorioServico.Received(1).ObterUmProjetadoSemRastreamentoAsync<Servico>(Arg.Any<global::Core.Especificacoes.Base.Interfaces.IEspecificacao<ServicoEntityDto>>());

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterPorNome.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<string>());
            _fixture.LogServicoObterPorNome.Received(1).LogFim(Arg.Any<string>(), Arg.Any<ObterServicoPorNomeResponse>());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Handle_ComNomeInvalido_DeveRetornarNull(string nome)
        {
            // Arrange
            var handler = _fixture.CriarObterServicoPorNomeHandler();

            // Act
            var resultado = await handler.Handle(nome);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Servico.Should().BeNull();
            
            // Para nomes inválidos, o handler não chama o repositório

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterPorNome.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<string>());
            _fixture.LogServicoObterPorNome.Received(1).LogFim(Arg.Any<string>(), Arg.Any<ObterServicoPorNomeResponse>());
        }

        [Fact]
        public async Task Handle_ComExcecaoInesperada_DevePropagaExcecao()
        {
            // Arrange
            var nome = "Serviço Teste";

            // Configurar o repositório para lançar uma exceção quando chamado com a especificação correta
            _fixture.RepositorioServico
                .ObterUmProjetadoSemRastreamentoAsync<Servico>(Arg.Any<global::Core.Especificacoes.Base.Interfaces.IEspecificacao<ServicoEntityDto>>())
                .Returns(Task.FromException<Servico>(new InvalidOperationException("Erro simulado")));

            var handler = _fixture.CriarObterServicoPorNomeHandler();

            // Act & Assert
            var act = async () => await handler.Handle(nome);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Erro simulado");

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterPorNome.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<string>());
            _fixture.LogServicoObterPorNome.Received(1).LogErro(Arg.Any<string>(), Arg.Any<InvalidOperationException>());
        }
    }
}
