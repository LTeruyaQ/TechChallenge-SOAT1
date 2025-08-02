using Aplicacao.DTOs.Requests.Cliente;
using Aplicacao.DTOs.Responses.Cliente;
using Aplicacao.Servicos;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Especificacoes.Cliente;
using Dominio.Especificacoes.Contato;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using FluentAssertions;
using Moq;

namespace MecanicaOSTests.Servicos
{
    public class ClienteServicoNovosTests
    {
        private readonly Mock<IRepositorio<Cliente>> _clienteRepoMock;
        private readonly Mock<IRepositorio<Endereco>> _enderecoRepoMock;
        private readonly Mock<IRepositorio<Contato>> _contatoRepoMock;
        private readonly Mock<ILogServico<ClienteServico>> _logMock;
        private readonly Mock<IUnidadeDeTrabalho> _uotMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUsuarioLogadoServico> _usuarioLogadoServico;
        private readonly ClienteServico _clienteServico;

        public ClienteServicoNovosTests()
        {
            _clienteRepoMock = new Mock<IRepositorio<Cliente>>();
            _enderecoRepoMock = new Mock<IRepositorio<Endereco>>();
            _contatoRepoMock = new Mock<IRepositorio<Contato>>();
            _logMock = new Mock<ILogServico<ClienteServico>>();
            _uotMock = new Mock<IUnidadeDeTrabalho>();
            _mapperMock = new Mock<IMapper>();
            _usuarioLogadoServico = new Mock<IUsuarioLogadoServico>();

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
        public async Task Dado_ClienteComDocumentoExistente_Quando_CadastrarAsync_Entao_DeveLancarExcecao()
        {
            // Arrange
            var request = new CadastrarClienteRequest 
            { 
                Nome = "João Silva",
                Documento = "123.456.789-09",
                TipoCliente = TipoCliente.PessoaJuridico,
                DataNascimento = "01/01/1990",
                Email = "joao@exemplo.com",
                Telefone = "(11) 99999-9999"
            };

            _clienteRepoMock.Setup(x => x.ObterUmAsync(It.IsAny<ObterClientePorDocumento>()))
                .ReturnsAsync(new Cliente());

            // Act & Assert
            await _clienteServico.Invoking(x => x.CadastrarAsync(request))
                .Should().ThrowAsync<DadosJaCadastradosException>()
                .WithMessage("Já existe um cliente cadastrado com este documento");
        }

        [Fact]
        public async Task Dado_ClienteComEmailExistente_Quando_CadastrarAsync_Entao_DeveLancarExcecao()
        {
            // Arrange
            var request = new CadastrarClienteRequest 
            { 
                Nome = "João Silva",
                Documento = "123.456.789-09",
                TipoCliente = TipoCliente.PessoaJuridico,
                DataNascimento = "01/01/1990",
                Email = "joao@exemplo.com",
                Telefone = "(11) 99999-9999"
            };

            _clienteRepoMock.Setup(x => x.ObterUmAsync(It.IsAny<ObterClientePorDocumento>()))
                .ReturnsAsync((Cliente)null);

            _contatoRepoMock.Setup(x => x.ObterUmAsync(It.IsAny<ObterContatoPorEmailEspecificacao>()))
                .ReturnsAsync(new Contato { Email = request.Email });

            _mapperMock.Setup(m => m.Map<Cliente>(It.IsAny<CadastrarClienteRequest>()))
                .Returns(new Cliente 
                { 
                    Nome = "Nome Teste",
                    Documento = "12345678901",
                    TipoCliente = TipoCliente.PessoaFisica,
                    DataNascimento = DateTime.Now.ToString("dd/MM/yyyy"),
                    Endereco = new Endereco(),
                    Contato = new Contato()
                });

            // Act & Assert
            await _clienteServico.Invoking(x => x.CadastrarAsync(request))
                .Should().ThrowAsync<DadosJaCadastradosException>()
                .WithMessage("Já existe um cliente cadastrado com este e-mail");
        }

        [Fact]
        public async Task Dado_ClienteValido_Quando_AtualizarContato_Entao_DeveAtualizarContato()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var contatoId = Guid.NewGuid();
            
            var clienteExistente = new Cliente
            {
                Id = clienteId,
                Contato = new Contato 
                { 
                    Id = contatoId,
                    Email = "antigo@exemplo.com",
                    Telefone = "(11) 99999-9999"
                }
            };

            var request = new AtualizarContatoRequest
            {
                Email = "novo@exemplo.com",
                Telefone = "(11) 98888-8888"
            };

            _clienteRepoMock.Setup(x => x.ObterPorIdAsync(clienteId))
                .ReturnsAsync(clienteExistente);

            _contatoRepoMock.Setup(x => x.EditarAsync(It.IsAny<Contato>()));
            _uotMock.Setup(x => x.Commit())
                .ReturnsAsync(true);

            // Act
            await _clienteServico.AtualizarContatoAsync(clienteId, request);

            // Assert
            _contatoRepoMock.Verify(x => x.EditarAsync(It.Is<Contato>(c => 
                c.Email == request.Email && 
                c.Telefone == request.Telefone)), 
                Times.Once);

            _uotMock.Verify(x => x.Commit(), Times.Once);
        }

        [Fact]
        public async Task Dado_ClienteNaoEncontrado_Quando_AtualizarContato_Entao_DeveLancarExcecao()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var request = new AtualizarContatoRequest();

            _clienteRepoMock.Setup(x => x.ObterPorIdAsync(clienteId))
                .ReturnsAsync((Cliente)null);

            // Act & Assert
            await _clienteServico.Invoking(x => x.AtualizarContatoAsync(clienteId, request))
                .Should().ThrowAsync<DadosNaoEncontradosException>()
                .WithMessage("Cliente não encontrado");
        }

        [Fact]
        public async Task Dado_EmailEmUso_Quando_AtualizarContato_Entao_DeveLancarExcecao()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var outroClienteId = Guid.NewGuid();
            
            var clienteExistente = new Cliente
            {
                Id = clienteId,
                Contato = new Contato 
                { 
                    Id = Guid.NewGuid(),
                    Email = "antigo@exemplo.com"
                }
            };

            var request = new AtualizarContatoRequest
            {
                Email = "email_em_uso@exemplo.com"
            };

            _clienteRepoMock.Setup(x => x.ObterPorIdAsync(clienteId))
                .ReturnsAsync(clienteExistente);

            // Configura o mock para retornar um contato quando buscar por email (simulando que o email já está em uso por outro cliente)
            _contatoRepoMock.Setup(x => x.ObterUmAsync(It.Is<ObterContatoPorEmailEspecificacao>(s => 
                s.Expressao.Compile()(new Contato { Email = request.Email, IdCliente = outroClienteId }))))
                .ReturnsAsync(new Contato { Email = request.Email, IdCliente = outroClienteId });

            // Configura o mock para retornar true no Commit (para evitar a PersistirDadosException)
            _uotMock.Setup(x => x.Commit()).ReturnsAsync(true);

            // Act & Assert
            await _clienteServico.Invoking(x => x.AtualizarContatoAsync(clienteId, request))
                .Should().ThrowAsync<DadosJaCadastradosException>()
                .WithMessage("Já existe um cliente cadastrado com este e-mail");
        }
        
        [Fact]
        public async Task Dado_ClienteComEmailExistente_Quando_AtualizarAsync_Entao_DeveLancarExcecao()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var outroClienteId = Guid.NewGuid();
            var contatoId = Guid.NewGuid();
            
            var clienteExistente = new Cliente
            {
                Id = clienteId,
                Nome = "João Silva",
                Documento = "123.456.789-09",
                TipoCliente = TipoCliente.PessoaFisica,
                DataNascimento = "01/01/1990",
                Contato = new Contato 
                { 
                    Id = contatoId,
                    Email = "antigo@exemplo.com",
                    Telefone = "(11) 99999-9999"
                }
            };

            var request = new AtualizarClienteRequest
            {
                Id = clienteId,
                Nome = "João Silva",
                Documento = "123.456.789-09",
                TipoCliente = TipoCliente.PessoaFisica,
                DataNascimento = "01/01/1990",
                Email = "novo@exemplo.com",
                ContatoId = contatoId,
                Telefone = "(11) 98888-8888"
            };

            _clienteRepoMock.Setup(x => x.ObterPorIdAsync(clienteId))
                .ReturnsAsync(clienteExistente);

            // Configura o mock para retornar o contato quando buscar por email
            _contatoRepoMock.Setup(x => x.ObterUmAsync(It.Is<ObterContatoPorEmailEspecificacao>(
                    spec => spec.Expressao.Compile()(new Contato { Email = request.Email, IdCliente = outroClienteId }))))
                .ReturnsAsync(new Contato { Email = request.Email, IdCliente = outroClienteId });

            // Configura o mock para retornar o contato quando buscar por ID
            _contatoRepoMock.Setup(x => x.ObterPorIdAsync(contatoId))
                .ReturnsAsync(new Contato { 
                    Id = contatoId, 
                    Email = "antigo@exemplo.com", 
                    IdCliente = clienteId 
                });

            // Configura o mock para retornar true no Commit
            _uotMock.Setup(x => x.Commit()).ReturnsAsync(true);

            // Act & Assert
            await _clienteServico.Invoking(x => x.AtualizarAsync(clienteId, request))
                .Should().ThrowAsync<DadosJaCadastradosException>()
                .WithMessage("Já existe um cliente cadastrado com este e-mail");
        }

        [Fact]
        public async Task Dado_AtualizarClienteComMesmoEmail_Quando_AtualizarAsync_Entao_NaoDeveLancarExcecao()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var contatoId = Guid.NewGuid();
            var email = "mesmo@exemplo.com";
            
            var clienteExistente = new Cliente
            {
                Id = clienteId,
                Nome = "João Silva",
                Documento = "123.456.789-09",
                TipoCliente = TipoCliente.PessoaFisica,
                DataNascimento = "01/01/1990",
                Contato = new Contato 
                { 
                    Id = contatoId,
                    Email = email,
                    Telefone = "(11) 99999-9999"
                }
            };

            var request = new AtualizarClienteRequest
            {
                Id = clienteId,
                Nome = "João Silva",
                Documento = "123.456.789-09",
                TipoCliente = TipoCliente.PessoaFisica,
                DataNascimento = "01/01/1990",
                Email = email, // Mesmo e-mail
                ContatoId = contatoId,
                Telefone = "(11) 98888-8888" // Apenas o telefone foi alterado
            };

            _clienteRepoMock.Setup(x => x.ObterPorIdAsync(clienteId))
                .ReturnsAsync(clienteExistente);

            _contatoRepoMock.Setup(x => x.ObterUmAsync(It.Is<ObterContatoPorEmailEspecificacao>(
                    spec => spec.Expressao.Compile()(new Contato { Email = request.Email, IdCliente = clienteId }))))
                .ReturnsAsync(clienteExistente.Contato);

            _contatoRepoMock.Setup(x => x.EditarAsync(It.IsAny<Contato>()))
                .Returns(Task.CompletedTask);

            _clienteRepoMock.Setup(x => x.EditarAsync(It.IsAny<Cliente>()))
                .Returns(Task.CompletedTask);

            _uotMock.Setup(x => x.Commit())
                .ReturnsAsync(true);

            // Act
            Func<Task> act = async () => await _clienteServico.AtualizarAsync(clienteId, request);

            // Assert
            await act.Should().NotThrowAsync<DadosJaCadastradosException>();
        }

        [Fact]
        public async Task Dado_DocumentoEmUso_Quando_AtualizarCliente_Entao_DeveLancarExcecao()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var outroClienteId = Guid.NewGuid();
            
            var clienteExistente = new Cliente
            {
                Id = clienteId,
                Nome = "João Silva",
                Documento = "123.456.789-09",
                TipoCliente = TipoCliente.PessoaJuridico,
                DataNascimento = "01/01/1990"
            };

            var request = new AtualizarClienteRequest
            {
                Nome = "João Silva",
                Documento = "987.654.321-00", // Novo documento que já está em uso
                TipoCliente = TipoCliente.PessoaJuridico,
                DataNascimento = "01/01/1990"
            };

            _clienteRepoMock.Setup(x => x.ObterPorIdAsync(clienteId))
                .ReturnsAsync(clienteExistente);

            _clienteRepoMock.Setup(x => x.ObterUmAsync(It.Is<ObterClientePorDocumento>(
                    spec => spec.Expressao.Compile()(new Cliente { Documento = request.Documento }))))
                .ReturnsAsync(new Cliente { Id = outroClienteId });

            // Act & Assert
            await _clienteServico.Invoking(x => x.AtualizarAsync(clienteId, request))
                .Should().ThrowAsync<DadosJaCadastradosException>()
                .WithMessage("Já existe um cliente cadastrado com este documento");
        }
    }
}
