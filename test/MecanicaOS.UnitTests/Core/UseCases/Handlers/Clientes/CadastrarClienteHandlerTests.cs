using Core.DTOs.UseCases.Cliente;
using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
using Core.UseCases.Clientes.CadastrarCliente;
using FluentAssertions;
using MecanicaOS.UnitTests.Fixtures.Handlers;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MecanicaOS.UnitTests.Core.UseCases.Handlers.Clientes
{
    public class CadastrarClienteHandlerTests
    {
        private readonly ClienteHandlerFixture _fixture;

        public CadastrarClienteHandlerTests()
        {
            _fixture = new ClienteHandlerFixture();
        }

        [Fact]
        public async Task Handle_ComDadosValidos_DeveCadastrarCliente()
        {
            // Arrange
            var dto = ClienteHandlerFixture.CriarCadastrarClientePessoaFisicaDtoValido();
            var clienteId = Guid.NewGuid();
            
            // Configurar o gateway para retornar null ao verificar se o cliente já existe
            _fixture.ClienteGateway.ObterClientePorDocumentoAsync(dto.Documento).Returns((Cliente)null);
            
            // Configurar o gateway para simular o cadastro
            var clienteCadastrado = new Cliente
            {
                Id = clienteId,
                Nome = dto.Nome,
                Documento = dto.Documento,
                DataNascimento = dto.DataNascimento,
                Sexo = dto.Sexo,
                TipoCliente = dto.TipoCliente,
                Ativo = true,
                DataCadastro = DateTime.UtcNow,
                DataAtualizacao = DateTime.UtcNow
            };
            
            _fixture.ConfigurarMockClienteGatewayParaCadastrar(clienteCadastrado);
            
            var handler = _fixture.CriarCadastrarClienteHandler();

            // Act
            var resultado = await handler.Handle(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Cliente.Should().NotBeNull();
            resultado.Cliente.Id.Should().Be(clienteId);
            resultado.Cliente.Nome.Should().Be(dto.Nome);
            resultado.Cliente.Documento.Should().Be(dto.Documento);
            resultado.Cliente.TipoCliente.Should().Be(dto.TipoCliente);
            
            // Verificar que o gateway foi chamado para verificar se o cliente já existe
            await _fixture.ClienteGateway.Received(1).ObterClientePorDocumentoAsync(dto.Documento);
            
            // Verificar que o gateway foi chamado para cadastrar o cliente
            await _fixture.ClienteGateway.Received(1).CadastrarAsync(Arg.Any<Cliente>());
            
            // Verificar que os gateways de endereço e contato foram chamados
            await _fixture.EnderecoGateway.Received(1).CadastrarAsync(Arg.Any<Endereco>());
            await _fixture.ContatoGateway.Received(1).CadastrarAsync(Arg.Any<Contato>());
            
            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), dto);
            _fixture.LogServicoCadastrar.Received(1).LogFim(Arg.Any<string>(), Arg.Any<Cliente>());
        }

        [Fact]
        public async Task Handle_ComDocumentoJaCadastrado_DeveLancarDadosJaCadastradosException()
        {
            // Arrange
            var dto = ClienteHandlerFixture.CriarCadastrarClientePessoaFisicaDtoValido();
            var clienteExistente = ClienteHandlerFixture.CriarClientePessoaFisicaValido();
            clienteExistente.Documento = dto.Documento;
            
            // Configurar o gateway para retornar um cliente existente ao verificar se o cliente já existe
            _fixture.ClienteGateway.ObterClientePorDocumentoAsync(dto.Documento).Returns(clienteExistente);
            
            var handler = _fixture.CriarCadastrarClienteHandler();

            // Act & Assert
            var act = async () => await handler.Handle(dto);
            
            await act.Should().ThrowAsync<DadosJaCadastradosException>()
                .WithMessage("Cliente já cadastrado");
            
            // Verificar que o gateway foi chamado para verificar se o cliente já existe
            await _fixture.ClienteGateway.Received(1).ObterClientePorDocumentoAsync(dto.Documento);
            
            // Verificar que o gateway NÃO foi chamado para cadastrar o cliente
            await _fixture.ClienteGateway.DidNotReceive().CadastrarAsync(Arg.Any<Cliente>());
            
            // Verificar que os gateways de endereço e contato NÃO foram chamados
            await _fixture.EnderecoGateway.DidNotReceive().CadastrarAsync(Arg.Any<Endereco>());
            await _fixture.ContatoGateway.DidNotReceive().CadastrarAsync(Arg.Any<Contato>());
            
            // Verificar que o commit NÃO foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceive().Commit();
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), dto);
            _fixture.LogServicoCadastrar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<DadosJaCadastradosException>());
        }

        [Fact]
        public async Task Handle_QuandoCommitFalha_DeveLancarPersistirDadosException()
        {
            // Arrange
            var dto = ClienteHandlerFixture.CriarCadastrarClientePessoaFisicaDtoValido();
            var clienteId = Guid.NewGuid();
            
            // Configurar o gateway para retornar null ao verificar se o cliente já existe
            _fixture.ClienteGateway.ObterClientePorDocumentoAsync(dto.Documento).Returns((Cliente)null);
            
            // Criar um cliente para retornar do gateway
            var clienteCadastrado = new Cliente
            {
                Id = clienteId,
                Nome = dto.Nome,
                Documento = dto.Documento,
                DataNascimento = dto.DataNascimento,
                Sexo = dto.Sexo,
                TipoCliente = dto.TipoCliente,
                Ativo = true,
                DataCadastro = DateTime.UtcNow,
                DataAtualizacao = DateTime.UtcNow
            };
            
            // Configurar o gateway para retornar o cliente cadastrado
            _fixture.ClienteGateway.CadastrarAsync(Arg.Any<Cliente>()).Returns(Task.FromResult(clienteCadastrado));
            
            // Configurar o UDT para falhar no commit
            _fixture.ConfigurarMockUdtParaCommitFalha();
            
            var handler = _fixture.CriarCadastrarClienteHandler();

            // Act & Assert
            var act = async () => await handler.Handle(dto);
            
            await act.Should().ThrowAsync<PersistirDadosException>()
                .WithMessage("Erro ao cadastrar cliente");
            
            // Verificar que o gateway foi chamado para verificar se o cliente já existe
            await _fixture.ClienteGateway.Received(1).ObterClientePorDocumentoAsync(dto.Documento);
            
            // Verificar que o gateway foi chamado para cadastrar o cliente
            await _fixture.ClienteGateway.Received(1).CadastrarAsync(Arg.Any<Cliente>());
            
            // Verificar que os gateways de endereço e contato foram chamados
            await _fixture.EnderecoGateway.Received(1).CadastrarAsync(Arg.Any<Endereco>());
            await _fixture.ContatoGateway.Received(1).CadastrarAsync(Arg.Any<Contato>());
            
            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), dto);
            _fixture.LogServicoCadastrar.Received(1).LogErro(Arg.Any<string>(), Arg.Any<PersistirDadosException>());
        }

        [Fact]
        public async Task Handle_QuandoGatewayLancaExcecao_DeveRegistrarLogEPropagar()
        {
            // Arrange
            var dto = ClienteHandlerFixture.CriarCadastrarClientePessoaFisicaDtoValido();
            var excecaoEsperada = new Exception("Erro no banco de dados");
            
            // Configurar o gateway para lançar uma exceção
            _fixture.ClienteGateway.When(x => x.ObterClientePorDocumentoAsync(Arg.Any<string>()))
                .Do(x => { throw excecaoEsperada; });
            
            var handler = _fixture.CriarCadastrarClienteHandler();

            // Act & Assert
            var act = async () => await handler.Handle(dto);
            
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Erro no banco de dados");
            
            // Verificar que o gateway foi chamado para verificar se o cliente já existe
            await _fixture.ClienteGateway.Received(1).ObterClientePorDocumentoAsync(dto.Documento);
            
            // Verificar que o gateway NÃO foi chamado para cadastrar o cliente
            await _fixture.ClienteGateway.DidNotReceive().CadastrarAsync(Arg.Any<Cliente>());
            
            // Verificar que os gateways de endereço e contato NÃO foram chamados
            await _fixture.EnderecoGateway.DidNotReceive().CadastrarAsync(Arg.Any<Endereco>());
            await _fixture.ContatoGateway.DidNotReceive().CadastrarAsync(Arg.Any<Contato>());
            
            // Verificar que o commit NÃO foi chamado
            await _fixture.UnidadeDeTrabalho.DidNotReceive().Commit();
            
            // Verificar que os logs foram registrados
            _fixture.LogServicoCadastrar.Received(1).LogInicio(Arg.Any<string>(), dto);
            _fixture.LogServicoCadastrar.Received(1).LogErro(Arg.Any<string>(), excecaoEsperada);
        }

        [Fact]
        public async Task Handle_DevePassarDadosCorretamenteEntreHandlerERepositorio()
        {
            // Arrange
            var dto = new CadastrarClienteUseCaseDto
            {
                Nome = "Cliente Específico de Teste",
                Documento = "111.222.333-44",
                DataNascimento = "01/01/1990",
                Sexo = "M",
                TipoCliente = TipoCliente.PessoaFisica,
                Email = "teste.especifico@example.com",
                Telefone = "(11) 91234-5678",
                Rua = "Rua de Teste",
                Numero = "123",
                Complemento = "Apto 42",
                Bairro = "Bairro Teste",
                Cidade = "Cidade Teste",
                CEP = "12345-678"
            };
            
            var clienteId = Guid.NewGuid();
            
            // Configurar o gateway para retornar null ao verificar se o cliente já existe
            _fixture.ClienteGateway.ObterClientePorDocumentoAsync(dto.Documento).Returns((Cliente)null);
            
            // Criar um cliente para retornar do gateway
            var clienteCadastrado = new Cliente
            {
                Id = clienteId,
                Nome = dto.Nome,
                Documento = dto.Documento,
                DataNascimento = dto.DataNascimento,
                Sexo = dto.Sexo,
                TipoCliente = dto.TipoCliente,
                Ativo = true,
                DataCadastro = DateTime.UtcNow,
                DataAtualizacao = DateTime.UtcNow
            };
            
            // Configurar o gateway para retornar o cliente cadastrado
            _fixture.ConfigurarMockClienteGatewayParaCadastrar(clienteCadastrado);
            
            // Capturar o cliente que está sendo passado para o gateway
            Cliente clientePassado = null;
            _fixture.ClienteGateway.When(x => x.CadastrarAsync(Arg.Any<Cliente>()))
                .Do(callInfo => clientePassado = callInfo.Arg<Cliente>());
            
            // Configurar os gateways de endereço e contato para capturar os objetos que estão sendo cadastrados
            Endereco enderecoCadastrado = null;
            _fixture.EnderecoGateway.When(x => x.CadastrarAsync(Arg.Any<Endereco>()))
                .Do(callInfo => enderecoCadastrado = callInfo.Arg<Endereco>());
            
            Contato contatoCadastrado = null;
            _fixture.ContatoGateway.When(x => x.CadastrarAsync(Arg.Any<Contato>()))
                .Do(callInfo => contatoCadastrado = callInfo.Arg<Contato>());
            
            var handler = _fixture.CriarCadastrarClienteHandler();

            // Act
            var resultado = await handler.Handle(dto);

            // Assert
            // Verificar que o cliente foi passado com os dados corretos
            clientePassado.Should().NotBeNull();
            clientePassado.Nome.Should().Be(dto.Nome);
            clientePassado.Documento.Should().Be(dto.Documento);
            clientePassado.DataNascimento.Should().Be(dto.DataNascimento);
            clientePassado.Sexo.Should().Be(dto.Sexo);
            clientePassado.TipoCliente.Should().Be(dto.TipoCliente);
            
            // Verificar que o endereço foi cadastrado com os dados corretos
            enderecoCadastrado.Should().NotBeNull();
            enderecoCadastrado.Rua.Should().Be(dto.Rua);
            enderecoCadastrado.Numero.Should().Be(dto.Numero);
            enderecoCadastrado.Complemento.Should().Be(dto.Complemento);
            enderecoCadastrado.Bairro.Should().Be(dto.Bairro);
            enderecoCadastrado.Cidade.Should().Be(dto.Cidade);
            enderecoCadastrado.CEP.Should().Be(dto.CEP);
            enderecoCadastrado.IdCliente.Should().Be(clienteId);
            
            // Verificar que o contato foi cadastrado com os dados corretos
            contatoCadastrado.Should().NotBeNull();
            contatoCadastrado.Email.Should().Be(dto.Email);
            contatoCadastrado.Telefone.Should().Be(dto.Telefone);
            contatoCadastrado.IdCliente.Should().Be(clienteId);
            
            // Verificar que o gateway foi chamado para verificar se o cliente já existe
            await _fixture.ClienteGateway.Received(1).ObterClientePorDocumentoAsync(dto.Documento);
            
            // Verificar que o gateway foi chamado para cadastrar o cliente
            await _fixture.ClienteGateway.Received(1).CadastrarAsync(Arg.Any<Cliente>());
            
            // Verificar que os gateways de endereço e contato foram chamados
            await _fixture.EnderecoGateway.Received(1).CadastrarAsync(Arg.Any<Endereco>());
            await _fixture.ContatoGateway.Received(1).CadastrarAsync(Arg.Any<Contato>());
            
            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();
            
            // Verificar o resultado
            resultado.Should().NotBeNull();
            resultado.Cliente.Should().NotBeNull();
            resultado.Cliente.Id.Should().Be(clienteId);
        }

        [Theory]
        [InlineData(TipoCliente.PessoaFisica)]
        [InlineData(TipoCliente.PessoaJuridico)]
        public async Task Handle_ComDiferentesTiposCliente_DeveCadastrarCorretamente(TipoCliente tipoCliente)
        {
            // Arrange
            var dto = tipoCliente == TipoCliente.PessoaFisica
                ? ClienteHandlerFixture.CriarCadastrarClientePessoaFisicaDtoValido()
                : ClienteHandlerFixture.CriarCadastrarClientePessoaJuridicaDtoValido();
            
            dto.TipoCliente = tipoCliente;
            var clienteId = Guid.NewGuid();
            
            // Configurar o gateway para retornar null ao verificar se o cliente já existe
            _fixture.ClienteGateway.ObterClientePorDocumentoAsync(dto.Documento).Returns((Cliente)null);
            
            // Criar um cliente para retornar do gateway
            var clienteCadastrado = new Cliente
            {
                Id = clienteId,
                Nome = dto.Nome,
                Documento = dto.Documento,
                DataNascimento = dto.DataNascimento,
                Sexo = dto.Sexo,
                TipoCliente = dto.TipoCliente,
                Ativo = true,
                DataCadastro = DateTime.UtcNow,
                DataAtualizacao = DateTime.UtcNow
            };
            
            // Configurar o gateway para retornar o cliente cadastrado
            _fixture.ClienteGateway.CadastrarAsync(Arg.Any<Cliente>()).Returns(Task.FromResult(clienteCadastrado));
            
            // Capturar o cliente que está sendo passado para o gateway
            Cliente clientePassado = null;
            _fixture.ClienteGateway.When(x => x.CadastrarAsync(Arg.Any<Cliente>()))
                .Do(callInfo => clientePassado = callInfo.Arg<Cliente>());
            
            var handler = _fixture.CriarCadastrarClienteHandler();

            // Act
            var resultado = await handler.Handle(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Cliente.Should().NotBeNull();
            resultado.Cliente.TipoCliente.Should().Be(tipoCliente);
            
            // Verificar que o cliente passado tem o tipo correto
            clientePassado.Should().NotBeNull();
            clientePassado.TipoCliente.Should().Be(tipoCliente);
            
            // Verificar que o gateway foi chamado para verificar se o cliente já existe
            await _fixture.ClienteGateway.Received(1).ObterClientePorDocumentoAsync(dto.Documento);
            
            // Verificar que o gateway foi chamado para cadastrar o cliente
            await _fixture.ClienteGateway.Received(1).CadastrarAsync(Arg.Is<Cliente>(c => c.TipoCliente == tipoCliente));
            
            // Verificar que o commit foi chamado
            await _fixture.UnidadeDeTrabalho.Received(1).Commit();
        }
    }
}
