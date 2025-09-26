using Core.DTOs.Entidades.Cliente;
using Core.Entidades;
using Core.Especificacoes.Base.Interfaces;
using MecanicaOS.UnitTests.Fixtures.Handlers;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.Clientes
{
    public class ObterTodosClientesHandlerTests
    {
        private readonly ClienteHandlerFixture _fixture;

        public ObterTodosClientesHandlerTests()
        {
            _fixture = new ClienteHandlerFixture();
        }

        [Fact]
        public async Task Handle_DeveRetornarTodosClientes()
        {
            // Arrange
            var clientes = new List<Cliente>
            {
                ClienteHandlerFixture.CriarClientePessoaFisicaValido(),
                ClienteHandlerFixture.CriarClientePessoaJuridicaValido()
            };

            // Configurar o repositório para retornar a lista de clientes
            _fixture.ConfigurarMockRepositorioClienteParaObterTodos(clientes);

            var handler = _fixture.CriarObterTodosClientesHandler();

            // Act
            var resultado = await handler.Handle();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.Should().BeEquivalentTo(clientes);

            // Verificar que o repositório foi chamado
            await _fixture.RepositorioCliente.Received(1).ListarProjetadoAsync<Cliente>(Arg.Any<IEspecificacao<ClienteEntityDto>>());

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterTodos.Received(1).LogInicio(Arg.Any<string>());
            _fixture.LogServicoObterTodos.Received(1).LogFim(Arg.Any<string>(), Arg.Any<object>());
        }

        [Fact]
        public async Task Handle_QuandoNaoHaClientes_DeveRetornarListaVazia()
        {
            // Arrange
            var listaVazia = new List<Cliente>();

            // Configurar o repositório para retornar uma lista vazia
            _fixture.ConfigurarMockRepositorioClienteParaObterTodos(listaVazia);

            var handler = _fixture.CriarObterTodosClientesHandler();

            // Act
            var resultado = await handler.Handle();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();

            // Verificar que o repositório foi chamado
            await _fixture.RepositorioCliente.Received(1).ListarProjetadoAsync<Cliente>(Arg.Any<IEspecificacao<ClienteEntityDto>>());

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterTodos.Received(1).LogInicio(Arg.Any<string>());
            _fixture.LogServicoObterTodos.Received(1).LogFim(Arg.Any<string>(), Arg.Any<object>());
        }

        [Fact]
        public async Task Handle_QuandoRepositorioLancaExcecao_DeveRegistrarLogEPropagar()
        {
            // Arrange
            var excecaoEsperada = new Exception("Erro no banco de dados");

            // Configurar o repositório para lançar uma exceção
            _fixture.RepositorioCliente.ListarProjetadoAsync<Cliente>(Arg.Any<IEspecificacao<ClienteEntityDto>>())
                .Returns(Task.FromException<IEnumerable<Cliente>>(excecaoEsperada));

            var handler = _fixture.CriarObterTodosClientesHandler();

            // Act & Assert
            var act = async () => await handler.Handle();

            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Erro no banco de dados");

            // Verificar que o repositório foi chamado
            await _fixture.RepositorioCliente.Received(1).ListarProjetadoAsync<Cliente>(Arg.Any<IEspecificacao<ClienteEntityDto>>());

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterTodos.Received(1).LogInicio(Arg.Any<string>());
            _fixture.LogServicoObterTodos.Received(1).LogErro(Arg.Any<string>(), excecaoEsperada);
        }

        [Fact]
        public async Task Handle_DevePassarDadosCorretamenteEntreHandlerERepositorio()
        {
            // Arrange
            var cliente1 = ClienteHandlerFixture.CriarClientePessoaFisicaValido();
            cliente1.Nome = "Cliente Específico 1";

            var cliente2 = ClienteHandlerFixture.CriarClientePessoaJuridicaValido();
            cliente2.Nome = "Cliente Específico 2";

            var clientes = new List<Cliente> { cliente1, cliente2 };

            // Configurar o repositório para retornar a lista específica
            _fixture.ConfigurarMockRepositorioClienteParaObterTodos(clientes);

            var handler = _fixture.CriarObterTodosClientesHandler();

            // Act
            var resultado = await handler.Handle();

            // Assert
            // Verificar que o repositório foi chamado
            await _fixture.RepositorioCliente.Received(1).ListarProjetadoAsync<Cliente>(Arg.Any<IEspecificacao<ClienteEntityDto>>());

            // Verificar que o resultado contém exatamente os mesmos dados retornados pelo gateway
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.Should().BeEquivalentTo(clientes);

            // Verificar que os nomes específicos estão presentes
            resultado.Should().Contain(c => c.Nome == "Cliente Específico 1");
            resultado.Should().Contain(c => c.Nome == "Cliente Específico 2");
        }
    }
}
