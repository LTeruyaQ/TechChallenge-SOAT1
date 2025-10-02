using Core.DTOs.Entidades.Cliente;
using Core.Entidades;
using Core.Enumeradores;
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
            var cliente1 = new Cliente
            {
                Id = Guid.NewGuid(),
                Nome = "Cliente Específico 1",
                Documento = "111.222.333-44",
                DataNascimento = "01/01/1990",
                Sexo = "M",
                TipoCliente = TipoCliente.PessoaFisica,
                Contato = new Contato
                {
                    Email = "cliente1@example.com",
                    Telefone = "(11) 91234-5678"
                },
                Endereco = new Endereco
                {
                    Rua = "Rua Cliente 1",
                    Numero = "123",
                    Complemento = "Apto 42",
                    Bairro = "Bairro Cliente 1",
                    Cidade = "Cidade Cliente 1",
                    CEP = "12345-678"
                },
                Ativo = true,
                DataCadastro = new DateTime(2023, 1, 15),
                DataAtualizacao = new DateTime(2023, 6, 30)
            };

            var cliente2 = new Cliente
            {
                Id = Guid.NewGuid(),
                Nome = "Cliente Específico 2",
                Documento = "98.765.432/0001-10",
                DataNascimento = "01/01/2000",
                Sexo = null,
                TipoCliente = TipoCliente.PessoaJuridico,
                Contato = new Contato
                {
                    Email = "cliente2@example.com",
                    Telefone = "(11) 5555-5555"
                },
                Endereco = new Endereco
                {
                    Rua = "Rua Cliente 2",
                    Numero = "456",
                    Complemento = "Sala 10",
                    Bairro = "Bairro Cliente 2",
                    Cidade = "Cidade Cliente 2",
                    CEP = "54321-876"
                },
                Ativo = true,
                DataCadastro = new DateTime(2023, 2, 20),
                DataAtualizacao = new DateTime(2023, 7, 15)
            };

            var clientes = new List<Cliente> { cliente1, cliente2 };

            // Configurar o repositório para retornar a lista específica
            _fixture.ConfigurarMockRepositorioClienteParaObterTodos(clientes);

            var handler = _fixture.CriarObterTodosClientesHandler();

            // Act
            var resultado = await handler.Handle();

            // Assert
            // Verificar que o repositório foi chamado
            await _fixture.RepositorioCliente.Received(1).ListarProjetadoAsync<Cliente>(Arg.Any<IEspecificacao<ClienteEntityDto>>());

            // Verificar que o resultado contém exatamente os mesmos dados retornados pelo repositório
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.Should().BeEquivalentTo(clientes);

            // Verificar que os clientes específicos estão presentes
            var clienteRetornado1 = resultado.FirstOrDefault(c => c.Nome == "Cliente Específico 1");
            clienteRetornado1.Should().NotBeNull();
            clienteRetornado1.Documento.Should().Be("111.222.333-44");
            clienteRetornado1.TipoCliente.Should().Be(TipoCliente.PessoaFisica);
            clienteRetornado1.Contato.Email.Should().Be("cliente1@example.com");
            clienteRetornado1.Endereco.Cidade.Should().Be("Cidade Cliente 1");
            clienteRetornado1.DataCadastro.Should().Be(new DateTime(2023, 1, 15));
            clienteRetornado1.DataAtualizacao.Should().Be(new DateTime(2023, 6, 30));

            var clienteRetornado2 = resultado.FirstOrDefault(c => c.Nome == "Cliente Específico 2");
            clienteRetornado2.Should().NotBeNull();
            clienteRetornado2.Documento.Should().Be("98.765.432/0001-10");
            clienteRetornado2.TipoCliente.Should().Be(TipoCliente.PessoaJuridico);
            clienteRetornado2.Contato.Email.Should().Be("cliente2@example.com");
            clienteRetornado2.Endereco.Cidade.Should().Be("Cidade Cliente 2");
            clienteRetornado2.DataCadastro.Should().Be(new DateTime(2023, 2, 20));
            clienteRetornado2.DataAtualizacao.Should().Be(new DateTime(2023, 7, 15));

            // Verificar que os logs foram registrados
            _fixture.LogServicoObterTodos.Received(1).LogInicio(Arg.Any<string>());
            _fixture.LogServicoObterTodos.Received(1).LogFim(Arg.Any<string>(), Arg.Any<IEnumerable<Cliente>>());
        }
    }
}
