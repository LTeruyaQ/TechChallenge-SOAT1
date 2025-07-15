using Aplicacao.DTOs.Requests.Autenticacao;
using Aplicacao.DTOs.Requests.Usuario;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.Servicos;
using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Exceptions;
using Dominio.Interfaces.Servicos;
using Moq;


public class AutenticacaoServicoTests
{

    private readonly Mock<IUsuarioServico> _usuarioServicoMock = new();
    private readonly Mock<IServicoSenha> _servicoSenhaMock = new();
    private readonly Mock<IServicoJwt> _servicoJwtMock = new();
    private readonly Mock<IClienteServico> _clienteServicoMock = new();
    private readonly Mock<ILogServico<AutenticacaoServico>> _logMock = new();

    private readonly AutenticacaoServico _servico;
    public AutenticacaoServicoTests()
    {
        _servico = new AutenticacaoServico(
            _usuarioServicoMock.Object,
            _servicoSenhaMock.Object,
            _servicoJwtMock.Object,
            _logMock.Object,
            _clienteServicoMock.Object);
    }
    [Fact]
    public async Task Given_EmailInexistente_When_AutenticarAsync_Then_ThrowsCredenciaisInvalidasException()
    {
        // Arrange
        var request = new AutenticacaoRequest { Email = "naoexiste@email.com", Senha = "123456" };
        _usuarioServicoMock.Setup(s => s.ObterPorEmailAsync(request.Email))
            .ReturnsAsync((Usuario?)null);

        // Act & Assert
        await Assert.ThrowsAsync<CredenciaisInvalidasException>(() =>
            _servico.AutenticarAsync(request));
    }
    [Fact]
    public async Task Given_UsuarioInativo_When_AutenticarAsync_Then_ThrowsUsuarioInativoException()
    {
        var request = new AutenticacaoRequest { Email = "ativo@email.com", Senha = "123456" };
        var usuario = new Usuario { Email = request.Email, Ativo = false };

        _usuarioServicoMock.Setup(s => s.ObterPorEmailAsync(request.Email))
            .ReturnsAsync(usuario);

        // Act & Assert
        await Assert.ThrowsAsync<UsuarioInativoException>(() =>
            _servico.AutenticarAsync(request));
    }
    [Fact]
    public async Task Given_SenhaIncorreta_When_AutenticarAsync_Then_ThrowsCredenciaisInvalidasException()
    {
        var request = new AutenticacaoRequest { Email = "teste@email.com", Senha = "errada" };
        var usuario = new Usuario { Email = request.Email, Ativo = true, Senha = "hash123" };

        _usuarioServicoMock.Setup(s => s.ObterPorEmailAsync(request.Email)).ReturnsAsync(usuario);
        _servicoSenhaMock.Setup(s => s.VerificarSenha(request.Senha, usuario.Senha)).Returns(false);

        // Act & Assert
        await Assert.ThrowsAsync<CredenciaisInvalidasException>(() =>
            _servico.AutenticarAsync(request));
    }
    [Fact]
    public async Task Given_UsuarioClienteSemClienteId_When_AutenticarAsync_Then_ThrowsDadosInvalidosException()
    {
        var request = new AutenticacaoRequest { Email = "cliente@email.com", Senha = "senha123" };
        var usuario = new Usuario
        {
            Email = request.Email,
            Ativo = true,
            Senha = "hash123",
            TipoUsuario = TipoUsuario.Cliente,
            ClienteId = null
        };

        _usuarioServicoMock.Setup(s => s.ObterPorEmailAsync(request.Email)).ReturnsAsync(usuario);
        _servicoSenhaMock.Setup(s => s.VerificarSenha(request.Senha, usuario.Senha)).Returns(true);

        // Act & Assert
        await Assert.ThrowsAsync<DadosInvalidosException>(() =>
            _servico.AutenticarAsync(request));
    }
    //[Fact]
    //public async Task Given_UsuarioAdminComCredenciaisValidas_When_AutenticarAsync_Then_ReturnsToken()
    //{
    //    var request = new AutenticacaoRequest { Email = "admin@email.com", Senha = "123456" };
    //    var usuario = new Usuario
    //    {
    //        Id = Guid.NewGuid(),
    //        Email = request.Email,
    //        Ativo = true,
    //        Senha = "hash",
    //        TipoUsuario = TipoUsuario.Admin
    //    };

    //    _usuarioServicoMock.Setup(s => s.ObterPorEmailAsync(request.Email)).ReturnsAsync(usuario);
    //    _servicoSenhaMock.Setup(s => s.VerificarSenha(request.Senha, usuario.Senha)).Returns(true);
    //    _servicoJwtMock.Setup(j => j.GerarToken(
    //        usuario.Id,
    //        usuario.Email,
    //        usuario.TipoUsuario.ToString(),
    //        usuario.Email,
    //        It.Is<IEnumerable<string>>(p => p.Contains("administrador"))))
    //        .Returns("token_admin");

    //    _usuarioServicoMock.Setup(s => s.AtualizarAsync(usuario.Id, It.IsAny<AtualizarUsuarioRequest>()))
    //        .Returns(Task.CompletedTask);

    //    var result = await _servico.AutenticarAsync(request);

    //    Assert.Equal("token_admin", result.Token);
    //}
    //[Fact]
    //public async Task Given_UsuarioClienteComCredenciaisValidas_When_AutenticarAsync_Then_ReturnsToken()
    //{
    //    var request = new AutenticacaoRequest { Email = "cliente@email.com", Senha = "123456" };
    //    var usuario = new Usuario
    //    {
    //        Id = Guid.NewGuid(),
    //        Email = request.Email,
    //        Ativo = true,
    //        Senha = "hash",
    //        TipoUsuario = TipoUsuario.Cliente,
    //        ClienteId = Guid.NewGuid()
    //    };

    //    var cliente = new Cliente { Nome = "João Cliente" };

    //    _usuarioServicoMock.Setup(s => s.ObterPorEmailAsync(request.Email)).ReturnsAsync(usuario);
    //    _servicoSenhaMock.Setup(s => s.VerificarSenha(request.Senha, usuario.Senha)).Returns(true);
    //    _clienteServicoMock.Setup(c => c.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(cliente);

    //    _servicoJwtMock.Setup(j => j.GerarToken(
    //        usuario.Id,
    //        usuario.Email,
    //        usuario.TipoUsuario.ToString(),
    //        cliente.Nome,
    //        It.Is<IEnumerable<string>>(p => p.Contains("cliente"))))
    //        .Returns("token_cliente");

    //    _usuarioServicoMock.Setup(s => s.AtualizarAsync(usuario.Id, It.IsAny<AtualizarUsuarioRequest>()))
    //        .Returns(Task.CompletedTask);

    //    var result = await _servico.AutenticarAsync(request);

    //    Assert.Equal("token_cliente", result.Token);
    //}
}