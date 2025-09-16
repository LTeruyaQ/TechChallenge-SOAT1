using Core.Entidades;
using Core.Enumeradores;
using Core.Exceptions;
using MecanicaOS.UnitTests.Fixtures.UseCases;

namespace MecanicaOS.UnitTests.Core.UseCases;

public class UsuarioUseCasesUnitTests
{
    private readonly UsuarioUseCasesFixture _fixture;

    public UsuarioUseCasesUnitTests()
    {
        _fixture = new UsuarioUseCasesFixture();
    }

    [Fact]
    public async Task CadastrarUseCaseAsync_ComUsuarioAdministrador_DeveRetornarUsuarioCadastrado()
    {
        // Arrange
        var mockUsuarioGateway = UsuarioUseCasesFixture.CriarMockUsuarioGateway();
        var mockServicoSenha = UsuarioUseCasesFixture.CriarMockServicoSenha();
        var mockUdt = UsuarioUseCasesFixture.CriarMockUnidadeDeTrabalho();

        var request = UsuarioUseCasesFixture.CriarCadastrarUsuarioAdministradorDto();
        var usuarioEsperado = UsuarioUseCasesFixture.CriarUsuarioAdministradorValido();

        _fixture.ConfigurarMockUsuarioGatewayParaEmailNaoEncontrado(mockUsuarioGateway, request.Email);
        _fixture.ConfigurarMockUsuarioGatewayParaCadastro(mockUsuarioGateway, usuarioEsperado);
        _fixture.ConfigurarMockServicoSenha(mockServicoSenha, request.Senha, "senhaHasheada123");
        mockUdt.Commit().Returns(true);

        var usuarioUseCases = _fixture.CriarUsuarioUseCases(
            mockUsuarioGateway, null, mockServicoSenha, mockUdt);

        // Act
        var resultado = await usuarioUseCases.CadastrarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Email.Should().Be(request.Email);
        resultado.TipoUsuario.Should().Be(request.TipoUsuario);
        resultado.RecebeAlertaEstoque.Should().Be(request.RecebeAlertaEstoque.Value);
        resultado.Senha.Should().BeNull(); // Senha deve ser removida no retorno

        await mockUsuarioGateway.Received(1).ObterPorEmailAsync(request.Email);
        await mockUsuarioGateway.Received(1).CadastrarAsync(Arg.Any<Usuario>());
        mockServicoSenha.Received(1).CriptografarSenha(request.Senha);
        await mockUdt.Received(1).Commit();
    }

    [Fact]
    public async Task CadastrarUseCaseAsync_ComUsuarioCliente_DeveVincularClienteId()
    {
        // Arrange
        var mockUsuarioGateway = UsuarioUseCasesFixture.CriarMockUsuarioGateway();
        var mockClienteUseCases = UsuarioUseCasesFixture.CriarMockClienteUseCases();
        var mockServicoSenha = UsuarioUseCasesFixture.CriarMockServicoSenha();
        var mockUdt = UsuarioUseCasesFixture.CriarMockUnidadeDeTrabalho();

        var request = UsuarioUseCasesFixture.CriarCadastrarUsuarioClienteDto();
        var clienteExistente = UsuarioUseCasesFixture.CriarClienteParaUsuario();
        var usuarioEsperado = UsuarioUseCasesFixture.CriarUsuarioClienteValido();

        _fixture.ConfigurarMockUsuarioGatewayParaEmailNaoEncontrado(mockUsuarioGateway, request.Email);
        _fixture.ConfigurarMockUsuarioGatewayParaCadastro(mockUsuarioGateway, usuarioEsperado);
        _fixture.ConfigurarMockClienteUseCasesParaDocumento(mockClienteUseCases, request.Documento!, clienteExistente);
        _fixture.ConfigurarMockServicoSenha(mockServicoSenha, request.Senha, "senhaHasheada789");
        mockUdt.Commit().Returns(Task.FromResult(true));

        var usuarioUseCases = _fixture.CriarUsuarioUseCases(
            mockUsuarioGateway, mockClienteUseCases, mockServicoSenha, mockUdt);

        // Act
        var resultado = await usuarioUseCases.CadastrarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.TipoUsuario.Should().Be(TipoUsuario.Cliente);
        resultado.ClienteId.Should().NotBeNull();

        await mockClienteUseCases.Received(1).ObterPorDocumentoUseCaseAsync(request.Documento!);
    }

    [Fact]
    public async Task CadastrarUseCaseAsync_ComEmailJaCadastrado_DeveLancarDadosJaCadastradosException()
    {
        // Arrange
        var mockUsuarioGateway = UsuarioUseCasesFixture.CriarMockUsuarioGateway();
        var request = UsuarioUseCasesFixture.CriarCadastrarUsuarioAdministradorDto();

        _fixture.ConfigurarMockUsuarioGatewayParaEmailJaCadastrado(mockUsuarioGateway, request.Email);

        var usuarioUseCases = _fixture.CriarUsuarioUseCases(mockUsuarioGateway);

        // Act & Assert
        await usuarioUseCases
            .Invoking(x => x.CadastrarUseCaseAsync(request))
            .Should()
            .ThrowAsync<DadosJaCadastradosException>()
            .WithMessage("Usuário já cadastrado");

        await mockUsuarioGateway.Received(1).ObterPorEmailAsync(request.Email);
        await mockUsuarioGateway.DidNotReceive().CadastrarAsync(Arg.Any<Usuario>());
    }

    [Fact]
    public async Task CadastrarUseCaseAsync_ComUsuarioClienteSemDocumento_DeveLancarDadosInvalidosException()
    {
        // Arrange
        var mockUsuarioGateway = UsuarioUseCasesFixture.CriarMockUsuarioGateway();
        var request = UsuarioUseCasesFixture.CriarCadastrarUsuarioClienteDto();
        request.Documento = null; // Remove o documento

        _fixture.ConfigurarMockUsuarioGatewayParaEmailNaoEncontrado(mockUsuarioGateway, request.Email);

        var usuarioUseCases = _fixture.CriarUsuarioUseCases(mockUsuarioGateway);

        // Act & Assert
        await usuarioUseCases
            .Invoking(x => x.CadastrarUseCaseAsync(request))
            .Should()
            .ThrowAsync<DadosInvalidosException>()
            .WithMessage("Usuários do tipo cliente devem informar o documento.");
    }

    [Fact]
    public async Task AtualizarUseCaseAsync_ComDadosValidos_DeveRetornarUsuarioAtualizado()
    {
        // Arrange
        var mockUsuarioGateway = UsuarioUseCasesFixture.CriarMockUsuarioGateway();
        var mockServicoSenha = UsuarioUseCasesFixture.CriarMockServicoSenha();
        var mockUdt = UsuarioUseCasesFixture.CriarMockUnidadeDeTrabalho();

        var usuarioExistente = UsuarioUseCasesFixture.CriarUsuarioFuncionarioValido();
        var request = UsuarioUseCasesFixture.CriarAtualizarUsuarioUseCaseDtoValido();

        _fixture.ConfigurarMockUsuarioGatewayParaAtualizacao(mockUsuarioGateway, usuarioExistente);
        _fixture.ConfigurarMockServicoSenha(mockServicoSenha, request.Senha!, "novaSenhaHasheada");
        mockUdt.Commit().Returns(Task.FromResult(true));

        var usuarioUseCases = _fixture.CriarUsuarioUseCases(
            mockUsuarioGateway, null, mockServicoSenha, mockUdt);

        // Act
        var resultado = await usuarioUseCases.AtualizarUseCaseAsync(usuarioExistente.Id, request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(usuarioExistente.Id);
        resultado.Email.Should().Be(request.Email);
        resultado.TipoUsuario.Should().Be(request.TipoUsuario);
        resultado.RecebeAlertaEstoque.Should().Be(request.RecebeAlertaEstoque.Value);
        resultado.Senha.Should().BeNull(); // Senha deve ser removida no retorno

        await mockUsuarioGateway.Received(1).ObterPorIdAsync(usuarioExistente.Id);
        await mockUsuarioGateway.Received(1).EditarAsync(Arg.Any<Usuario>());
        mockServicoSenha.Received(1).CriptografarSenha(request.Senha!);
        mockUdt.Received(1).Commit();
    }

    [Fact]
    public async Task AtualizarUseCaseAsync_SemNovaSenha_DeveManterSenhaExistente()
    {
        // Arrange
        var mockUsuarioGateway = UsuarioUseCasesFixture.CriarMockUsuarioGateway();
        var mockServicoSenha = UsuarioUseCasesFixture.CriarMockServicoSenha();
        var mockUdt = UsuarioUseCasesFixture.CriarMockUnidadeDeTrabalho();

        var usuarioExistente = UsuarioUseCasesFixture.CriarUsuarioAdministradorValido();
        var request = UsuarioUseCasesFixture.CriarAtualizarUsuarioSemSenhaDto();

        _fixture.ConfigurarMockUsuarioGatewayParaAtualizacao(mockUsuarioGateway, usuarioExistente);
        mockUdt.Commit().Returns(Task.FromResult(true));

        var usuarioUseCases = _fixture.CriarUsuarioUseCases(
            mockUsuarioGateway, null, mockServicoSenha, mockUdt);

        // Act
        var resultado = await usuarioUseCases.AtualizarUseCaseAsync(usuarioExistente.Id, request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Email.Should().Be(request.Email);

        mockServicoSenha.DidNotReceive().CriptografarSenha(Arg.Any<string>());
    }

    [Fact]
    public async Task AtualizarUseCaseAsync_ComUsuarioInexistente_DeveLancarDadosNaoEncontradosException()
    {
        // Arrange
        var mockUsuarioGateway = UsuarioUseCasesFixture.CriarMockUsuarioGateway();
        var usuarioId = Guid.NewGuid();
        var request = UsuarioUseCasesFixture.CriarAtualizarUsuarioUseCaseDtoValido();

        _fixture.ConfigurarMockUsuarioGatewayParaUsuarioNaoEncontrado(mockUsuarioGateway, usuarioId);

        var usuarioUseCases = _fixture.CriarUsuarioUseCases(mockUsuarioGateway);

        // Act & Assert
        await usuarioUseCases
            .Invoking(x => x.AtualizarUseCaseAsync(usuarioId, request))
            .Should()
            .ThrowAsync<DadosNaoEncontradosException>()
            .WithMessage("Usuário não encontrado");

        await mockUsuarioGateway.Received(1).ObterPorIdAsync(usuarioId);
        await mockUsuarioGateway.DidNotReceive().EditarAsync(Arg.Any<Usuario>());
    }

    [Fact]
    public async Task ObterPorIdUseCaseAsync_ComIdValido_DeveRetornarUsuarioSemSenha()
    {
        // Arrange
        var mockUsuarioGateway = UsuarioUseCasesFixture.CriarMockUsuarioGateway();
        var usuarioExistente = UsuarioUseCasesFixture.CriarUsuarioAdministradorValido();

        mockUsuarioGateway.ObterPorIdAsync(usuarioExistente.Id).Returns(usuarioExistente);

        var usuarioUseCases = _fixture.CriarUsuarioUseCases(mockUsuarioGateway);

        // Act
        var resultado = await usuarioUseCases.ObterPorIdUseCaseAsync(usuarioExistente.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(usuarioExistente.Id);
        resultado.Email.Should().Be(usuarioExistente.Email);
        resultado.Senha.Should().BeNull(); // Senha deve ser removida

        await mockUsuarioGateway.Received(1).ObterPorIdAsync(usuarioExistente.Id);
    }

    [Fact]
    public async Task ObterTodosUseCaseAsync_DeveRetornarListaDeUsuariosSemSenhas()
    {
        // Arrange
        var mockUsuarioGateway = UsuarioUseCasesFixture.CriarMockUsuarioGateway();
        var usuariosEsperados = UsuarioUseCasesFixture.CriarListaUsuariosVariados();

        _fixture.ConfigurarMockUsuarioGatewayParaListagem(mockUsuarioGateway, usuariosEsperados);

        var usuarioUseCases = _fixture.CriarUsuarioUseCases(mockUsuarioGateway);

        // Act
        var resultado = await usuarioUseCases.ObterTodosUseCaseAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(usuariosEsperados.Count);
        resultado.Should().OnlyContain(u => u.Senha == null); // Todas as senhas devem ser removidas

        await mockUsuarioGateway.Received(1).ObterTodosAsync();
    }

    [Fact]
    public async Task ObterPorEmailUseCaseAsync_ComEmailExistente_DeveRetornarUsuario()
    {
        // Arrange
        var mockUsuarioGateway = UsuarioUseCasesFixture.CriarMockUsuarioGateway();
        var usuarioExistente = UsuarioUseCasesFixture.CriarUsuarioFuncionarioValido();

        mockUsuarioGateway.ObterPorEmailAsync(usuarioExistente.Email).Returns(usuarioExistente);

        var usuarioUseCases = _fixture.CriarUsuarioUseCases(mockUsuarioGateway);

        // Act
        var resultado = await usuarioUseCases.ObterPorEmailUseCaseAsync(usuarioExistente.Email);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Email.Should().Be(usuarioExistente.Email);
        resultado.Id.Should().Be(usuarioExistente.Id);

        await mockUsuarioGateway.Received(1).ObterPorEmailAsync(usuarioExistente.Email);
    }

    [Fact]
    public async Task ObterPorEmailUseCaseAsync_ComEmailInexistente_DeveRetornarNull()
    {
        // Arrange
        var mockUsuarioGateway = UsuarioUseCasesFixture.CriarMockUsuarioGateway();
        var emailInexistente = "inexistente@email.com";

        _fixture.ConfigurarMockUsuarioGatewayParaEmailNaoEncontrado(mockUsuarioGateway, emailInexistente);

        var usuarioUseCases = _fixture.CriarUsuarioUseCases(mockUsuarioGateway);

        // Act
        var resultado = await usuarioUseCases.ObterPorEmailUseCaseAsync(emailInexistente);

        // Assert
        resultado.Should().BeNull();
        await mockUsuarioGateway.Received(1).ObterPorEmailAsync(emailInexistente);
    }

    [Fact]
    public async Task DeletarUseCaseAsync_ComUsuarioExistente_DeveRetornarTrue()
    {
        // Arrange
        var mockUsuarioGateway = UsuarioUseCasesFixture.CriarMockUsuarioGateway();
        var mockUdt = UsuarioUseCasesFixture.CriarMockUnidadeDeTrabalho();
        var usuarioExistente = UsuarioUseCasesFixture.CriarUsuarioFuncionarioValido();

        _fixture.ConfigurarMockUsuarioGatewayParaDelecao(mockUsuarioGateway, usuarioExistente);
        mockUdt.Commit().Returns(Task.FromResult(true));

        var usuarioUseCases = _fixture.CriarUsuarioUseCases(
            mockUsuarioGateway, null, null, mockUdt);

        // Act
        var resultado = await usuarioUseCases.DeletarUseCaseAsync(usuarioExistente.Id);

        // Assert
        resultado.Should().BeTrue();

        await mockUsuarioGateway.Received(1).ObterPorIdAsync(usuarioExistente.Id);
        await mockUsuarioGateway.Received(1).DeletarAsync(usuarioExistente);
        mockUdt.Received(1).Commit();
    }

    [Fact]
    public async Task DeletarUseCaseAsync_ComUsuarioInexistente_DeveLancarDadosNaoEncontradosException()
    {
        // Arrange
        var mockUsuarioGateway = UsuarioUseCasesFixture.CriarMockUsuarioGateway();
        var usuarioId = Guid.NewGuid();

        _fixture.ConfigurarMockUsuarioGatewayParaUsuarioNaoEncontrado(mockUsuarioGateway, usuarioId);

        var usuarioUseCases = _fixture.CriarUsuarioUseCases(mockUsuarioGateway);

        // Act & Assert
        await usuarioUseCases
            .Invoking(x => x.DeletarUseCaseAsync(usuarioId))
            .Should()
            .ThrowAsync<DadosNaoEncontradosException>()
            .WithMessage("Usuário não encontrado");

        await mockUsuarioGateway.Received(1).ObterPorIdAsync(usuarioId);
        await mockUsuarioGateway.DidNotReceive().DeletarAsync(Arg.Any<Usuario>());
    }

    [Theory]
    [InlineData(TipoUsuario.Admin)]
    [InlineData(TipoUsuario.Cliente)]
    public async Task CadastrarUseCaseAsync_ComDiferentesTiposUsuario_DeveCadastrarCorretamente(TipoUsuario tipoUsuario)
    {
        // Arrange
        var mockUsuarioGateway = UsuarioUseCasesFixture.CriarMockUsuarioGateway();
        var mockClienteUseCases = UsuarioUseCasesFixture.CriarMockClienteUseCases();
        var mockServicoSenha = UsuarioUseCasesFixture.CriarMockServicoSenha();
        var mockUdt = UsuarioUseCasesFixture.CriarMockUnidadeDeTrabalho();

        var request = UsuarioUseCasesFixture.CriarCadastrarUsuarioAdministradorDto();
        request.TipoUsuario = tipoUsuario;

        if (tipoUsuario == TipoUsuario.Cliente)
        {
            request.Documento = "12345678901";
            var cliente = UsuarioUseCasesFixture.CriarClienteParaUsuario();
            _fixture.ConfigurarMockClienteUseCasesParaDocumento(mockClienteUseCases, request.Documento, cliente);
        }

        _fixture.ConfigurarMockUsuarioGatewayParaEmailNaoEncontrado(mockUsuarioGateway, request.Email);
        _fixture.ConfigurarMockUsuarioGatewayParaCadastro(mockUsuarioGateway);
        _fixture.ConfigurarMockServicoSenha(mockServicoSenha);
        mockUdt.Commit().Returns(Task.FromResult(true));

        var usuarioUseCases = _fixture.CriarUsuarioUseCases(
            mockUsuarioGateway, mockClienteUseCases, mockServicoSenha, mockUdt);

        // Act
        var resultado = await usuarioUseCases.CadastrarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.TipoUsuario.Should().Be(tipoUsuario);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public async Task CadastrarUseCaseAsync_ComDiferentesRecebeAlertaEstoque_DeveCadastrarCorretamente(bool? recebeAlerta)
    {
        // Arrange
        var mockUsuarioGateway = UsuarioUseCasesFixture.CriarMockUsuarioGateway();
        var mockServicoSenha = UsuarioUseCasesFixture.CriarMockServicoSenha();
        var mockUdt = UsuarioUseCasesFixture.CriarMockUnidadeDeTrabalho();

        var request = UsuarioUseCasesFixture.CriarCadastrarUsuarioAdministradorDto();
        request.RecebeAlertaEstoque = recebeAlerta;

        _fixture.ConfigurarMockUsuarioGatewayParaEmailNaoEncontrado(mockUsuarioGateway, request.Email);
        _fixture.ConfigurarMockUsuarioGatewayParaCadastro(mockUsuarioGateway);
        _fixture.ConfigurarMockServicoSenha(mockServicoSenha);
        mockUdt.Commit().Returns(Task.FromResult(true));

        var usuarioUseCases = _fixture.CriarUsuarioUseCases(
            mockUsuarioGateway, null, mockServicoSenha, mockUdt);

        // Act
        var resultado = await usuarioUseCases.CadastrarUseCaseAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.RecebeAlertaEstoque.Should().Be(recebeAlerta ?? false);
    }

    [Fact]
    public void Constructor_ComParametrosNulos_DeveLancarArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            _fixture.CriarUsuarioUseCases(null));
    }
}
