using Dominio.Entidades;
using FluentAssertions;
using Moq;

namespace MecanicaOSTests.Entidades
{
    public class VeiculoTests
    {
        [Fact]
        public void Dado_DadosValidos_Quando_CriarVeiculo_Entao_DeveCriarComSucesso()
        {
            // Arrange
            var placa = "ABC-1234";
            var marca = "Marca Teste";
            var modelo = "Modelo Teste";
            var cor = "Cor Teste";
            var ano = "2023";
            var anotacoes = "Anotações do Veículo";
            var clienteId = Guid.NewGuid();
            var clienteMock = new Mock<Cliente>();

            // Act
            var veiculo = new Veiculo
            {
                Placa = placa,
                Marca = marca,
                Modelo = modelo,
                Cor = cor,
                Ano = ano,
                Anotacoes = anotacoes,
                ClienteId = clienteId,
                Cliente = clienteMock.Object
            };

            // Assert
            veiculo.Should().NotBeNull();
            veiculo.Placa.Should().Be(placa);
            veiculo.Marca.Should().Be(marca);
            veiculo.Modelo.Should().Be(modelo);
            veiculo.Cor.Should().Be(cor);
            veiculo.Ano.Should().Be(ano);
            veiculo.Anotacoes.Should().Be(anotacoes);
            veiculo.ClienteId.Should().Be(clienteId);
            veiculo.Cliente.Should().Be(clienteMock.Object);
        }
    }
}
