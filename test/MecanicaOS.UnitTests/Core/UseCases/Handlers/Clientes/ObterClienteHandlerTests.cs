using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
using Core.UseCases.Clientes.ObterCliente;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.Handlers;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.Clientes
{
    public class ObterClienteHandlerTests
    {
        private readonly ClienteHandlerFixture _fixture;

        public ObterClienteHandlerTests()
        {
            _fixture = new ClienteHandlerFixture();
        }

        [Fact]
        public async Task Handle_ComIdExistente_DeveRetornarClienteCorreto()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var clienteEsperado = ClienteHandlerFixture.CriarClientePessoaFisicaValido();
            clienteEsperado.Id = clienteId;
            
            // Configurar o gateway para retornar o cliente esperado
            _fixture.ConfigurarMockClienteGatewayParaObterPorId(clienteId, clienteEsperado);
            
            var handler = _fixture.CriarObterClienteHandler();

            // Act
            var resultado = await handler.Handle(clienteId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Cliente.Should().NotBeNull();
            resultado.Cliente.Should().Be(clienteEsperado);
            
            // Verificar que o gateway foi chamado com o ID correto
            await _fixture.ClienteGateway.Received(1).ObterPorIdAsync(clienteId);
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoObter.Received(1).LogInicio(Arg.Any<string>(), clienteId);
            _fixture.LogServicoObter.Received(1).LogFim(Arg.Any<string>(), clienteEsperado);
        }

        [Fact]
        public async Task Handle_ComIdInexistente_DeveRetornarNull()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            
            // Configurar o gateway para retornar null
            _fixture.ClienteGateway.ObterPorIdAsync(clienteId).Returns((Cliente)null);
            
            var handler = _fixture.CriarObterClienteHandler();

            // Act
            var resultado = await handler.Handle(clienteId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Cliente.Should().BeNull();
            
            // Verificar que o gateway foi chamado com o ID correto
            await _fixture.ClienteGateway.Received(1).ObterPorIdAsync(clienteId);
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoObter.Received(1).LogInicio(Arg.Any<string>(), clienteId);
            _fixture.LogServicoObter.Received(1).LogFim(Arg.Any<string>(), Arg.Any<object>());
        }

        [Fact]
        public async Task Handle_QuandoGatewayLancaExcecao_DeveRegistrarLogEPropagar()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var excecaoEsperada = new Exception("Erro no banco de dados");
            
            // Configurar o gateway para lançar uma exceção
            _fixture.ClienteGateway.When(x => x.ObterPorIdAsync(Arg.Any<Guid>()))
                .Do(x => { throw excecaoEsperada; });
            
            var handler = _fixture.CriarObterClienteHandler();

            // Act & Assert
            var act = async () => await handler.Handle(clienteId);
            
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Erro no banco de dados");
            
            // Verificar que o gateway foi chamado com o ID correto
            await _fixture.ClienteGateway.Received(1).ObterPorIdAsync(clienteId);
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoObter.Received(1).LogInicio(Arg.Any<string>(), clienteId);
            _fixture.LogServicoObter.Received(1).LogErro(Arg.Any<string>(), excecaoEsperada);
        }

        [Fact]
        public async Task Handle_DevePassarDadosCorretamenteEntreHandlerERepositorio()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            
            // Criar um cliente com valores específicos para identificar no teste
            var clienteEsperado = new Cliente
            {
                Id = clienteId,
                Nome = "Cliente Específico de Teste",
                Documento = "111.222.333-44",
                DataNascimento = "01/01/1990",
                Sexo = "M",
                TipoCliente = TipoCliente.PessoaFisica,
                Contato = new Contato
                {
                    Email = "teste.especifico@example.com",
                    Telefone = "(11) 91234-5678"
                },
                Endereco = new Endereco
                {
                    Rua = "Rua de Teste",
                    Numero = "123",
                    Complemento = "Apto 42",
                    Bairro = "Bairro Teste",
                    Cidade = "Cidade Teste",
                    CEP = "12345-678"
                },
                Ativo = true,
                DataCadastro = new DateTime(2023, 1, 15),
                DataAtualizacao = new DateTime(2023, 6, 30)
            };
            
            // Configurar o gateway para retornar o cliente específico
            _fixture.ConfigurarMockClienteGatewayParaObterPorId(clienteId, clienteEsperado);
            
            var handler = _fixture.CriarObterClienteHandler();

            // Act
            var resultado = await handler.Handle(clienteId);

            // Assert
            // Verificar que o gateway foi chamado com o ID correto
            await _fixture.ClienteGateway.Received(1).ObterPorIdAsync(clienteId);
            
            // Verificar que o resultado contém exatamente os mesmos dados retornados pelo gateway
            resultado.Should().NotBeNull();
            resultado.Cliente.Should().NotBeNull();
            resultado.Cliente.Should().BeSameAs(clienteEsperado);
            
            // Verificar cada propriedade individualmente para garantir que não houve alteração
            resultado.Cliente.Id.Should().Be(clienteId);
            resultado.Cliente.Nome.Should().Be("Cliente Específico de Teste");
            resultado.Cliente.Documento.Should().Be("111.222.333-44");
            resultado.Cliente.TipoCliente.Should().Be(TipoCliente.PessoaFisica);
            resultado.Cliente.DataNascimento.Should().Be("01/01/1990");
            resultado.Cliente.Sexo.Should().Be("M");
            
            // Verificar propriedades de contato
            resultado.Cliente.Contato.Email.Should().Be("teste.especifico@example.com");
            resultado.Cliente.Contato.Telefone.Should().Be("(11) 91234-5678");
            
            // Verificar propriedades de endereço
            resultado.Cliente.Endereco.Rua.Should().Be("Rua de Teste");
            resultado.Cliente.Endereco.Numero.Should().Be("123");
            resultado.Cliente.Endereco.Complemento.Should().Be("Apto 42");
            resultado.Cliente.Endereco.Bairro.Should().Be("Bairro Teste");
            resultado.Cliente.Endereco.Cidade.Should().Be("Cidade Teste");
            resultado.Cliente.Endereco.CEP.Should().Be("12345-678");
            
            // Verificar que os campos técnicos foram preservados
            resultado.Cliente.Ativo.Should().BeTrue();
            resultado.Cliente.DataCadastro.Should().Be(new DateTime(2023, 1, 15));
            resultado.Cliente.DataAtualizacao.Should().Be(new DateTime(2023, 6, 30));
        }
    }
}
