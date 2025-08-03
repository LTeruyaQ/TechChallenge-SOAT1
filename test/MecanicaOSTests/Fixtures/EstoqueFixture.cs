using Aplicacao.DTOs.Requests.Estoque;
using Aplicacao.DTOs.Responses.Estoque;
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
                Preco = (decimal)49.90
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
                Preco = 50m,
                DataCadastro = DateTime.UtcNow,
                DataAtualizacao = DateTime.UtcNow
            };
        }

        public static Estoque CriarEstoque(decimal preco)
        {
            var estoque = CriarEstoqueValido();
            estoque.Preco = preco;
            return estoque;
        }

        public static EstoqueResponse CriarEstoqueResponseValido(Guid id, int quantidade)
        {
            return new EstoqueResponse
            {
                Id = id,
                Insumo = "Óleo Motor",
                Descricao = "Óleo sintético 5W30",
                QuantidadeDisponivel = quantidade,
                Preco = 45.90
            };
        }
    }
}
