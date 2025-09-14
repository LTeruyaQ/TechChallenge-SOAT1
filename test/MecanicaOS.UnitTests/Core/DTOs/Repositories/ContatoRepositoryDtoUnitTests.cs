using Xunit;
using FluentAssertions;
using Core.DTOs.Repositories.Cliente;
using Core.DTOs.Repositories.Autenticacao;

namespace MecanicaOS.UnitTests.Core.DTOs.Repositories;

public class ContatoRepositoryDtoUnitTests
{
    [Fact]
    public void ContatoRepositoryDTO_QuandoCriado_DeveHerdarDeRepositoryDto()
    {
        // Arrange & Act
        var dto = new ContatoRepositoryDTO();

        // Assert
        dto.Should().BeAssignableTo<RepositoryDto>("ContatoRepositoryDTO deve herdar de RepositoryDto");
        dto.Id.Should().Be(Guid.Empty, "Id deve ser vazio por padrão no DTO");
        dto.Ativo.Should().BeFalse("Ativo deve ser false por padrão no DTO");
    }

    [Fact]
    public void ContatoRepositoryDTO_QuandoDefinidoCamposTecnicos_DevePreservarAuditoria()
    {
        // Arrange
        var dto = new ContatoRepositoryDTO();
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
    public void ContatoRepositoryDTO_QuandoDefinidoTelefone_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new ContatoRepositoryDTO();
        var telefoneEsperado = "(11) 99999-9999";

        // Act
        dto.Telefone = telefoneEsperado;

        // Assert
        dto.Telefone.Should().Be(telefoneEsperado, "o telefone deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void ContatoRepositoryDTO_QuandoDefinidoEmail_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new ContatoRepositoryDTO();
        var emailEsperado = "cliente@exemplo.com";

        // Act
        dto.Email = emailEsperado;

        // Assert
        dto.Email.Should().Be(emailEsperado, "o email deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void ContatoRepositoryDTO_QuandoDefinidoIdCliente_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new ContatoRepositoryDTO();
        var idClienteEsperado = Guid.NewGuid();

        // Act
        dto.IdCliente = idClienteEsperado;

        // Assert
        dto.IdCliente.Should().Be(idClienteEsperado, "o IdCliente deve ser armazenado corretamente no DTO");
    }

    [Theory]
    [InlineData("(11) 99999-9999")]
    [InlineData("11999999999")]
    [InlineData("+55 11 99999-9999")]
    public void ContatoRepositoryDTO_QuandoDefinidoTelefoneComFormatosDiferentes_DeveArmazenarCorretamente(string telefone)
    {
        // Arrange
        var dto = new ContatoRepositoryDTO();

        // Act
        dto.Telefone = telefone;

        // Assert
        dto.Telefone.Should().Be(telefone, "o telefone deve ser armazenado independente do formato no DTO");
    }

    [Theory]
    [InlineData("usuario@dominio.com")]
    [InlineData("teste.email@empresa.com.br")]
    [InlineData("contato+tag@exemplo.org")]
    public void ContatoRepositoryDTO_QuandoDefinidoEmailComFormatosDiferentes_DeveArmazenarCorretamente(string email)
    {
        // Arrange
        var dto = new ContatoRepositoryDTO();

        // Act
        dto.Email = email;

        // Assert
        dto.Email.Should().Be(email, "o email deve ser armazenado independente do formato no DTO");
    }

    [Fact]
    public void ContatoRepositoryDTO_QuandoDefinidoClienteReferencia_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new ContatoRepositoryDTO();
        var clienteDto = new ClienteRepositoryDTO { Nome = "João Silva" };

        // Act
        dto.Cliente = clienteDto;

        // Assert
        dto.Cliente.Should().Be(clienteDto, "a referência do cliente deve ser armazenada corretamente no DTO");
        dto.Cliente.Nome.Should().Be("João Silva", "o nome do cliente deve ser preservado na referência");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ContatoRepositoryDTO_QuandoDefinidoTelefoneVazioOuNulo_DeveArmazenarCorretamente(string? telefone)
    {
        // Arrange
        var dto = new ContatoRepositoryDTO();

        // Act
        dto.Telefone = telefone;

        // Assert
        dto.Telefone.Should().Be(telefone, "o telefone deve aceitar valores vazios ou nulos no DTO");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ContatoRepositoryDTO_QuandoDefinidoEmailVazioOuNulo_DeveArmazenarCorretamente(string? email)
    {
        // Arrange
        var dto = new ContatoRepositoryDTO();

        // Act
        dto.Email = email;

        // Assert
        dto.Email.Should().Be(email, "o email deve aceitar valores vazios ou nulos no DTO");
    }
}
