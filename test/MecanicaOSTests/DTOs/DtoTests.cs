using FluentAssertions;

namespace MecanicaOSTests.DTOs
{
    public class DtoTests
    {
        [Fact]
        public void CadastrarClienteRequest_PodeSerInstanciado()
        {
            // Arrange
            var dto = new CadastrarClienteRequest
            {
                Nome = "Teste"
            };

            // Act & Assert
            dto.Should().NotBeNull();
            dto.Nome.Should().Be("Teste");
        }

        [Fact]
        public void ClienteResponse_PodeSerInstanciado()
        {
            // Arrange
            var dto = new ClienteResponse
            {
                Nome = "Teste Response"
            };

            // Act & Assert
            dto.Should().NotBeNull();
            dto.Nome.Should().Be("Teste Response");
        }
    }
}
