using Aplicacao.DTOs.Requests.Usuario;
using Aplicacao.DTOs.Responses.Cliente;
using Aplicacao.DTOs.Responses.Usuario;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.Servicos;
using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Exceptions;
using Dominio.Interfaces.Servicos;
using MecanicaOSTests.Fixtures;
using Moq;


public class AutenticacaoServicoTests : BaseTestFixture<AutenticacaoServico>
{
    private readonly Mock<IUsuarioServico> _usuarioServicoMock;
    private readonly Mock<IServicoSenha> _servicoSenhaMock;
    private readonly Mock<IServicoJwt> _servicoJwtMock;
    private readonly Mock<IClienteServico> _clienteServicoMock;
    private readonly Mock<ILogServico<AutenticacaoServico>> _logServicoMock;
    private readonly AutenticacaoServico _servico;
    public AutenticacaoServicoTests() : base()
    {
        _usuarioServicoMock = CreateServiceMock<IUsuarioServico>();
        _servicoSenhaMock = CreateServiceMock<IServicoSenha>();
        _servicoJwtMock = CreateServiceMock<IServicoJwt>();
        _clienteServicoMock = CreateServiceMock<IClienteServico>();
        _logServicoMock = CreateServiceMock<ILogServico<AutenticacaoServico>>();

        _servico = new AutenticacaoServico(
            _usuarioServicoMock.Object,
            _servicoSenhaMock.Object,
            _servicoJwtMock.Object,
            _logServicoMock.Object,
            _clienteServicoMock.Object);
    }
    [Fact]
    public async Task Dado_EmailInexistente_Quando_AutenticarAsync_Entao_LancaExcecaoCredenciaisInvalidas()
    {
        // Arrange
        var request = AutenticacaoFixture.CriarAutenticacaoRequestValida();
        _usuarioServicoMock.Setup(s => s.ObterPorEmailAsync(request.Email))
            .ReturnsAsync((Usuario?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<CredenciaisInvalidasException>(() =>
            _servico.AutenticarAsync(request));

        Assert.Equal("Credenciais inválidas", ex.Message);
        _logServicoMock.Verify(
            x => x.LogErro("AutenticarAsync", It.IsAny<Exception>()),
            Times.Once);
    }
    [Fact]
    public async Task Dado_UsuarioInativo_Quando_AutenticarAsync_Entao_LancaExcecaoUsuarioInativo()
    {
        // Arrange
        var request = AutenticacaoFixture.CriarAutenticacaoRequestValida();
        var usuarioInativo = AutenticacaoFixture.CriarUsuarioInativo();
        usuarioInativo.Email = request.Email;

        _usuarioServicoMock.Setup(s => s.ObterPorEmailAsync(request.Email))
            .ReturnsAsync(usuarioInativo);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<UsuarioInativoException>(() =>
            _servico.AutenticarAsync(request));
    }
    [Fact]
    public async Task Dado_SenhaIncorreta_Quando_AutenticarAsync_Entao_LancaExcecaoCredenciaisInvalidas()
    {
        // Arrange
        var request = AutenticacaoFixture.CriarAutenticacaoRequestValida();
        var usuario = AutenticacaoFixture.CriarUsuarioAtivo();
        usuario.Email = request.Email;
        usuario.Senha = "hash_correta";

        _usuarioServicoMock.Setup(s => s.ObterPorEmailAsync(request.Email)).ReturnsAsync(usuario);
        _servicoSenhaMock.Setup(s => s.VerificarSenha(request.Senha, usuario.Senha)).Returns(false);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<CredenciaisInvalidasException>(() =>
            _servico.AutenticarAsync(request));

        Assert.Equal("Credenciais inválidas", ex.Message);
        _logServicoMock.Verify(
            x => x.LogErro("AutenticarAsync", It.IsAny<Exception>()),
            Times.Once);
    }
    [Fact]
    public async Task Dado_UsuarioClienteSemClienteId_Quando_AutenticarAsync_Entao_LancaExcecaoDadosInvalidos()
    {
        // Arrange
        var request = AutenticacaoFixture.CriarAutenticacaoRequestValida();
        var usuario = AutenticacaoFixture.CriarUsuarioAtivo();
        usuario.Email = request.Email;
        usuario.TipoUsuario = TipoUsuario.Cliente;
        usuario.ClienteId = null; // ClienteId nulo para forçar o erro

        _usuarioServicoMock.Setup(s => s.ObterPorEmailAsync(request.Email)).ReturnsAsync(usuario);
        _servicoSenhaMock.Setup(s => s.VerificarSenha(request.Senha, usuario.Senha)).Returns(true);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<DadosInvalidosException>(() =>
            _servico.AutenticarAsync(request));

        Assert.Contains("Erro ao detectar usuario", ex.Message);
        _logServicoMock.Verify(
            x => x.LogErro("AutenticarAsync", It.IsAny<Exception>()),
            Times.Once);
    }
    [Fact]
    public async Task Dado_UsuarioAdminComCredenciaisValidas_Quando_AutenticarAsync_Entao_RetornaToken()
    {
        // Arrange
        var request = AutenticacaoFixture.CriarAutenticacaoRequestValida();
        var admin = AutenticacaoFixture.CriarUsuarioAdmin();
        admin.Email = request.Email;
        admin.Senha = "hash_criptografada";

        const string tokenEsperado = "token_gerado";

        _usuarioServicoMock.Setup(s => s.ObterPorEmailAsync(request.Email)).ReturnsAsync(admin);
        _servicoSenhaMock.Setup(s => s.VerificarSenha(request.Senha, admin.Senha)).Returns(true);
        _servicoJwtMock.Setup(j => j.GerarToken(
            admin.Id,
            admin.Email,
            admin.TipoUsuario.ToString(),
            admin.Email,
            It.Is<IEnumerable<string>>(p => p.Contains("administrador"))))
            .Returns(tokenEsperado);

        _usuarioServicoMock.Setup(s => s.AtualizarAsync(admin.Id, It.IsAny<AtualizarUsuarioRequest>()))
            .ReturnsAsync(new UsuarioResponse
            {
                Id = admin.Id,
                Email = admin.Email,
                TipoUsuario = admin.TipoUsuario,
                DataUltimoAcesso = DateTime.UtcNow,
                Ativo = true,
                DataCadastro = DateTime.UtcNow.AddDays(-1)
            });

        // Act
        var result = await _servico.AutenticarAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(tokenEsperado, result.Token);
    }
    [Fact]
    public async Task Dado_UsuarioClienteComCredenciaisValidas_Quando_AutenticarAsync_Entao_RetornaToken()
    {
        // Arrange
        var request = AutenticacaoFixture.CriarAutenticacaoRequestValida();
        var cliente = ClienteFixture.CriarClienteValido();
        var usuario = AutenticacaoFixture.CriarUsuarioAtivo();

        usuario.Email = request.Email;
        usuario.Senha = "hash_criptografada";
        usuario.TipoUsuario = TipoUsuario.Cliente;
        usuario.ClienteId = cliente.Id;

        const string tokenEsperado = "token_gerado_cliente";

        _usuarioServicoMock.Setup(s => s.ObterPorEmailAsync(request.Email)).ReturnsAsync(usuario);
        _servicoSenhaMock.Setup(s => s.VerificarSenha(request.Senha, usuario.Senha)).Returns(true);
        _clienteServicoMock.Setup(c => c.ObterPorIdAsync(cliente.Id)).ReturnsAsync(new ClienteResponse
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            Documento = cliente.Documento,
            DataCadastro = DateTime.UtcNow.AddDays(-1).ToString()
        });

        _servicoJwtMock.Setup(j => j.GerarToken(
            usuario.Id,
            usuario.Email,
            usuario.TipoUsuario.ToString(),
            cliente.Nome,
            It.Is<IEnumerable<string>>(p => p.Contains("cliente"))))
            .Returns(tokenEsperado);

        _usuarioServicoMock.Setup(s => s.AtualizarAsync(usuario.Id, It.IsAny<AtualizarUsuarioRequest>()))
            .ReturnsAsync(new UsuarioResponse
            {
                Id = usuario.Id,
                Email = usuario.Email,
                TipoUsuario = usuario.TipoUsuario,
                DataUltimoAcesso = DateTime.UtcNow,
                Ativo = true,
                DataCadastro = DateTime.UtcNow.AddDays(-1)
            });

        // Act
        var result = await _servico.AutenticarAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(tokenEsperado, result.Token);

        _clienteServicoMock.Verify(c => c.ObterPorIdAsync(cliente.Id), Times.Once);
    }
}