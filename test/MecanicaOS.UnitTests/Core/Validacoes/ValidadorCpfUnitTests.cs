using Core.Validacoes;

namespace MecanicaOS.UnitTests.Core.Validacoes;

public class ValidadorCpfUnitTests
{
    [Theory]
    [InlineData("11144477735")]  // CPF válido
    [InlineData("12345678909")]  // CPF válido
    [InlineData("98765432100")]  // CPF válido
    [InlineData("111.444.777-35")]  // CPF válido com formatação
    [InlineData("123.456.789-09")]  // CPF válido com formatação
    [InlineData("987.654.321-00")]  // CPF válido com formatação
    public void Valido_ComCpfValido_DeveRetornarTrue(string cpf)
    {
        // Act
        var resultado = ValidadorCpf.Valido(cpf);

        // Assert
        resultado.Should().BeTrue($"CPF {cpf} deveria ser válido");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Valido_ComCpfNuloOuVazio_DeveRetornarFalse(string cpf)
    {
        // Act
        var resultado = ValidadorCpf.Valido(cpf);

        // Assert
        resultado.Should().BeFalse("CPF nulo ou vazio deveria ser inválido");
    }

    [Theory]
    [InlineData("1234567890")]    // 10 dígitos
    [InlineData("123456789012")]  // 12 dígitos
    [InlineData("12345")]         // 5 dígitos
    public void Valido_ComCpfComQuantidadeIncorretaDeDigitos_DeveRetornarFalse(string cpf)
    {
        // Act
        var resultado = ValidadorCpf.Valido(cpf);

        // Assert
        resultado.Should().BeFalse($"CPF {cpf} com quantidade incorreta de dígitos deveria ser inválido");
    }

    [Theory]
    [InlineData("00000000000")]
    [InlineData("11111111111")]
    [InlineData("22222222222")]
    [InlineData("33333333333")]
    [InlineData("44444444444")]
    [InlineData("55555555555")]
    [InlineData("66666666666")]
    [InlineData("77777777777")]
    [InlineData("88888888888")]
    [InlineData("99999999999")]
    public void Valido_ComCpfComTodosDigitosIguais_DeveRetornarFalse(string cpf)
    {
        // Act
        var resultado = ValidadorCpf.Valido(cpf);

        // Assert
        resultado.Should().BeFalse($"CPF {cpf} com todos os dígitos iguais deveria ser inválido");
    }

    [Theory]
    [InlineData("11144477736")]  // Último dígito incorreto
    [InlineData("12345678908")]  // Último dígito incorreto
    [InlineData("11144477725")]  // Penúltimo dígito incorreto
    [InlineData("12345678999")]  // Penúltimo dígito incorreto
    [InlineData("12345678900")]  // Ambos os dígitos verificadores incorretos
    public void Valido_ComCpfComDigitosVerificadoresIncorretos_DeveRetornarFalse(string cpf)
    {
        // Act
        var resultado = ValidadorCpf.Valido(cpf);

        // Assert
        resultado.Should().BeFalse($"CPF {cpf} com dígitos verificadores incorretos deveria ser inválido");
    }

    [Theory]
    [InlineData("111.444.777-35", "11144477735")]
    [InlineData("123-456-789.09", "12345678909")]
    [InlineData("987 654 321 00", "98765432100")]
    [InlineData("111abc444def777ghi35", "11144477735")]
    public void Valido_ComCpfComCaracteresNaoNumericos_DeveIgnorarCaracteresEValidar(string cpfComFormatacao, string cpfLimpo)
    {
        // Act
        var resultadoFormatado = ValidadorCpf.Valido(cpfComFormatacao);
        var resultadoLimpo = ValidadorCpf.Valido(cpfLimpo);

        // Assert
        resultadoFormatado.Should().Be(resultadoLimpo,
            "CPF com formatação deveria ter o mesmo resultado que CPF limpo");
        resultadoFormatado.Should().BeTrue("CPF deveria ser válido após limpeza");
    }

    [Fact]
    public void Valido_ComCpfApenasComLetras_DeveRetornarFalse()
    {
        // Arrange
        var cpf = "abcdefghijk";

        // Act
        var resultado = ValidadorCpf.Valido(cpf);

        // Assert
        resultado.Should().BeFalse("CPF com apenas letras deveria ser inválido");
    }

    [Fact]
    public void Valido_ComCpfComCaracteresEspeciais_DeveRetornarFalse()
    {
        // Arrange
        var cpf = "@#$%&*()_+-";

        // Act
        var resultado = ValidadorCpf.Valido(cpf);

        // Assert
        resultado.Should().BeFalse("CPF com apenas caracteres especiais deveria ser inválido");
    }

    [Theory]
    [InlineData("52998224725")]  // CPF válido adicional
    [InlineData("01234567890")]  // CPF válido com zero à esquerda
    [InlineData("11144477735")]  // CPF válido adicional
    public void Valido_ComCpfsValidosAdicionais_DeveRetornarTrue(string cpf)
    {
        // Act
        var resultado = ValidadorCpf.Valido(cpf);

        // Assert
        resultado.Should().BeTrue($"CPF {cpf} deveria ser válido");
    }

    [Theory]
    [InlineData("52998224726")]  // Último dígito alterado
    [InlineData("01234567891")]  // Último dígito alterado
    [InlineData("04688645192")]  // Último dígito alterado
    public void Valido_ComCpfsInvalidosBaseadosEmValidosComAlteracao_DeveRetornarFalse(string cpf)
    {
        // Act
        var resultado = ValidadorCpf.Valido(cpf);

        // Assert
        resultado.Should().BeFalse($"CPF {cpf} deveria ser inválido");
    }
}
