using Dominio.Validacoes;
using FluentAssertions;
using Xunit;

namespace MecanicaOSTests.Dominio.Validacoes
{
    public class ValidadorTests
    {
        [Theory]
        [InlineData("75149366030", true)]
        [InlineData("12345678901", false)]
        [InlineData("11111111111", false)]
        [InlineData("123.456.789-01", false)]
        [InlineData("751.493.660-30", true)]
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
        [InlineData("81287131000170", true)]
        [InlineData("11111111111111", false)]
        [InlineData("12345678901234", false)]
        [InlineData("81.287.131/0001-70", true)]
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
