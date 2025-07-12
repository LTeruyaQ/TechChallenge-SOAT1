using Aplicacao.DTOs.Requests.Cliente;
using Aplicacao.DTOs.Responses.Cliente;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using Moq;

namespace Aplicacao.Servicos.Tests
{
    public class ClienteServicoTests
    {
        private readonly Mock<IRepositorio<Cliente>> _clienteRepo = new Mock<IRepositorio<Cliente>>();
        private readonly Mock<IRepositorio<Endereco>> _enderecoRepo = new Mock<IRepositorio<Endereco>>();
        private readonly Mock<IRepositorio<Contato>> _contatoRepo = new Mock<IRepositorio<Contato>>();
        private readonly Mock<IUnidadeDeTrabalho> _uotRepo = new Mock<IUnidadeDeTrabalho>();
        private readonly Mock<ILogServico<ClienteServico>> _logServico = new Mock<ILogServico<ClienteServico>>();
        private readonly Mock<IMapper> _mapper = new Mock<IMapper>();


        private ClienteServico CreateService()
        {
            return new ClienteServico(_clienteRepo.Object, _enderecoRepo.Object, _contatoRepo.Object, _logServico.Object, _uotRepo.Object, _mapper.Object);
        }


        [Fact]
        public async Task Given_ClienteNaoExiste_when_AtualizarAsync_Then_ThrowsDadosNaoEncontradosException()
        {
            var id = Guid.NewGuid(); ;
            _clienteRepo.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync((Cliente)null);
            var service = CreateService();

            await Assert.ThrowsAsync<DadosNaoEncontradosException>(() => service.AtualizarAsync(id, new AtualizarClienteRequest()));

        }



        [Fact]
        public async Task Given_ValidRequest_When_AtualizarAsync_Then_UpdateFieldsAndComits()
        {
            var id = Guid.NewGuid();
            var cliente = new Cliente() { Id = id };
            _clienteRepo.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(cliente);
            _clienteRepo.Setup(r => r.EditarAsync(cliente)).Returns(Task.CompletedTask);
            _uotRepo.Setup(u => u.Commit()).ReturnsAsync(true);
            _mapper.Setup(m => m.Map<ClienteResponse>(It.IsAny<Cliente>())).Returns(new ClienteResponse());

            var service = CreateService();

            var request = new AtualizarClienteRequest 
            { 
                Nome = "Novo Nome",
                Sexo = "M",
                TipoCliente = "PJ",
                Documento = "546",
                DataNascimento = DateTime.UtcNow.ToString(),
                EnderecoId = Guid.Empty,
                ContatoId = Guid.Empty,
            };

            var result = await service.AtualizarAsync(id, request);
            Assert.NotNull(result);
            Assert.Equal("Novo Nome", cliente.Nome);
            _clienteRepo.Verify(r => r.EditarAsync(cliente), Times.Once);
            _uotRepo.Verify(u => u.Commit(), Times.Once);



        }

    }
}