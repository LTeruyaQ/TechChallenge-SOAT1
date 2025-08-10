using Dominio.Especificacoes.Veiculo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace MecanicaOSTests.Dominio.Especificacoes.Veiculo
{
    public class VeiculoEspecificacaoTests
    {
        private readonly Guid clienteId = Guid.NewGuid();

        private List<global::Dominio.Entidades.Veiculo> GetVeiculosDeTeste()
        {
            return new List<global::Dominio.Entidades.Veiculo>
            {
                new global::Dominio.Entidades.Veiculo { Id = Guid.NewGuid(), Placa = "ABC-1234", Marca = "Ford", Modelo = "Fiesta", Ano = "2020", ClienteId = clienteId },
                new global::Dominio.Entidades.Veiculo { Id = Guid.NewGuid(), Placa = "DEF-5678", Marca = "Chevrolet", Modelo = "Onix", Ano = "2021", ClienteId = clienteId },
                new global::Dominio.Entidades.Veiculo { Id = Guid.NewGuid(), Placa = "GHI-9012", Marca = "Volkswagen", Modelo = "Gol", Ano = "2019", ClienteId = Guid.NewGuid() }
            };
        }

        [Fact]
        public void ObterVeiculoPorClienteEspecificacao_DeveRetornarVeiculosCorretos()
        {
            // Arrange
            var especificacao = new ObterVeiculoPorClienteEspecificacao(clienteId);
            var veiculos = GetVeiculosDeTeste();

            // Act
            var resultado = veiculos.Where(especificacao.Expressao.Compile()).ToList();

            // Assert
            Assert.Equal(2, resultado.Count);
            Assert.All(resultado, v => Assert.Equal(clienteId, v.ClienteId));
        }

        [Fact]
        public void ObterVeiculoPorPlacaEspecificacao_DeveRetornarVeiculoCorreto()
        {
            // Arrange
            var placa = "ABC-1234";
            var especificacao = new ObterVeiculoPorPlacaEspecificacao(placa);
            var veiculos = GetVeiculosDeTeste();

            // Act
            var resultado = veiculos.Where(especificacao.Expressao.Compile()).ToList();

            // Assert
            Assert.Single(resultado);
            Assert.Equal(placa, resultado.First().Placa);
        }

        [Fact]
        public void ObterVeiculosResumidosEspecificacao_DeveRetornarVeiculosCorretosEProjetarCorretamente()
        {
            // Arrange
            var placa = "ABC"; // Partial plate
            var modelo = "Fiesta";
            var especificacao = new ObterVeiculosResumidosEspecificacao(placa, modelo);
            var veiculos = GetVeiculosDeTeste();

            // Act
            var resultado = veiculos.AsQueryable().Where(especificacao.Expressao).ToList();

            // Assert a nível de filtro
            Assert.Single(resultado);
            Assert.Equal(modelo, resultado.First().Modelo);
            Assert.Contains(placa, resultado.First().Placa);

            // Assert a nível de projeção
            var projecaoLambda = (LambdaExpression)especificacao.ObterProjecao();
            Assert.NotNull(projecaoLambda);

            // Compila e executa a projeção no primeiro item filtrado
            var projecaoFunc = projecaoLambda.Compile();
            var veiculoProjetado = (VeiculoResumidoDto)projecaoFunc.DynamicInvoke(resultado.First());

            Assert.NotNull(veiculoProjetado);
            Assert.Equal(resultado.First().Id, veiculoProjetado.Id);
            Assert.Equal(resultado.First().Modelo, veiculoProjetado.Modelo);
            Assert.Equal(resultado.First().Placa, veiculoProjetado.Placa);
            Assert.Equal(resultado.First().Ano, veiculoProjetado.Ano);
        }
    }
}
