using Xunit;
using FluentAssertions;
using Core.DTOs.Repositories.Usuarios;
using Core.DTOs.Repositories.Autenticacao;
using Core.DTOs.Repositories.Cliente;
using Core.Enumeradores;

namespace MecanicaOS.UnitTests.Core.DTOs.Repositories;

public class UsuarioRepositoryDtoUnitTests
{
    [Fact]
    public void UsuarioRepositoryDto_QuandoCriado_DeveHerdarDeRepositoryDto()
    {
        // Arrange & Act
        var dto = new UsuarioRepositoryDto();

        // Assert
        dto.Should().BeAssignableTo<RepositoryDto>("UsuarioRepositoryDto deve herdar de RepositoryDto");
        dto.Id.Should().Be(Guid.Empty, "Id deve ser vazio por padrão no DTO");
        dto.Ativo.Should().BeFalse("Ativo deve ser false por padrão no DTO");
    }

    [Fact]
    public void UsuarioRepositoryDto_QuandoDefinidoCamposTecnicos_DevePreservarAuditoria()
    {
        // Arrange
        var dto = new UsuarioRepositoryDto();
        var id = Guid.NewGuid();
        var dataCadastro = DateTime.Now;
        var dataAtualizacao = DateTime.Now.AddMinutes(5);

        // Act
        dto.Id = id;
        dto.DataCadastro = dataCadastro;
        dto.DataAtualizacao = dataAtualizacao;
        dto.Ativo = true;

        // Assert
        dto.Id.Should().Be(id, "o ID deve ser preservado corretamente");
        dto.DataCadastro.Should().Be(dataCadastro, "a data de cadastro deve ser preservada");
        dto.DataAtualizacao.Should().Be(dataAtualizacao, "a data de atualização deve ser preservada");
        dto.Ativo.Should().BeTrue("o status ativo deve ser preservado");
    }

    [Fact]
    public void UsuarioRepositoryDto_QuandoDefinidoEmail_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new UsuarioRepositoryDto();
        var emailEsperado = "usuario@teste.com";

        // Act
        dto.Email = emailEsperado;

        // Assert
        dto.Email.Should().Be(emailEsperado, "o email deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void UsuarioRepositoryDto_QuandoDefinidaSenha_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new UsuarioRepositoryDto();
        var senhaEsperada = "senha123";

        // Act
        dto.Senha = senhaEsperada;

        // Assert
        dto.Senha.Should().Be(senhaEsperada, "a senha deve ser armazenada corretamente no DTO");
    }

    [Theory]
    [InlineData(TipoUsuario.Admin)]
    [InlineData(TipoUsuario.Cliente)]
    public void UsuarioRepositoryDto_QuandoDefinidoTipoUsuario_DeveArmazenarCorretamente(TipoUsuario tipoUsuario)
    {
        // Arrange
        var dto = new UsuarioRepositoryDto();

        // Act
        dto.TipoUsuario = tipoUsuario;

        // Assert
        dto.TipoUsuario.Should().Be(tipoUsuario, "o tipo de usuário deve ser armazenado corretamente no DTO");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void UsuarioRepositoryDto_QuandoDefinidoRecebeAlertaEstoque_DeveArmazenarCorretamente(bool recebeAlerta)
    {
        // Arrange
        var dto = new UsuarioRepositoryDto();

        // Act
        dto.RecebeAlertaEstoque = recebeAlerta;

        // Assert
        dto.RecebeAlertaEstoque.Should().Be(recebeAlerta, "a configuração de alerta de estoque deve ser armazenada corretamente no DTO");
    }

    [Fact]
    public void UsuarioRepositoryDto_QuandoDefinidaDataUltimoAcesso_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new UsuarioRepositoryDto();
        var dataEsperada = DateTime.Now;

        // Act
        dto.DataUltimoAcesso = dataEsperada;

        // Assert
        dto.DataUltimoAcesso.Should().Be(dataEsperada, "a data do último acesso deve ser armazenada corretamente no DTO");
    }

    [Fact]
    public void UsuarioRepositoryDto_QuandoDefinidoClienteId_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new UsuarioRepositoryDto();
        var clienteIdEsperado = Guid.NewGuid();

        // Act
        dto.ClienteId = clienteIdEsperado;

        // Assert
        dto.ClienteId.Should().Be(clienteIdEsperado, "o ClienteId deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void UsuarioRepositoryDto_QuandoDefinidoCliente_DeveArmazenarReferencia()
    {
        // Arrange
        var dto = new UsuarioRepositoryDto();
        var clienteDto = new ClienteRepositoryDTO { Nome = "João Silva" };

        // Act
        dto.Cliente = clienteDto;

        // Assert
        dto.Cliente.Should().Be(clienteDto, "a referência do cliente deve ser armazenada corretamente no DTO");
        dto.Cliente.Nome.Should().Be("João Silva", "o nome do cliente deve ser preservado na referência");
    }

    [Fact]
    public void UsuarioRepositoryDto_QuandoClienteIdNulo_DevePermitirValorNulo()
    {
        // Arrange
        var dto = new UsuarioRepositoryDto();

        // Act
        dto.ClienteId = null;

        // Assert
        dto.ClienteId.Should().BeNull("ClienteId pode ser nulo no DTO");
    }

    [Fact]
    public void UsuarioRepositoryDto_QuandoClienteNulo_DevePermitirValorNulo()
    {
        // Arrange
        var dto = new UsuarioRepositoryDto();

        // Act
        dto.Cliente = null;

        // Assert
        dto.Cliente.Should().BeNull("Cliente pode ser nulo no DTO");
    }

    [Fact]
    public void UsuarioRepositoryDto_QuandoDataUltimoAcessoNula_DevePermitirValorNulo()
    {
        // Arrange
        var dto = new UsuarioRepositoryDto();

        // Act
        dto.DataUltimoAcesso = null;

        // Assert
        dto.DataUltimoAcesso.Should().BeNull("DataUltimoAcesso pode ser nula no DTO");
    }

    [Theory]
    [InlineData("admin@empresa.com")]
    [InlineData("cliente@teste.com")]
    [InlineData("funcionario@mecanica.com")]
    public void UsuarioRepositoryDto_QuandoDefinidoEmailComValoresDiferentes_DeveArmazenarCorretamente(string email)
    {
        // Arrange
        var dto = new UsuarioRepositoryDto();

        // Act
        dto.Email = email;

        // Assert
        dto.Email.Should().Be(email, "o email deve ser armazenado independente do conteúdo no DTO");
    }

    [Theory]
    [InlineData("senha123")]
    [InlineData("senhaCompleta@123")]
    [InlineData("minhasenha")]
    public void UsuarioRepositoryDto_QuandoDefinidaSenhaComValoresDiferentes_DeveArmazenarCorretamente(string senha)
    {
        // Arrange
        var dto = new UsuarioRepositoryDto();

        // Act
        dto.Senha = senha;

        // Assert
        dto.Senha.Should().Be(senha, "a senha deve ser armazenada independente do conteúdo no DTO");
    }
}
