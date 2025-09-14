using Core.Validacoes;
using FluentAssertions;
using Xunit;

namespace MecanicaOS.UnitTests.Core.Validacoes;

public class ValidadorCnpjUnitTests
{
    [Theory]
    [InlineData("11222333000181")]  // CNPJ válido
    [InlineData("11444777000161")]  // CNPJ válido
    [InlineData("34028316000103")]  // CNPJ válido
    [InlineData("11.222.333/0001-81")]  // CNPJ válido com formatação
    [InlineData("11.444.777/0001-61")]  // CNPJ válido com formatação
    [InlineData("34.028.316/0001-03")]  // CNPJ válido com formatação
    public void Valido_ComCnpjValido_DeveRetornarTrue(string cnpj)
    {
        // Act
        var resultado = ValidadorCnpj.Valido(cnpj);

        // Assert
        resultado.Should().BeTrue($"CNPJ {cnpj} deveria ser válido");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Valido_ComCnpjNuloOuVazio_DeveRetornarFalse(string cnpj)
    {
        // Act
        var resultado = ValidadorCnpj.Valido(cnpj);

        // Assert
        resultado.Should().BeFalse("CNPJ nulo ou vazio deveria ser inválido");
    }

    [Theory]
    [InlineData("1122233300018")]    // 13 dígitos
    [InlineData("112223330001811")]  // 15 dígitos
    [InlineData("1122233")]          // 7 dígitos
    public void Valido_ComCnpjComQuantidadeIncorretaDeDigitos_DeveRetornarFalse(string cnpj)
    {
        // Act
        var resultado = ValidadorCnpj.Valido(cnpj);

        // Assert
        resultado.Should().BeFalse($"CNPJ {cnpj} com quantidade incorreta de dígitos deveria ser inválido");
    }

    [Theory]
    [InlineData("00000000000000")]
    [InlineData("11111111111111")]
    [InlineData("22222222222222")]
    [InlineData("33333333333333")]
    [InlineData("44444444444444")]
    [InlineData("55555555555555")]
    [InlineData("66666666666666")]
    [InlineData("77777777777777")]
    [InlineData("88888888888888")]
    [InlineData("99999999999999")]
    public void Valido_ComCnpjComTodosDigitosIguais_DeveRetornarFalse(string cnpj)
    {
        // Act
        var resultado = ValidadorCnpj.Valido(cnpj);

        // Assert
        resultado.Should().BeFalse($"CNPJ {cnpj} com todos os dígitos iguais deveria ser inválido");
    }

    [Theory]
    [InlineData("11222333000182")]  // Último dígito incorreto
    [InlineData("11444777000162")]  // Último dígito incorreto
    [InlineData("11222333000171")]  // Penúltimo dígito incorreto
    [InlineData("11444777000151")]  // Penúltimo dígito incorreto
    [InlineData("11222333000199")]  // Ambos os dígitos verificadores incorretos
    public void Valido_ComCnpjComDigitosVerificadoresIncorretos_DeveRetornarFalse(string cnpj)
    {
        // Act
        var resultado = ValidadorCnpj.Valido(cnpj);

        // Assert
        resultado.Should().BeFalse($"CNPJ {cnpj} com dígitos verificadores incorretos deveria ser inválido");
    }

    [Theory]
    [InlineData("11.222.333/0001-81", "11222333000181")]
    [InlineData("11-444-777/0001.61", "11444777000161")]
    [InlineData("34 028 316 0001 03", "34028316000103")]
    [InlineData("11abc222def333ghi000jkl181", "11222333000181")]
    public void Valido_ComCnpjComCaracteresNaoNumericos_DeveIgnorarCaracteresEValidar(string cnpjComFormatacao, string cnpjLimpo)
    {
        // Act
        var resultadoFormatado = ValidadorCnpj.Valido(cnpjComFormatacao);
        var resultadoLimpo = ValidadorCnpj.Valido(cnpjLimpo);

        // Assert
        resultadoFormatado.Should().Be(resultadoLimpo, 
            "CNPJ com formatação deveria ter o mesmo resultado que CNPJ limpo");
        resultadoFormatado.Should().BeTrue("CNPJ deveria ser válido após limpeza");
    }

    [Fact]
    public void Valido_ComCnpjApenasComLetras_DeveRetornarFalse()
    {
        // Arrange
        var cnpj = "abcdefghijklmn";

        // Act
        var resultado = ValidadorCnpj.Valido(cnpj);

        // Assert
        resultado.Should().BeFalse("CNPJ com apenas letras deveria ser inválido");
    }

    [Fact]
    public void Valido_ComCnpjComCaracteresEspeciais_DeveRetornarFalse()
    {
        // Arrange
        var cnpj = "@#$%&*()_+-=[]";

        // Act
        var resultado = ValidadorCnpj.Valido(cnpj);

        // Assert
        resultado.Should().BeFalse("CNPJ com apenas caracteres especiais deveria ser inválido");
    }

    [Theory]
    [InlineData("60746948000112")]  // CNPJ válido adicional
    [InlineData("11444777000161")]  // CNPJ válido adicional
    [InlineData("11222333000181")]  // CNPJ válido adicional
    public void Valido_ComCnpjsValidosAdicionais_DeveRetornarTrue(string cnpj)
    {
        // Act
        var resultado = ValidadorCnpj.Valido(cnpj);

        // Assert
        resultado.Should().BeTrue($"CNPJ {cnpj} deveria ser válido");
    }

    [Theory]
    [InlineData("60746948000113")]  // Último dígito alterado
    [InlineData("11444777000162")]  // Último dígito alterado
    [InlineData("11222333000182")]  // Último dígito alterado
    public void Valido_ComCnpjsInvalidosBaseadosEmValidosComAlteracao_DeveRetornarFalse(string cnpj)
    {
        // Act
        var resultado = ValidadorCnpj.Valido(cnpj);

        // Assert
        resultado.Should().BeFalse($"CNPJ {cnpj} deveria ser inválido");
    }

    [Theory]
    [InlineData("12345678901234")]  // Sequência numérica
    [InlineData("98765432109876")]  // Sequência numérica reversa
    [InlineData("13579024681357")]  // Números alternados
    public void Valido_ComCnpjsComSequenciasNumericas_DeveRetornarFalse(string cnpj)
    {
        // Act
        var resultado = ValidadorCnpj.Valido(cnpj);

        // Assert
        resultado.Should().BeFalse($"CNPJ {cnpj} com sequência numérica deveria ser inválido");
    }

    [Fact]
    public void Valido_ComCnpjComZerosAEsquerda_DeveValidarCorretamente()
    {
        // Arrange
        var cnpj = "01234567000195";

        // Act
        var resultado = ValidadorCnpj.Valido(cnpj);

        // Assert
        resultado.Should().BeTrue("CNPJ com zeros à esquerda deveria ser válido se os dígitos verificadores estiverem corretos");
    }
}
