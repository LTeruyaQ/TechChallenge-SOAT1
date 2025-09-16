using Xunit;
using FluentAssertions;
using Core.DTOs.Entidades.Cliente;
using Core.DTOs.Entidades.Autenticacao;

namespace MecanicaOS.UnitTests.Core.DTOs.Entidades;

public class EnderecoEntityDtoUnitTests
{
    [Fact]
    public void EnderecoEntityDto_QuandoCriado_DeveHerdarDeEntityDto()
    {
        // Arrange & Act
        var dto = new EnderecoEntityDto();

        // Assert
        dto.Should().BeAssignableTo<EntityDto>("EnderecoEntityDto deve herdar de EntityDto");
        dto.Id.Should().Be(Guid.Empty, "Id deve ser vazio por padrão no DTO");
        dto.Ativo.Should().BeFalse("Ativo deve ser false por padrão no DTO");
    }

    [Fact]
    public void EnderecoEntityDto_QuandoDefinidoCamposTecnicos_DevePreservarAuditoria()
    {
        // Arrange
        var dto = new EnderecoEntityDto();
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
    public void EnderecoEntityDto_QuandoDefinidaRua_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new EnderecoEntityDto();
        var ruaEsperada = "Rua das Flores, 123";

        // Act
        dto.Rua = ruaEsperada;

        // Assert
        dto.Rua.Should().Be(ruaEsperada, "a rua deve ser armazenada corretamente no DTO");
    }

    [Fact]
    public void EnderecoEntityDto_QuandoDefinidoBairro_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new EnderecoEntityDto();
        var bairroEsperado = "Centro";

        // Act
        dto.Bairro = bairroEsperado;

        // Assert
        dto.Bairro.Should().Be(bairroEsperado, "o bairro deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void EnderecoEntityDto_QuandoDefinidaCidade_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new EnderecoEntityDto();
        var cidadeEsperada = "São Paulo";

        // Act
        dto.Cidade = cidadeEsperada;

        // Assert
        dto.Cidade.Should().Be(cidadeEsperada, "a cidade deve ser armazenada corretamente no DTO");
    }

    [Fact]
    public void EnderecoEntityDto_QuandoDefinidoNumero_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new EnderecoEntityDto();
        var numeroEsperado = "123";

        // Act
        dto.Numero = numeroEsperado;

        // Assert
        dto.Numero.Should().Be(numeroEsperado, "o número deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void EnderecoEntityDto_QuandoDefinidoCEP_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new EnderecoEntityDto();
        var cepEsperado = "01234-567";

        // Act
        dto.CEP = cepEsperado;

        // Assert
        dto.CEP.Should().Be(cepEsperado, "o CEP deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void EnderecoEntityDto_QuandoDefinidoComplemento_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new EnderecoEntityDto();
        var complementoEsperado = "Apto 101";

        // Act
        dto.Complemento = complementoEsperado;

        // Assert
        dto.Complemento.Should().Be(complementoEsperado, "o complemento deve ser armazenado corretamente no DTO");
    }

    [Fact]
    public void EnderecoEntityDto_QuandoDefinidoIdCliente_DeveArmazenarCorretamente()
    {
        // Arrange
        var dto = new EnderecoEntityDto();
        var idClienteEsperado = Guid.NewGuid();

        // Act
        dto.IdCliente = idClienteEsperado;

        // Assert
        dto.IdCliente.Should().Be(idClienteEsperado, "o IdCliente deve ser armazenado corretamente no DTO");
    }

    [Theory]
    [InlineData("01234-567")]
    [InlineData("12345678")]
    [InlineData("98765-432")]
    public void EnderecoEntityDto_QuandoDefinidoCEPComFormatosDiferentes_DeveArmazenarCorretamente(string cep)
    {
        // Arrange
        var dto = new EnderecoEntityDto();

        // Act
        dto.CEP = cep;

        // Assert
        dto.CEP.Should().Be(cep, "o CEP deve ser armazenado independente do formato");
    }

    [Theory]
    [InlineData("123")]
    [InlineData("456A")]
    [InlineData("S/N")]
    public void EnderecoEntityDto_QuandoDefinidoNumeroComFormatosDiferentes_DeveArmazenarCorretamente(string numero)
    {
        // Arrange
        var dto = new EnderecoEntityDto();

        // Act
        dto.Numero = numero;

        // Assert
        dto.Numero.Should().Be(numero, "o número deve ser armazenado independente do formato");
    }
}
