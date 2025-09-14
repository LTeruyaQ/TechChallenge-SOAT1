using FluentAssertions;
using Moq;

namespace MecanicaOSTests.Entidades
{
    public class EnderecoTests
    {
        [Fact]
        public void Dado_DadosValidos_Quando_CriarEndereco_Entao_DeveCriarComSucesso()
        {
            // Arrange
            var rua = "Rua Teste";
            var bairro = "Bairro Teste";
            var cidade = "Cidade Teste";
            var numero = "123";
            var cep = "12345-678";
            var complemento = "Apto 101";
            var idCliente = Guid.NewGuid();
            var clienteMock = new Mock<Cliente>();

            // Act
            var endereco = new Endereco
            {
                Rua = rua,
                Bairro = bairro,
                Cidade = cidade,
                Numero = numero,
                CEP = cep,
                Complemento = complemento,
                IdCliente = idCliente,
                Cliente = clienteMock.Object
            };

            // Assert
            endereco.Should().NotBeNull();
            endereco.Rua.Should().Be(rua);
            endereco.Bairro.Should().Be(bairro);
            endereco.Cidade.Should().Be(cidade);
            endereco.Numero.Should().Be(numero);
            endereco.CEP.Should().Be(cep);
            endereco.Complemento.Should().Be(complemento);
            endereco.IdCliente.Should().Be(idCliente);
            endereco.Cliente.Should().BeEquivalentTo(clienteMock.Object);
        }
    }
}
