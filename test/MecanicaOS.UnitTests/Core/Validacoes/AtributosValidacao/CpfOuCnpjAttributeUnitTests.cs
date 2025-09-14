using Core.Validacoes.AtributosValidacao;
using FluentAssertions;
using Xunit;

namespace MecanicaOS.UnitTests.Core.Validacoes.AtributosValidacao;

public class CpfOuCnpjAttributeUnitTests
{
    private readonly CpfOuCnpjAttribute _attribute;

    public CpfOuCnpjAttributeUnitTests()
    {
        _attribute = new CpfOuCnpjAttribute();
    }

    [Theory]
    [InlineData("11144477735")]     // CPF válido
    [InlineData("12345678909")]     // CPF válido
    [InlineData("111.444.777-35")]  // CPF válido com formatação
    public void IsValid_ComCpfValido_DeveRetornarTrue(string cpf)
    {
        // Act
        var resultado = _attribute.IsValid(cpf);

        // Assert
        resultado.Should().BeTrue($"CPF {cpf} deveria ser considerado válido pelo atributo");
    }

    [Theory]
    [InlineData("11222333000181")]     // CNPJ válido
    [InlineData("11444777000161")]     // CNPJ válido
    [InlineData("11.222.333/0001-81")] // CNPJ válido com formatação
    public void IsValid_ComCnpjValido_DeveRetornarTrue(string cnpj)
    {
        // Act
        var resultado = _attribute.IsValid(cnpj);

        // Assert
        resultado.Should().BeTrue($"CNPJ {cnpj} deveria ser considerado válido pelo atributo");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void IsValid_ComValorNuloOuVazio_DeveRetornarTrue(string valor)
    {
        // Act
        var resultado = _attribute.IsValid(valor);

        // Assert
        resultado.Should().BeTrue("Valores nulos ou vazios deveriam ser considerados válidos (validação opcional)");
    }

    [Theory]
    [InlineData("11144477736")]     // CPF inválido
    [InlineData("11222333000182")]  // CNPJ inválido
    [InlineData("12345678900")]     // CPF inválido
    [InlineData("11222333000199")]  // CNPJ inválido
    public void IsValid_ComCpfOuCnpjInvalido_DeveRetornarFalse(string documento)
    {
        // Act
        var resultado = _attribute.IsValid(documento);

        // Assert
        resultado.Should().BeFalse($"Documento {documento} deveria ser considerado inválido pelo atributo");
    }

    [Theory]
    [InlineData("00000000000")]     // CPF com todos os dígitos iguais
    [InlineData("11111111111")]     // CPF com todos os dígitos iguais
    [InlineData("00000000000000")]  // CNPJ com todos os dígitos iguais
    [InlineData("11111111111111")]  // CNPJ com todos os dígitos iguais
    public void IsValid_ComDocumentoComTodosDigitosIguais_DeveRetornarFalse(string documento)
    {
        // Act
        var resultado = _attribute.IsValid(documento);

        // Assert
        resultado.Should().BeFalse($"Documento {documento} com todos os dígitos iguais deveria ser inválido");
    }

    [Theory]
    [InlineData("1234567890")]      // 10 dígitos - nem CPF nem CNPJ
    [InlineData("123456789012")]    // 12 dígitos - nem CPF nem CNPJ
    [InlineData("123456789012345")] // 15 dígitos - nem CPF nem CNPJ
    public void IsValid_ComQuantidadeIncorretaDeDigitos_DeveRetornarFalse(string documento)
    {
        // Act
        var resultado = _attribute.IsValid(documento);

        // Assert
        resultado.Should().BeFalse($"Documento {documento} com quantidade incorreta de dígitos deveria ser inválido");
    }

    [Theory]
    [InlineData("abcdefghijk")]      // Apenas letras
    [InlineData("@#$%&*()_+-")]     // Apenas caracteres especiais
    [InlineData("abc123def456")]    // Mistura de letras e números insuficientes
    public void IsValid_ComCaracteresInvalidos_DeveRetornarFalse(string documento)
    {
        // Act
        var resultado = _attribute.IsValid(documento);

        // Assert
        resultado.Should().BeFalse($"Documento {documento} com caracteres inválidos deveria ser inválido");
    }

    [Theory]
    [InlineData("111.444.777-35", "11144477735")]       // CPF formatado vs limpo
    [InlineData("11.222.333/0001-81", "11222333000181")] // CNPJ formatado vs limpo
    public void IsValid_ComDocumentoFormatadoELimpo_DeveRetornarMesmoResultado(string documentoFormatado, string documentoLimpo)
    {
        // Act
        var resultadoFormatado = _attribute.IsValid(documentoFormatado);
        var resultadoLimpo = _attribute.IsValid(documentoLimpo);

        // Assert
        resultadoFormatado.Should().Be(resultadoLimpo, 
            "Documento formatado e limpo deveriam ter o mesmo resultado de validação");
        resultadoFormatado.Should().BeTrue("Ambos os documentos deveriam ser válidos");
    }

    [Fact]
    public void IsValid_ComObjetoNaoString_DeveRetornarTrue()
    {
        // Arrange
        var objeto = new { Propriedade = "valor" };

        // Act
        var resultado = _attribute.IsValid(objeto);

        // Assert
        resultado.Should().BeTrue("Objetos que não são string deveriam ser considerados válidos");
    }

    [Fact]
    public void IsValid_ComNumeroInteiro_DeveRetornarTrue()
    {
        // Arrange
        var numero = 12345;

        // Act
        var resultado = _attribute.IsValid(numero);

        // Assert
        resultado.Should().BeTrue("Números inteiros deveriam ser considerados válidos");
    }

    [Theory]
    [InlineData("111abc444def777ghi35")]      // CPF com caracteres não numéricos
    [InlineData("11abc222def333ghi000jkl181")] // CNPJ com caracteres não numéricos
    public void IsValid_ComDocumentoComCaracteresNaoNumericosValidosAposLimpeza_DeveRetornarTrue(string documento)
    {
        // Act
        var resultado = _attribute.IsValid(documento);

        // Assert
        resultado.Should().BeTrue($"Documento {documento} deveria ser válido após remoção de caracteres não numéricos");
    }

    [Theory]
    [InlineData("52998224725")]     // CPF válido adicional
    [InlineData("60746948000112")]  // CNPJ válido adicional
    public void IsValid_ComDocumentosValidosAdicionais_DeveRetornarTrue(string documento)
    {
        // Act
        var resultado = _attribute.IsValid(documento);

        // Assert
        resultado.Should().BeTrue($"Documento {documento} deveria ser válido");
    }

    [Theory]
    [InlineData("52998224726")]     // CPF inválido (último dígito alterado)
    [InlineData("60746948000113")]  // CNPJ inválido (último dígito alterado)
    public void IsValid_ComDocumentosInvalidosBaseadosEmValidos_DeveRetornarFalse(string documento)
    {
        // Act
        var resultado = _attribute.IsValid(documento);

        // Assert
        resultado.Should().BeFalse($"Documento {documento} deveria ser inválido");
    }
}
