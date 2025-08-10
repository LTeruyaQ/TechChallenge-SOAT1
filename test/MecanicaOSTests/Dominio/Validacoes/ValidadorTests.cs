using Dominio.Validacoes;
using FluentAssertions;
using Xunit;

namespace MecanicaOSTests.Dominio.Validacoes
{
    public class ValidadorTests
    {
        [Theory]
        [InlineData("11144477735", true)]
        [InlineData("12345678901", false)]
        [InlineData("11111111111", false)]
        [InlineData("123.456.789-01", false)]
        [InlineData("111.444.777-35", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void ValidadorCpf_DeveValidarCorretamente(string cpf, bool esperado)
        {
            // Act
            var resultado = ValidadorCpf.Valido(cpf);

            // Assert
            resultado.Should().Be(esperado);
        }

        [Theory]
        [InlineData("33041260065290", true)]
        [InlineData("11111111111111", false)]
        [InlineData("12345678901234", false)]
        [InlineData("33.041.260/0652-90", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void ValidadorCnpj_DeveValidarCorretamente(string cnpj, bool esperado)
        {
            // Act
            var resultado = ValidadorCnpj.Valido(cnpj);

            // Assert
            resultado.Should().Be(esperado);
        }
    }
}
