using Core.DTOs.Entidades.Cliente;
using Core.Entidades;
using Core.Enumeradores;
using Core.Especificacoes.Base.Interfaces;
using Core.Exceptions;
using Core.UseCases.Clientes.ObterClientePorDocumento;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.Handlers;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.Clientes
{
    public class ObterClientePorDocumentoHandlerTests
    {
        private readonly ClienteHandlerFixture _fixture;

        public ObterClientePorDocumentoHandlerTests()
        {
            _fixture = new ClienteHandlerFixture();
        }

        [Fact]
        public async Task Handle_ComDocumentoExistente_DeveRetornarClienteCorreto()
        {
            // Arrange
            var documento = "123.456.789-00";
            var clienteEsperado = ClienteHandlerFixture.CriarClientePessoaFisicaValido();
            clienteEsperado.Documento = documento;
            
            // Configurar o gateway para retornar o cliente esperado
            _fixture.ConfigurarMockClienteGatewayParaObterPorDocumento(documento, clienteEsperado);
            
            var handler = _fixture.CriarObterClientePorDocumentoHandler();

            // Act
            var resultado = await handler.Handle(documento);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Cliente.Should().NotBeNull();
            resultado.Cliente.Should().Be(clienteEsperado);
            
            // Verificar que o gateway foi chamado com o documento correto
            await _fixture.RepositorioCliente.Received(1).ObterUmProjetadoSemRastreamentoAsync<Cliente>(Arg.Any<IEspecificacao<ClienteEntityDto>>());
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoObterPorDocumento.Received(1).LogInicio(Arg.Any<string>(), documento);
            _fixture.LogServicoObterPorDocumento.Received(1).LogFim(Arg.Any<string>(), clienteEsperado);
        }

        [Fact]
        public async Task Handle_ComDocumentoInexistente_DeveLancarDadosNaoEncontradosException()
        {
            // Arrange
            var documento = "999.999.999-99";
            
            // Configurar o gateway para retornar null (cliente não encontrado)
            _fixture.ClienteGateway.ObterClientePorDocumentoAsync(documento).Returns((Cliente)null);
            
            var handler = _fixture.CriarObterClientePorDocumentoHandler();

            // Act & Assert
            var act = async () => await handler.Handle(documento);
            
            await act.Should().ThrowAsync<DadosNaoEncontradosException>()
                .WithMessage($"Cliente de documento {documento} não encontrado");
            
            // Verificar que o gateway foi chamado com o documento correto
            await _fixture.RepositorioCliente.Received(1).ObterUmProjetadoSemRastreamentoAsync<Cliente>(Arg.Any<IEspecificacao<ClienteEntityDto>>());
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoObterPorDocumento.Received(1).LogInicio(Arg.Any<string>(), documento);
            _fixture.LogServicoObterPorDocumento.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosNaoEncontradosException>());
        }

        [Fact]
        public async Task Handle_ComDocumentoInvalido_DeveLancarDadosInvalidosException()
        {
            // Arrange
            var documento = "";
            
            var handler = _fixture.CriarObterClientePorDocumentoHandler();

            // Act & Assert
            var act = async () => await handler.Handle(documento);
            
            await act.Should().ThrowAsync<DadosInvalidosException>()
                .WithMessage("Deve ser informado o documento do usuario do cliente");
            
            // Verificar que o gateway NÃO foi chamado
            await _fixture.RepositorioCliente.DidNotReceive().ObterUmProjetadoSemRastreamentoAsync<Cliente>(Arg.Any<IEspecificacao<ClienteEntityDto>>());
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoObterPorDocumento.Received(1).LogInicio(Arg.Any<string>(), documento);
            _fixture.LogServicoObterPorDocumento.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosInvalidosException>());
        }

        [Fact]
        public async Task Handle_QuandoGatewayLancaExcecao_DeveRegistrarLogEPropagar()
        {
            // Arrange
            var documento = "123.456.789-00";
            var excecaoEsperada = new Exception("Erro no banco de dados");
            
            // Configurar o repositório para lançar uma exceção
            _fixture.RepositorioCliente
                .ObterUmProjetadoSemRastreamentoAsync<Cliente>(Arg.Any<IEspecificacao<ClienteEntityDto>>())
                .Returns(Task.FromException<Cliente>(excecaoEsperada));
            
            var handler = _fixture.CriarObterClientePorDocumentoHandler();

            // Act & Assert
            var act = async () => await handler.Handle(documento);
            
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Erro no banco de dados");
            
            // Verificar que o gateway foi chamado com o documento correto
            await _fixture.RepositorioCliente.Received(1).ObterUmProjetadoSemRastreamentoAsync<Cliente>(Arg.Any<IEspecificacao<ClienteEntityDto>>());
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoObterPorDocumento.Received(1).LogInicio(Arg.Any<string>(), documento);
            _fixture.LogServicoObterPorDocumento.Received(1).LogErro(Arg.Any<string>(), excecaoEsperada);
        }

        [Fact]
        public async Task Handle_DevePassarDadosCorretamenteEntreHandlerERepositorio()
        {
            // Arrange
            var documento = "111.222.333-44";
            
            // Criar um cliente com valores específicos para identificar no teste
            var clienteEsperado = new Cliente
            {
                Id = Guid.NewGuid(),
                Nome = "Cliente Específico de Teste",
                Documento = documento,
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
            _fixture.ConfigurarMockClienteGatewayParaObterPorDocumento(documento, clienteEsperado);
            
            var handler = _fixture.CriarObterClientePorDocumentoHandler();

            // Act
            var resultado = await handler.Handle(documento);

            // Assert
            // Verificar que o gateway foi chamado com o documento correto
            await _fixture.RepositorioCliente.Received(1).ObterUmProjetadoSemRastreamentoAsync<Cliente>(Arg.Any<IEspecificacao<ClienteEntityDto>>());
            
            // Verificar que o resultado contém exatamente os mesmos dados retornados pelo gateway
            resultado.Should().NotBeNull();
            resultado.Cliente.Should().NotBeNull();
            resultado.Cliente.Should().BeSameAs(clienteEsperado);
            
            // Verificar cada propriedade individualmente para garantir que não houve alteração
            resultado.Cliente.Id.Should().Be(clienteEsperado.Id);
            resultado.Cliente.Nome.Should().Be("Cliente Específico de Teste");
            resultado.Cliente.Documento.Should().Be(documento);
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
