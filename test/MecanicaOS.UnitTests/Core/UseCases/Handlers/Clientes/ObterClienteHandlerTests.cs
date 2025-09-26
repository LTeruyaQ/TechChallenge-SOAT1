using Core.DTOs.Entidades.Cliente;
using Core.Entidades;
using Core.Enumeradores;
using Core.Especificacoes.Base.Interfaces;
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
            
            // Configurar o repositório para retornar o cliente esperado
            _fixture.ConfigurarMockRepositorioClienteParaObterPorId(clienteId, clienteEsperado);
            
            var handler = _fixture.CriarObterClienteHandler();

            // Act
            var resultado = await handler.Handle(clienteId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(clienteEsperado);
            
            // Verificar que o repositório foi chamado com o ID correto
            await _fixture.RepositorioCliente.Received(1).ObterPorIdAsync(clienteId);
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoObter.Received(1).LogInicio(Arg.Any<string>(), clienteId);
            _fixture.LogServicoObter.Received(1).LogFim(Arg.Any<string>(), clienteEsperado);
        }

        [Fact]
        public async Task Handle_ComIdInexistente_DeveRetornarNull()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            
            // Configurar o repositório para retornar null
            _fixture.RepositorioCliente.ObterPorIdAsync(clienteId).Returns(Task.FromResult<ClienteEntityDto>(null));
            
            var handler = _fixture.CriarObterClienteHandler();

            // Act
            var resultado = await handler.Handle(clienteId);

            // Assert
            resultado.Should().BeNull();
            
            // Verificar que o repositório foi chamado com o ID correto
            await _fixture.RepositorioCliente.Received(1).ObterPorIdAsync(clienteId);
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoObter.Received(1).LogInicio(Arg.Any<string>(), clienteId);
            _fixture.LogServicoObter.Received(1).LogFim(Arg.Any<string>(), Arg.Any<object>());
        }

        [Fact]
        public async Task Handle_QuandoRepositorioLancaExcecao_DeveRegistrarLogEPropagar()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var excecaoEsperada = new Exception("Erro no banco de dados");
            
            // Configurar o repositório para lançar uma exceção
            _fixture.RepositorioCliente.ObterPorIdAsync(Arg.Any<Guid>())
                .Returns<ClienteEntityDto>(x => { throw excecaoEsperada; });
            
            var handler = _fixture.CriarObterClienteHandler();

            // Act & Assert
            var act = async () => await handler.Handle(clienteId);
            
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Erro no banco de dados");
            
            // Verificar que o repositório foi chamado com o ID correto
            await _fixture.RepositorioCliente.Received(1).ObterPorIdAsync(clienteId);
            
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
            
            // Configurar o repositório para retornar o cliente específico
            _fixture.ConfigurarMockRepositorioClienteParaObterPorId(clienteId, clienteEsperado);
            
            var handler = _fixture.CriarObterClienteHandler();

            // Act
            var resultado = await handler.Handle(clienteId);

            // Assert
            // Verificar que o repositório foi chamado com o ID correto
            await _fixture.RepositorioCliente.Received(1).ObterPorIdAsync(clienteId);
            
            // Verificar que o resultado contém exatamente os mesmos dados retornados pelo repositório
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(clienteEsperado);
            
            // Verificar cada propriedade individualmente para garantir que não houve alteração
            resultado.Id.Should().Be(clienteId);
            resultado.Nome.Should().Be("Cliente Específico de Teste");
            resultado.Documento.Should().Be("111.222.333-44");
            resultado.TipoCliente.Should().Be(TipoCliente.PessoaFisica);
            resultado.DataNascimento.Should().Be("01/01/1990");
            resultado.Sexo.Should().Be("M");
            
            // Verificar propriedades de contato
            resultado.Contato.Email.Should().Be("teste.especifico@example.com");
            resultado.Contato.Telefone.Should().Be("(11) 91234-5678");
            
            // Verificar propriedades de endereço
            resultado.Endereco.Rua.Should().Be("Rua de Teste");
            resultado.Endereco.Numero.Should().Be("123");
            resultado.Endereco.Complemento.Should().Be("Apto 42");
            resultado.Endereco.Bairro.Should().Be("Bairro Teste");
            resultado.Endereco.Cidade.Should().Be("Cidade Teste");
            resultado.Endereco.CEP.Should().Be("12345-678");
            
            // Verificar que os campos técnicos foram preservados
            resultado.Ativo.Should().BeTrue();
            resultado.DataCadastro.Should().Be(new DateTime(2023, 1, 15));
            resultado.DataAtualizacao.Should().Be(new DateTime(2023, 6, 30));
        }
    }
}
