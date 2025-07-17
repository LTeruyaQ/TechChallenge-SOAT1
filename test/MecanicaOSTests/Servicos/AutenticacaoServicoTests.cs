using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aplicacao.DTOs.Requests.Autenticacao;
using Aplicacao.DTOs.Requests.Usuario;
using Aplicacao.DTOs.Responses.Autenticacao;
using Aplicacao.DTOs.Responses.Cliente;
using Aplicacao.DTOs.Responses.Usuario;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.Servicos;
using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Exceptions;
using Dominio.Interfaces.Servicos;
using FluentAssertions;
using MecanicaOSTests.Fixtures;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

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

        // Act
        Func<Task> act = async () => await _servico.AutenticarAsync(request);
        
        // Assert
        await act.Should()
            .ThrowAsync<CredenciaisInvalidasException>()
            .WithMessage("Credenciais inválidas");
            
        _logServicoMock.Verify(
            x => x.LogErro("AutenticarAsync", It.IsAny<Exception>()),
            Times.Once,
            "porque deve registrar o erro no log");
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

        // Act
        Func<Task> act = async () => await _servico.AutenticarAsync(request);
        
        // Assert
        await act.Should()
            .ThrowAsync<UsuarioInativoException>()
            .WithMessage("Usuário inativo");
    }

    [Fact]
    public async Task Dado_SenhaIncorreta_Quando_AutenticarAsync_Entao_LancaExcecaoCredenciaisInvalidas()
    {
        // Arrange
        var request = AutenticacaoFixture.CriarAutenticacaoRequestValida();
        var usuario = AutenticacaoFixture.CriarUsuarioAtivo();
        usuario.Email = request.Email;
        usuario.Senha = "hash_correta";

        _usuarioServicoMock.Setup(s => s.ObterPorEmailAsync(request.Email))
            .ReturnsAsync(usuario);
        _servicoSenhaMock.Setup(s => s.VerificarSenha(request.Senha, usuario.Senha))
            .Returns(false);

        // Act
        Func<Task> act = async () => await _servico.AutenticarAsync(request);
        
        // Assert
        await act.Should()
            .ThrowAsync<CredenciaisInvalidasException>()
            .WithMessage("Credenciais inválidas");
            
        _logServicoMock.Verify(
            x => x.LogErro("AutenticarAsync", It.IsAny<Exception>()),
            Times.Once,
            "porque deve registrar o erro no log");
    }

    [Fact]
    public async Task Dado_UsuarioClienteSemClienteId_Quando_AutenticarAsync_Entao_LancaExcecaoDadosInvalidos()
    {
        // Arrange
        var request = AutenticacaoFixture.CriarAutenticacaoRequestValida();
        var usuario = AutenticacaoFixture.CriarUsuarioAtivo();
        usuario.Email = request.Email;
        usuario.Senha = "hash_correta";
        usuario.TipoUsuario = TipoUsuario.Cliente;
        usuario.ClienteId = null;

        _usuarioServicoMock.Setup(s => s.ObterPorEmailAsync(request.Email))
            .ReturnsAsync(usuario);
        _servicoSenhaMock.Setup(s => s.VerificarSenha(request.Senha, usuario.Senha))
            .Returns(true);

        // Act
        Func<Task> act = async () => await _servico.AutenticarAsync(request);
        
        // Assert
        await act.Should()
            .ThrowAsync<DadosInvalidosException>()
            .WithMessage("Dados inválidos");
    }

    [Fact]
    public async Task Dado_UsuarioAdminComCredenciaisValidas_Quando_AutenticarAsync_Entao_RetornaToken()
    {
        // Arrange
        var request = AutenticacaoFixture.CriarAutenticacaoRequestValida();
        var usuario = AutenticacaoFixture.CriarUsuarioAdmin();
        usuario.Email = request.Email;
        usuario.Senha = "hash_correta";
        var tokenEsperado = "token_jwt_gerado";

        _usuarioServicoMock.Setup(s => s.ObterPorEmailAsync(request.Email))
            .ReturnsAsync(usuario);
        _servicoSenhaMock.Setup(s => s.VerificarSenha(request.Senha, usuario.Senha))
            .Returns(true);
        _servicoJwtMock.Setup(s => s.GerarToken(usuario))
            .Returns(tokenEsperado);

        // Act
        var resultado = await _servico.AutenticarAsync(request);
        
        // Assert
        resultado.Should().NotBeNull("porque a autenticação deve retornar um token");
        resultado.Token.Should().Be(tokenEsperado, "porque deve conter o token JWT gerado");
        resultado.Usuario.Email.Should().Be(usuario.Email, "porque deve conter o email do usuário");
        resultado.Usuario.TipoUsuario.Should().Be(usuario.TipoUsuario, "porque deve conter o tipo de usuário");
    }

    [Fact]
    public async Task Dado_UsuarioClienteComCredenciaisValidas_Quando_AutenticarAsync_Entao_RetornaTokenComDadosCliente()
    {
        // Arrange
        var request = AutenticacaoFixture.CriarAutenticacaoRequestValida();
        var usuario = AutenticacaoFixture.CriarUsuarioAtivo();
        var cliente = AutenticacaoFixture.CriarClienteValido();
        usuario.Email = request.Email;
        usuario.Senha = "hash_correta";
        usuario.TipoUsuario = TipoUsuario.Cliente;
        usuario.ClienteId = cliente.Id;
        var tokenEsperado = "token_jwt_gerado";

        _usuarioServicoMock.Setup(s => s.ObterPorEmailAsync(request.Email))
            .ReturnsAsync(usuario);
        _servicoSenhaMock.Setup(s => s.VerificarSenha(request.Senha, usuario.Senha))
            .Returns(true);
        _servicoJwtMock.Setup(s => s.GerarToken(usuario))
            .Returns(tokenEsperado);
        _clienteServicoMock.Setup(s => s.ObterPorIdAsync(cliente.Id))
            .ReturnsAsync(new ClienteResponse { Id = cliente.Id, Nome = cliente.Nome });

        // Act
        var resultado = await _servico.AutenticarAsync(request);
        
        // Assert
        resultado.Should().NotBeNull("porque a autenticação deve retornar um token");
        resultado.Token.Should().Be(tokenEsperado, "porque deve conter o token JWT gerado");
        resultado.Usuario.Email.Should().Be(usuario.Email, "porque deve conter o email do usuário");
        resultado.Usuario.TipoUsuario.Should().Be(usuario.TipoUsuario, "porque deve conter o tipo de usuário");
        resultado.Cliente.Should().NotBeNull("porque usuários clientes devem ter dados do cliente");
        resultado.Cliente.Id.Should().Be(cliente.Id, "porque deve conter o ID do cliente");
    }
}
