using Aplicacao.DTOs.Requests.Cliente;
using Aplicacao.DTOs.Responses.Cliente;
using Aplicacao.Servicos;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Especificacoes.Base;
using Dominio.Especificacoes.Base.Interfaces;
using Dominio.Especificacoes.Cliente;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using Moq;

namespace MecanicaOSTests.Servicos
{
    public class ClienteServicoTests
    {
        private readonly Mock<IRepositorio<Cliente>> _clienteRepoMock;
        private readonly Mock<IRepositorio<Endereco>> _enderecoRepoMock;
        private readonly Mock<IRepositorio<Contato>> _contatoRepoMock;
        private readonly Mock<ILogServico<ClienteServico>> _logMock;
        private readonly Mock<IUnidadeDeTrabalho> _uotMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUsuarioLogadoServico> _usuarioLogadoServico = new();

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
                _mapperMock.Object,
                _usuarioLogadoServico.Object);
        }

        [Fact]
        public async Task Dado_RequestValido_Quando_CadastrarAsync_Entao_RetornaClienteResponse()
        {
            var request = new CadastrarClienteRequest { Nome = "João" };
            var cliente = new Cliente { Id = Guid.NewGuid(), Nome = "João" };
            var response = new ClienteResponse { Id = cliente.Id, Nome = cliente.Nome };

            _mapperMock.Setup(m => m.Map<Cliente>(request)).Returns(cliente);
            _clienteRepoMock.Setup(r => r.CadastrarAsync(cliente)).ReturnsAsync(cliente);
            _uotMock.Setup(u => u.Commit()).ReturnsAsync(true);
            _mapperMock.Setup(m => m.Map<ClienteResponse>(cliente)).Returns(response);

            var result = await _clienteServico.CadastrarAsync(request);

            Assert.Equal(cliente.Id, result.Id);
            Assert.Equal(cliente.Nome, result.Nome);
        }

        [Fact]
        public async Task Dado_FalhaNoCommit_Quando_CadastrarAsync_Entao_LancaExcecaoPersistirDados()
        {
            var request = new CadastrarClienteRequest();
            var cliente = new Cliente { Id = Guid.NewGuid() };

            _mapperMock.Setup(m => m.Map<Cliente>(request)).Returns(cliente);
            _clienteRepoMock.Setup(r => r.CadastrarAsync(cliente)).ReturnsAsync(cliente);
            _uotMock.Setup(u => u.Commit()).ReturnsAsync(false);

            await Assert.ThrowsAsync<PersistirDadosException>(() => _clienteServico.CadastrarAsync(request));
        }

        [Fact]
        public async Task Dado_IdValido_Quando_AtualizarAsync_Entao_RetornaClienteResponse()
        {
            var id = Guid.NewGuid();
            var cliente = new Cliente { Id = id, Nome = "Maria" };
            var response = new ClienteResponse { Id = id, Nome = "Maria" };

            _clienteRepoMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(cliente);
            _mapperMock.Setup(m => m.Map<ClienteResponse>(cliente)).Returns(response);

            var result = await _clienteServico.ObterPorIdAsync(id);

            Assert.Equal(id, result.Id);
            Assert.Equal("Maria", result.Nome);
        }

        [Fact]
        public async Task Dado_IdInvalido_Quando_ObterPorIdAsync_Entao_LancaExcecao()
        {
            _clienteRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Cliente)null);

            await Assert.ThrowsAsync<DadosNaoEncontradosException>(() => _clienteServico.ObterPorIdAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task Dado_IdValido_Quando_DeletarAsync_Entao_RetornaTrue()
        {
            var id = Guid.NewGuid();
            var cliente = new Cliente { Id = id };

            _clienteRepoMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(cliente);
            _clienteRepoMock.Setup(r => r.DeletarAsync(cliente)).Returns(Task.CompletedTask);
            _uotMock.Setup(u => u.Commit()).ReturnsAsync(true);

            var result = await _clienteServico.RemoverAsync(id);

            Assert.True(result);
        }

        [Fact]
        public async Task Dado_IdInvalido_Quando_DeletarAsync_Entao_LancaExcecao()
        {
            var id = Guid.NewGuid();
            var cliente = new Cliente { Id = id };

            _clienteRepoMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(cliente);
            _clienteRepoMock.Setup(r => r.DeletarAsync(cliente)).Returns(Task.CompletedTask);
            _uotMock.Setup(u => u.Commit()).ReturnsAsync(false);

            await Assert.ThrowsAsync<PersistirDadosException>(() => _clienteServico.RemoverAsync(id));
        }

        [Fact]
        public async Task Dado_DocumentoExistente_Quando_ObterPorDocumento_Then_ReturnCliente()
        {
            var documento = "12345678901";
            var cliente = new Cliente { Documento = documento };

            _clienteRepoMock
                .Setup(r => r.ObterUmAsync(It.IsAny<ObterClientePorDocumento>()))
                .ReturnsAsync(cliente);

            var result = await _clienteServico.ObterPorDocumento(documento);

            Assert.Equal(documento, result.Documento);
        }

        [Fact]
        public async Task Dado_DocumentoInexistente_Quando_ObterPorDocumento_Then_ThrowException()
        {
            await Assert.ThrowsAsync<DadosNaoEncontradosException>(() => _clienteServico.ObterPorDocumento("00000000000"));
        }
    }
}
