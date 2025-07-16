using Aplicacao.DTOs.Requests.Usuario;
using Aplicacao.DTOs.Responses.Usuario;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.Servicos;
using AutoMapper;
using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Especificacoes.Base.Interfaces;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using Moq;

public class UsuarioServicoTests
{
    private readonly Mock<IRepositorio<Usuario>> _repositorioMock = new();
    private readonly Mock<IUnidadeDeTrabalho> _uotMock = new();
    private readonly Mock<ILogServico<UsuarioServico>> _logMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IClienteServico> _clienteServicoMock = new();
    private readonly Mock<IServicoSenha> _servicoSenhaMock = new();

    private UsuarioServico CriarServico()
    {
        return new UsuarioServico(
            _repositorioMock.Object,
            _logMock.Object,
            _uotMock.Object,
            _mapperMock.Object,
            _clienteServicoMock.Object,
            _servicoSenhaMock.Object
        );
    }
    [Fact]
    public async Task Dado_NovoUsuarioClienteValido_Quando_CadastrarAsync_Entao_UsuarioCriadoComSucesso()
    {
        // Arrange
        var servico = CriarServico();

        var request = new CadastrarUsuarioRequest
        {
            Email = "cliente@email.com",
            Senha = "senha123",
            TipoUsuario = TipoUsuario.Cliente,
            Documento = "12345678900"
        };

        var usuario = new Usuario();

        _repositorioMock.Setup(r => r.ObterUmSemRastreamentoAsync(It.IsAny<IEspecificacao<Usuario>>()))
            .ReturnsAsync((Usuario)null);

        _mapperMock.Setup(m => m.Map<Usuario>(request)).Returns(usuario);
        _servicoSenhaMock.Setup(s => s.CriptografarSenha(request.Senha)).Returns("senhaHash");

        _clienteServicoMock.Setup(c => c.ObterPorDocumento(request.Documento)).ReturnsAsync(new Cliente { Id = Guid.NewGuid() });
        _repositorioMock.Setup(r => r.CadastrarAsync(usuario)).ReturnsAsync(usuario);
        _uotMock.Setup(u => u.Commit()).ReturnsAsync(true);
        _mapperMock.Setup(m => m.Map<UsuarioResponse>(usuario)).Returns(new UsuarioResponse());

        // Act
        var result = await servico.CadastrarAsync(request);

        // Assert
        Assert.NotNull(result);
        _repositorioMock.Verify(r => r.CadastrarAsync(It.IsAny<Usuario>()), Times.Once);
    }

    [Fact]
    public async Task Dado_EmailJaCadastrado_Quando_CadastrarAsync_Entao_LancaExcecao()
    {
        // Arrange
        var servico = CriarServico();
        var request = new CadastrarUsuarioRequest { Email = "duplicado@email.com", Senha = "senha" };

        _repositorioMock.Setup(r => r.ObterUmSemRastreamentoAsync(It.IsAny<IEspecificacao<Usuario>>()))
            .ReturnsAsync(new Usuario());

        // Act & Assert
        await Assert.ThrowsAsync<DadosJaCadastradosException>(() => servico.CadastrarAsync(request));
    }

    [Fact]
    public async Task Dado_FalhaNoCommit_Quando_CadastrarAsync_Entao_LancaExcecaoPersistirDados()
    {
        // Arrange
        var servico = CriarServico();
        var request = new CadastrarUsuarioRequest 
        { 
            Email = "novo@email.com", 
            Senha = "senha",
            TipoUsuario = TipoUsuario.Admin
        };
        
        var usuario = new Usuario 
        { 
            Email = request.Email,
            Senha = "senhaHash",
            TipoUsuario = request.TipoUsuario
        };

        _repositorioMock
            .Setup(r => r.ObterUmSemRastreamentoAsync(It.IsAny<IEspecificacao<Usuario>>()))
            .ReturnsAsync((Usuario)null);
            
        _mapperMock
            .Setup(m => m.Map<Usuario>(request))
            .Returns(usuario);
            
        _servicoSenhaMock
            .Setup(s => s.CriptografarSenha(request.Senha))
            .Returns("senhaHash");
            
        _repositorioMock
            .Setup(r => r.CadastrarAsync(It.Is<Usuario>(u => 
                u.Email == request.Email && 
                u.Senha == "senhaHash" &&
                u.TipoUsuario == request.TipoUsuario)))
            .ReturnsAsync(usuario);
            
        _uotMock
            .Setup(u => u.Commit())
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PersistirDadosException>(
            () => servico.CadastrarAsync(request));
            
        Assert.Equal("Erro ao cadastrar usuário", exception.Message);
        _repositorioMock.Verify(r => r.CadastrarAsync(It.IsAny<Usuario>()), Times.Once);
        _uotMock.Verify(u => u.Commit(), Times.Once);
    }
    [Fact]
    public async Task Dado_AtualizacaoValida_Quando_AtualizarAsync_Entao_UsuarioAtualizadoComSucesso()
    {
        // Arrange
        var servico = CriarServico();
        var id = Guid.NewGuid();
        var usuario = new Usuario();
        var request = new AtualizarUsuarioRequest
        {
            Email = "novo@email.com",
            Senha = "novaSenha",
            DataUltimoAcesso = DateTime.Now,
            TipoUsuario = TipoUsuario.Admin,
            RecebeAlertaEstoque = true,
            Documento = "123456789"
        };

        _repositorioMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(usuario);
        _servicoSenhaMock.Setup(s => s.CriptografarSenha(request.Senha)).Returns("senhaNova");
        _clienteServicoMock.Setup(c => c.ObterPorDocumento(request.Documento)).ReturnsAsync(new Cliente { Id = Guid.NewGuid() });
        _uotMock.Setup(u => u.Commit()).ReturnsAsync(true);
        _mapperMock.Setup(m => m.Map<UsuarioResponse>(usuario)).Returns(new UsuarioResponse());

        // Act
        var result = await servico.AtualizarAsync(id, request);

        // Assert
        Assert.NotNull(result);
        _repositorioMock.Verify(r => r.EditarAsync(usuario), Times.Once);
    }

    [Fact]
    public async Task Dado_IdInvalido_Quando_AtualizarAsync_Entao_LancaExcecaoDadosNaoEncontrados()
    {
        var servico = CriarServico();
        _repositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Usuario)null);

        await Assert.ThrowsAsync<DadosNaoEncontradosException>(() =>
            servico.AtualizarAsync(Guid.NewGuid(), new AtualizarUsuarioRequest()));
    }

    [Fact]
    public async Task Dado_FalhaNoCommit_Quando_AtualizarAsync_Entao_LancaExcecaoPersistirDados()
    {
        var servico = CriarServico();
        var id = Guid.NewGuid();
        var usuario = new Usuario();
        var request = new AtualizarUsuarioRequest();

        _repositorioMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(usuario);
        _uotMock.Setup(u => u.Commit()).ReturnsAsync(false);

        var ex = await Assert.ThrowsAsync<PersistirDadosException>(() =>
            servico.AtualizarAsync(id, request));
    }
    [Fact]
    public async Task Dado_IdValido_Quando_DeletarAsync_Entao_UsuarioRemovido()
    {
        var servico = CriarServico();
        var usuario = new Usuario();
        _repositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(usuario);
        _uotMock.Setup(u => u.Commit()).ReturnsAsync(true);

        var result = await servico.DeletarAsync(Guid.NewGuid());

        Assert.True(result);
        _repositorioMock.Verify(r => r.DeletarAsync(usuario), Times.Once);
    }

    [Fact]
    public async Task Dado_IdInvalido_Quando_DeletarAsync_Entao_LancaExcecao()
    {
        var servico = CriarServico();
        _repositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Usuario)null);

        await Assert.ThrowsAsync<DadosNaoEncontradosException>(() => servico.DeletarAsync(Guid.NewGuid()));
    }
    [Fact]
    public async Task Dado_IdValido_Quando_ObterPorIdAsync_Entao_RetornaUsuarioMapeado()
    {
        var servico = CriarServico();
        var usuario = new Usuario();
        _repositorioMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(usuario);
        _mapperMock.Setup(m => m.Map<UsuarioResponse>(usuario)).Returns(new UsuarioResponse());

        var result = await servico.ObterPorIdAsync(Guid.NewGuid());

        Assert.NotNull(result);
    }

    [Fact]
    public async Task Quando_ObterTodosAsync_Entao_RetornaUsuariosMapeados()
    {
        var servico = CriarServico();
        var lista = new List<Usuario> { new Usuario() };
        _repositorioMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(lista);
        _mapperMock.Setup(m => m.Map<IEnumerable<UsuarioResponse>>(lista)).Returns(new List<UsuarioResponse>());

        var result = await servico.ObterTodosAsync();

        Assert.NotNull(result);
    }
    [Fact]
    public async Task Dado_EmailValido_Quando_ObterPorEmailAsync_Entao_RetornaUsuario()
    {
        var servico = CriarServico();
        _repositorioMock.Setup(r => r.ObterUmSemRastreamentoAsync(It.IsAny<IEspecificacao<Usuario>>()))
            .ReturnsAsync(new Usuario());

        var result = await servico.ObterPorEmailAsync("email@teste.com");

        Assert.NotNull(result);
    }
}
