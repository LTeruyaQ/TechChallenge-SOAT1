using Aplicacao.DTOs.Requests.Cliente;
using Aplicacao.DTOs.Responses.Cliente;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using FluentAssertions;
using MecanicaOSTests.Fixtures;
using Moq;
using Xunit;

namespace Aplicacao.Servicos.Tests
{
    public class ClienteServicoTests
    {
        private readonly Mock<IRepositorio<Cliente>> _clienteRepoMock;
        private readonly Mock<IRepositorio<Endereco>> _enderecoRepoMock;
        private readonly Mock<IRepositorio<Contato>> _contatoRepoMock;
        private readonly Mock<ILogServico<ClienteServico>> _logMock;
        private readonly Mock<IUnidadeDeTrabalho> _uotMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ClienteServico _clienteServico;

        public ClienteServicoTests()
        {
            _clienteRepoMock = new Mock<IRepositorio<Cliente>>();
            _enderecoRepoMock = new Mock<IRepositorio<Endereco>>();
            _contatoRepoMock = new Mock<IRepositorio<Contato>>();
            _logMock = new Mock<ILogServico<ClienteServico>>();
            _uotMock = new Mock<IUnidadeDeTrabalho>();
            _mapperMock = new Mock<IMapper>();

            _clienteServico = new ClienteServico(
                _clienteRepoMock.Object,
                _enderecoRepoMock.Object,
                _contatoRepoMock.Object,
                _logMock.Object,
                _uotMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Dado_RequestValido_Quando_CadastrarAsync_Entao_RetornaClienteResponse()
        {
            // Arrange
            var request = new CadastrarClienteRequest { Nome = "João" };
            var cliente = new Cliente { Id = Guid.NewGuid(), Nome = "João" };
            var response = new ClienteResponse { Id = cliente.Id, Nome = cliente.Nome };

            _mapperMock.Setup(m => m.Map<Cliente>(request)).Returns(cliente);
            _clienteRepoMock.Setup(r => r.CadastrarAsync(cliente)).ReturnsAsync(cliente);
            _mapperMock.Setup(m => m.Map<ClienteResponse>(cliente)).Returns(response);
            _uotMock.Setup(u => u.SalvarAlteracoesAsync()).ReturnsAsync(true);

            // Act
            var resultado = await _clienteServico.CadastrarAsync(request);

            // Assert
            resultado.Should().NotBeNull("porque o cadastro deve retornar o cliente cadastrado");
            resultado.Id.Should().Be(cliente.Id, "porque o ID deve ser o mesmo do cliente cadastrado");
            resultado.Nome.Should().Be(cliente.Nome, "porque o nome deve ser o mesmo do cliente cadastrado");
            
            _clienteRepoMock.Verify(
                r => r.CadastrarAsync(It.IsAny<Cliente>()), 
                Times.Once, 
                "porque deve chamar o método de cadastro do repositório");
                
            _uotMock.Verify(
                u => u.SalvarAlteracoesAsync(), 
                Times.Once, 
                "porque deve salvar as alterações na unidade de trabalho");
        }

        [Fact]
        public async Task Dado_RequestInvalido_Quando_CadastrarAsync_Entao_LancaExcecao()
        {
            // Arrange
            CadastrarClienteRequest request = null;

            // Act
            Func<Task> act = async () => await _clienteServico.CadastrarAsync(request);

            // Assert
            await act.Should()
                .ThrowAsync<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'request')");
        }

        [Fact]
        public async Task Dado_ClienteExistente_Quando_ObterPorIdAsync_Entao_RetornaCliente()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var cliente = new Cliente { Id = clienteId, Nome = "Cliente Teste" };
            var clienteResponse = new ClienteResponse { Id = clienteId, Nome = "Cliente Teste" };

            _clienteRepoMock.Setup(r => r.ObterPorIdAsync(clienteId))
                .ReturnsAsync(cliente);
            _mapperMock.Setup(m => m.Map<ClienteResponse>(cliente))
                .Returns(clienteResponse);

            // Act
            var resultado = await _clienteServico.ObterPorIdAsync(clienteId);

            // Assert
            resultado.Should().NotBeNull("porque o cliente existe no repositório");
            resultado.Id.Should().Be(clienteId, "porque deve retornar o cliente com o ID especificado");
            resultado.Nome.Should().Be("Cliente Teste", "porque deve retornar o nome correto do cliente");
        }

        [Fact]
        public async Task Dado_ClienteInexistente_Quando_ObterPorIdAsync_Entao_LancaExcecao()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            _clienteRepoMock.Setup(r => r.ObterPorIdAsync(clienteId))
                .ReturnsAsync((Cliente)null);

            // Act
            Func<Task> act = async () => await _clienteServico.ObterPorIdAsync(clienteId);

            // Assert
            await act.Should()
                .ThrowAsync<EntidadeNaoEncontradaException>()
                .WithMessage($"Cliente com ID {clienteId} não encontrado");
        }

        [Fact]
        public async Task Dado_ClientesExistentes_Quando_ListarTodosAsync_Entao_RetornaListaClientes()
        {
            // Arrange
            var clientes = new List<Cliente>
            {
                new Cliente { Id = Guid.NewGuid(), Nome = "Cliente 1" },
                new Cliente { Id = Guid.NewGuid(), Nome = "Cliente 2" }
            };

            var clientesResponse = clientes.Select(c => new ClienteResponse 
            { 
                Id = c.Id, 
                Nome = c.Nome 
            }).ToList();

            _clienteRepoMock.Setup(r => r.ListarTodosAsync())
                .ReturnsAsync(clientes);
            _mapperMock.Setup(m => m.Map<IEnumerable<ClienteResponse>>(clientes))
                .Returns(clientesResponse);

            // Act
            var resultado = await _clienteServico.ListarTodosAsync();

            // Assert
            resultado.Should().NotBeNull("porque existem clientes cadastrados");
            resultado.Should().HaveCount(2, "porque existem dois clientes cadastrados");
            resultado.Should().Contain(c => c.Nome == "Cliente 1", "porque o primeiro cliente deve estar na lista");
            resultado.Should().Contain(c => c.Nome == "Cliente 2", "porque o segundo cliente deve estar na lista");
        }
    }
}
