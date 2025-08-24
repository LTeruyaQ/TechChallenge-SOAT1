using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Especificacoes.OrdemServico;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MecanicaOSTests.Dominio.Especificacoes.OrdemServico
{
    public class OrdemServicoEspecificacaoTests
    {
        private List<global::Dominio.Entidades.OrdemServico> GetOrdensDeServicoDeTeste()
        {
            var cliente = new global::Dominio.Entidades.Cliente { Id = Guid.NewGuid(), Nome = "Cliente Teste", Documento = "12345678901" };
            var veiculo = new global::Dominio.Entidades.Veiculo { Id = Guid.NewGuid(), Placa = "ABC-1234", Cliente = cliente };
            var servico = new global::Dominio.Entidades.Servico { Id = Guid.NewGuid(), Nome = "Troca de Oleo", Descricao = "Desc", Valor = 100 };
            var estoque = new global::Dominio.Entidades.Estoque { Id = Guid.NewGuid(), Insumo = "Oleo", Preco = 50, QuantidadeDisponivel = 10 };

            var ordemComInsumos = new global::Dominio.Entidades.OrdemServico
            {
                Id = Guid.NewGuid(),
                Cliente = cliente,
                Veiculo = veiculo,
                Servico = servico,
                Status = StatusOrdemServico.EmExecucao,
                InsumosOS = new List<InsumoOS> { new InsumoOS { Estoque = estoque, Quantidade = 1 } }
            };

            var ordemSemInsumos = new global::Dominio.Entidades.OrdemServico
            {
                Id = Guid.NewGuid(),
                Cliente = cliente,
                Veiculo = veiculo,
                Servico = servico,
                Status = StatusOrdemServico.AguardandoAprovação
            };

            var ordemExpirada = new global::Dominio.Entidades.OrdemServico
            {
                Id = Guid.NewGuid(),
                Cliente = cliente,
                Veiculo = veiculo,
                Servico = servico,
                Status = StatusOrdemServico.AguardandoAprovação,
            };

            ordemExpirada.Orcamento = new Orcamento(ordemExpirada.Id, 150)
            {
                DataEnvio = DateTime.UtcNow.AddDays(-3)
            };

            ordemExpirada.OrcamentoId = ordemExpirada.Orcamento.Id;

            return new List<global::Dominio.Entidades.OrdemServico> { ordemComInsumos, ordemSemInsumos, ordemExpirada };
        }

        [Fact]
        public void ObterOrdemServicoPorIdComInsumosEspecificacao_DeveRetornarOrdemCorreta()
        {
            // Arrange
            var ordens = GetOrdensDeServicoDeTeste();
            var ordemComInsumos = ordens.First(o => o.InsumosOS.Any());
            var especificacao = new ObterOrdemServicoPorIdComInsumosEOrcamentoEspecificacao(ordemComInsumos.Id);

            // Act
            var resultado = ordens.Where(especificacao.Expressao.Compile()).ToList();

            // Assert
            Assert.Single(resultado);
            Assert.Equal(ordemComInsumos.Id, resultado.First().Id);
        }

        [Fact]
        public void ObterOrdemServicoPorStatusEspecificacao_DeveRetornarOrdensCorretas()
        {
            // Arrange
            var status = StatusOrdemServico.EmExecucao;
            var especificacao = new ObterOrdemServicoPorStatusEspecificacao(status);
            var ordens = GetOrdensDeServicoDeTeste();

            // Act
            var resultado = ordens.Where(especificacao.Expressao.Compile()).ToList();

            // Assert
            Assert.Single(resultado);
            Assert.Equal(status, resultado.First().Status);
        }

        [Fact]
        public void ObterOSOrcamentoExpiradoEspecificacao_DeveRetornarOrdensExpiradas()
        {
            // Arrange
            var especificacao = new ObterOSOrcamentoExpiradoEspecificacao();
            var ordens = GetOrdensDeServicoDeTeste();

            // Act
            var resultado = ordens.Where(especificacao.Expressao.Compile()).ToList();

            // Assert
            Assert.Single(resultado);
            Assert.True(resultado.First().Orcamento.DataEnvio < DateTime.UtcNow.AddDays(-2));
        }
    }
}
