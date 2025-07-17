using Aplicacao.DTOs.Requests.Estoque;
using Dominio.Entidades;

namespace MecanicaOSTests.Fixtures
{
    public static class EstoqueFixture
    {
        public static CadastrarEstoqueRequest CriarCadastrarEstoqueRequestValido()
        {
            return new CadastrarEstoqueRequest
            {
                Insumo = "Óleo Motor",
                Descricao = "Óleo sintético 5W30",
                QuantidadeDisponivel = 10,
                Preco = 45.90
            };
        }

        public static AtualizarEstoqueRequest CriarAtualizarEstoqueRequestValido()
        {
            return new AtualizarEstoqueRequest
            {
                Insumo = "Óleo Motor Atualizado",
                Descricao = "Óleo sintético 5W30 Premium",
                QuantidadeDisponivel = 15,
                Preco = 49.90
            };
        }

        public static Estoque CriarEstoqueValido()
        {
            return new Estoque
            {
                Id = Guid.NewGuid(),
                Insumo = "Óleo Motor",
                Descricao = "Óleo sintético 5W30",
                QuantidadeDisponivel = 10,
                Preco = 45.90,
                DataCadastro = DateTime.UtcNow,
                DataAtualizacao = DateTime.UtcNow
            };
        }
    }
}
