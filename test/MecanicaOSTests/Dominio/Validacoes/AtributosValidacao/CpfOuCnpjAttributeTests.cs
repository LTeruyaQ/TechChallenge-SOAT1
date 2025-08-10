using Dominio.Validacoes.AtributosValidacao;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace MecanicaOSTests.Dominio.Validacoes.AtributosValidacao
{
    public class CpfOuCnpjAttributeTests
    {
        private class TestModel
        {
            [CpfOuCnpj]
            public string Documento { get; set; }
        }

        [Theory]
        [InlineData("11144477735", true)] // CPF Válido
        [InlineData("33041260065290", true)] // CNPJ Válido
        [InlineData("12345678901", false)] // CPF Inválido
        [InlineData("12345678901234", false)] // CNPJ Inválido
        [InlineData("documento_invalido", false)]
        [InlineData("", true)] // Empty should be valid to allow other attributes to handle required validation
        [InlineData(null, true)] // Null should be valid
        public void CpfOuCnpjAttribute_DeveValidarCorretamente(string documento, bool expected)
        {
            // Arrange
            var model = new TestModel { Documento = documento };
            var validationContext = new ValidationContext(model) { MemberName = nameof(TestModel.Documento) };
            var attribute = new CpfOuCnpjAttribute();

            // Act
            var result = attribute.GetValidationResult(model.Documento, validationContext);

            // Assert
            if (expected)
            {
                Assert.Equal(ValidationResult.Success, result);
            }
            else
            {
                Assert.NotEqual(ValidationResult.Success, result);
            }
        }
    }
}
