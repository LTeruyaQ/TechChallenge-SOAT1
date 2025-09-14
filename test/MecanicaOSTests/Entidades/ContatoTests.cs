using FluentAssertions;
using Moq;

namespace MecanicaOSTests.Entidades
{
    public class ContatoTests
    {
        [Fact]
        public void Dado_DadosValidos_Quando_CriarContato_Entao_DeveCriarComSucesso()
        {
            // Arrange
            var idCliente = Guid.NewGuid();
            var clienteMock = new Mock<Cliente>();
            var email = "contato@teste.com";
            var telefone = "123456789";

            // Act
            var contato = new Contato
            {
                IdCliente = idCliente,
                Cliente = clienteMock.Object,
                Email = email,
                Telefone = telefone
            };

            // Assert
            contato.Should().NotBeNull();
            contato.IdCliente.Should().Be(idCliente);
            contato.Cliente.Should().BeEquivalentTo(clienteMock.Object);
            contato.Email.Should().Be(email);
            contato.Telefone.Should().Be(telefone);
        }
    }
}
