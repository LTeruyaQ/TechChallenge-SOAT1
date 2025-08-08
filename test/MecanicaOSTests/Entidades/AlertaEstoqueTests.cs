using Dominio.Entidades;
using FluentAssertions;
using System;
using Xunit;

namespace MecanicaOSTests.Entidades
{
    public class AlertaEstoqueTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_AcessarPropriedades_Entao_DeveRetornarValoresPadrao()
        {
            // Arrange
            var alerta = new AlertaEstoque();

            // Act
            var estoqueId = alerta.EstoqueId;
            var estoque = alerta.Estoque;

            // Assert
            estoqueId.Should().Be(Guid.Empty);
            estoque.Should().BeNull();
        }
    }
}
