using Aplicacao.DTOs.Requests.Estoque;
using Aplicacao.UseCases.Estoque.CriarEstoque;
using Dominio.Entidades;

namespace MecanicaOSTests.Fixtures
{
    public static class EstoqueFixture
    {
        public static CriarEstoqueRequest CriarCadastrarEstoqueRequestValido()
        {
            return new CriarEstoqueRequest
            {
                Insumo = "Óleo Motor",
                Descricao = "Óleo sintético 5W30",
                QuantidadeDisponivel = 10,
                Preco = 45.90m
            };
        }

        public static AtualizarEstoqueRequest CriarAtualizarEstoqueRequestValido()
        {
            return new AtualizarEstoqueRequest
            {
                Insumo = "Óleo Motor Atualizado",
                Descricao = "Óleo sintético 5W30 Premium",
                QuantidadeDisponivel = 15,
                Preco = (decimal)49.90
            };
        }

        public static Estoque CriarEstoqueValido()
        {
            return new Estoque
            (
                "Óleo Motor",
                "Óleo sintético 5W30",
                (decimal)45.90,
                10,
                10
            );
        }
    }
}
