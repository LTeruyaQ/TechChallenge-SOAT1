using Core.Entidades;
using Core.Enumeradores;
using Core.UseCases.Clientes.ObterTodosClientes;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.Handlers;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

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
            var clientes = ClienteHandlerFixture.CriarListaClientesVariados();
            
            // Configurar o gateway para retornar a lista de clientes
            _fixture.ConfigurarMockClienteGatewayParaObterTodos(clientes);
            
            var handler = _fixture.CriarObterTodosClientesHandler();

            // Act
            var resultado = await handler.Handle();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Clientes.Should().NotBeNull();
            resultado.Clientes.Should().HaveCount(clientes.Count);
            
            // Verificar que o gateway foi chamado
            await _fixture.ClienteGateway.Received(1).ObterTodosClienteComVeiculoAsync();
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoObterTodos.Received(1).LogInicio(Arg.Any<string>());
            _fixture.LogServicoObterTodos.Received(1).LogFim(Arg.Any<string>(), Arg.Any<IEnumerable<Cliente>>());
        }

        [Fact]
        public async Task Handle_QuandoNaoHaClientes_DeveRetornarListaVazia()
        {
            // Arrange
            var clientes = new List<Cliente>();
            
            // Configurar o gateway para retornar uma lista vazia
            _fixture.ConfigurarMockClienteGatewayParaObterTodos(clientes);
            
            var handler = _fixture.CriarObterTodosClientesHandler();

            // Act
            var resultado = await handler.Handle();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Clientes.Should().NotBeNull();
            resultado.Clientes.Should().BeEmpty();
            
            // Verificar que o gateway foi chamado
            await _fixture.ClienteGateway.Received(1).ObterTodosClienteComVeiculoAsync();
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoObterTodos.Received(1).LogInicio(Arg.Any<string>());
            _fixture.LogServicoObterTodos.Received(1).LogFim(Arg.Any<string>(), Arg.Any<IEnumerable<Cliente>>());
        }

        [Fact]
        public async Task Handle_QuandoGatewayLancaExcecao_DeveRegistrarLogEPropagar()
        {
            // Arrange
            var excecaoEsperada = new Exception("Erro no banco de dados");
            
            // Configurar o gateway para lançar uma exceção
            _fixture.ClienteGateway.When(x => x.ObterTodosClienteComVeiculoAsync())
                .Do(x => { throw excecaoEsperada; });
            
            var handler = _fixture.CriarObterTodosClientesHandler();

            // Act & Assert
            var act = async () => await handler.Handle();
            
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Erro no banco de dados");
            
            // Verificar que o gateway foi chamado
            await _fixture.ClienteGateway.Received(1).ObterTodosClienteComVeiculoAsync();
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoObterTodos.Received(1).LogInicio(Arg.Any<string>());
            _fixture.LogServicoObterTodos.Received(1).LogErro(Arg.Any<string>(), excecaoEsperada);
        }

        [Fact]
        public async Task Handle_DevePassarDadosCorretamenteEntreHandlerERepositorio()
        {
            // Arrange
            var clientes = new List<Cliente>
            {
                new Cliente
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
                        Rua = "Rua Teste 1",
                        Numero = "100",
                        Complemento = "Apto 10",
                        Bairro = "Bairro Teste 1",
                        Cidade = "Cidade Teste 1",
                        CEP = "11111-111"
                    },
                    Ativo = true,
                    DataCadastro = new DateTime(2023, 1, 15),
                    DataAtualizacao = new DateTime(2023, 6, 30)
                },
                new Cliente
                {
                    Id = Guid.NewGuid(),
                    Nome = "Cliente Específico 2",
                    Documento = "22.333.444/0001-55",
                    DataNascimento = "01/01/2000",
                    TipoCliente = TipoCliente.PessoaJuridico,
                    Contato = new Contato
                    {
                        Email = "cliente2@example.com",
                        Telefone = "(11) 92345-6789"
                    },
                    Endereco = new Endereco
                    {
                        Rua = "Rua Teste 2",
                        Numero = "200",
                        Complemento = "Sala 20",
                        Bairro = "Bairro Teste 2",
                        Cidade = "Cidade Teste 2",
                        CEP = "22222-222"
                    },
                    Ativo = true,
                    DataCadastro = new DateTime(2023, 2, 15),
                    DataAtualizacao = new DateTime(2023, 7, 30)
                }
            };
            
            // Configurar o gateway para retornar a lista de clientes específica
            _fixture.ConfigurarMockClienteGatewayParaObterTodos(clientes);
            
            var handler = _fixture.CriarObterTodosClientesHandler();

            // Act
            var resultado = await handler.Handle();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Clientes.Should().NotBeNull();
            resultado.Clientes.Should().HaveCount(2);
            
            // Verificar os dados do primeiro cliente
            var primeiroCliente = resultado.Clientes.First();
            primeiroCliente.Nome.Should().Be("Cliente Específico 1");
            primeiroCliente.Documento.Should().Be("111.222.333-44");
            primeiroCliente.TipoCliente.Should().Be(TipoCliente.PessoaFisica);
            primeiroCliente.Contato.Email.Should().Be("cliente1@example.com");
            primeiroCliente.Endereco.Rua.Should().Be("Rua Teste 1");
            
            // Verificar os dados do segundo cliente
            var segundoCliente = resultado.Clientes.Skip(1).First();
            segundoCliente.Nome.Should().Be("Cliente Específico 2");
            segundoCliente.Documento.Should().Be("22.333.444/0001-55");
            segundoCliente.TipoCliente.Should().Be(TipoCliente.PessoaJuridico);
            segundoCliente.Contato.Email.Should().Be("cliente2@example.com");
            segundoCliente.Endereco.Rua.Should().Be("Rua Teste 2");
            
            // Verificar que o gateway foi chamado
            await _fixture.ClienteGateway.Received(1).ObterTodosClienteComVeiculoAsync();
        }
    }
}
