using Core.DTOs.Entidades.Servico;
using Core.Entidades;
using MecanicaOS.UnitTests.Fixtures.Handlers;

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
            resultado.Nome.Should().Be(nome);
            resultado.Id.Should().Be(servico.Id);
            resultado.Descricao.Should().Be(servico.Descricao);
            resultado.Valor.Should().Be(servico.Valor);
            resultado.Disponivel.Should().Be(servico.Disponivel);

            // Verificar que o repositório foi chamado com a especificação correta
            await _fixture.RepositorioServico.Received(1).ObterUmProjetadoSemRastreamentoAsync<Servico>(Arg.Any<global::Core.Especificacoes.Base.Interfaces.IEspecificacao<ServicoEntityDto>>());

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterPorNome.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<string>());
            _fixture.LogServicoObterPorNome.Received(1).LogFim(Arg.Any<string>(), Arg.Any<Servico>());
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
            resultado.Should().BeNull();

            // Verificar que o repositório foi chamado com a especificação correta
            await _fixture.RepositorioServico.Received(1).ObterUmProjetadoSemRastreamentoAsync<Servico>(Arg.Any<global::Core.Especificacoes.Base.Interfaces.IEspecificacao<ServicoEntityDto>>());

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterPorNome.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<string>());
            _fixture.LogServicoObterPorNome.Received(1).LogFim(Arg.Any<string>(), Arg.Any<object>());
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
            resultado.Should().BeNull();

            // Para nomes inválidos, o handler não chama o repositório

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterPorNome.Received(1).LogInicio(Arg.Any<string>(), Arg.Any<string>());
            _fixture.LogServicoObterPorNome.Received(1).LogFim(Arg.Any<string>(), Arg.Any<object>());
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
