using Core.DTOs.Entidades.Servico;
using Core.Entidades;
using Core.Especificacoes.Base.Interfaces;
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
        public async Task Handle_ComNomeEspecifico_DeveRetornarServicoCorreto()
        {
            // Arrange
            var nome = "Serviço Específico de Teste";
            var servicoId = Guid.NewGuid();
            
            // Criar um serviço com valores específicos para identificar no teste
            var servicoEsperado = new Servico
            {
                Id = servicoId,
                Nome = nome,
                Descricao = "Descrição específica para teste",
                Valor = 123.45m,
                Disponivel = true,
                Ativo = true,
                DataCadastro = new DateTime(2023, 5, 15),
                DataAtualizacao = new DateTime(2023, 6, 20)
            };

            // Configurar o repositório para retornar o serviço específico
            _fixture.RepositorioServico
                .ObterUmProjetadoSemRastreamentoAsync<Servico>(Arg.Any<IEspecificacao<ServicoEntityDto>>())
                .Returns(servicoEsperado);

            var handler = _fixture.CriarObterServicoPorNomeHandler();

            // Act
            var resultado = await handler.Handle(nome);

            // Assert
            // Verificar que o repositório foi chamado com alguma especificação
            await _fixture.RepositorioServico.Received(1).ObterUmProjetadoSemRastreamentoAsync<Servico>(
                Arg.Any<IEspecificacao<ServicoEntityDto>>());

            // Verificar que o resultado contém exatamente os mesmos dados do serviço esperado
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(servicoEsperado);

            // Verificar cada propriedade individualmente para garantir que não houve alteração
            resultado.Id.Should().Be(servicoId);
            resultado.Nome.Should().Be(nome);
            resultado.Descricao.Should().Be("Descrição específica para teste");
            resultado.Valor.Should().Be(123.45m);
            resultado.Disponivel.Should().BeTrue();
            
            // Verificar que os campos técnicos foram preservados
            resultado.Ativo.Should().BeTrue();
            resultado.DataCadastro.Should().Be(new DateTime(2023, 5, 15));
            resultado.DataAtualizacao.Should().Be(new DateTime(2023, 6, 20));

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterPorNome.Received(1).LogInicio(Arg.Any<string>(), nome);
            _fixture.LogServicoObterPorNome.Received(1).LogFim(Arg.Any<string>(), resultado);
        }

        [Fact]
        public async Task Handle_ComNomeInexistente_DeveRetornarNull()
        {
            // Arrange
            var nome = "Serviço Inexistente";

            // Configurar o repositório para retornar null quando chamado com a especificação correta
            _fixture.RepositorioServico
                .ObterUmProjetadoSemRastreamentoAsync<Servico>(Arg.Any<IEspecificacao<ServicoEntityDto>>())
                .Returns((Servico)null);

            var handler = _fixture.CriarObterServicoPorNomeHandler();

            // Act
            var resultado = await handler.Handle(nome);

            // Assert
            resultado.Should().BeNull();

            // Verificar que o repositório foi chamado com a especificação correta
            await _fixture.RepositorioServico.Received(1).ObterUmProjetadoSemRastreamentoAsync<Servico>(Arg.Any<IEspecificacao<ServicoEntityDto>>());

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
                .ObterUmProjetadoSemRastreamentoAsync<Servico>(Arg.Any<IEspecificacao<ServicoEntityDto>>())
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