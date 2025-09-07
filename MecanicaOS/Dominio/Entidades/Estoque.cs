using Dominio.Entidades.Abstratos;
using Dominio.Exceptions.Estoque;

namespace Dominio.Entidades;

public class Estoque : Entidade
{
    public string Insumo { get; private set; } = default!;
    public string? Descricao { get; private set; }
    public decimal Preco { get; private set; }
    public int QuantidadeDisponivel { get; private set; }
    public int QuantidadeMinima { get; private set; }

    public Estoque() { }

    public Estoque(string insumo, string? descricao, decimal preco, int quantidadeDisponivel, int quantidadeMinima)
    {
        if (preco <= 0)
            throw new EstoquePrecoInvalidoException();

        Insumo = insumo;
        Descricao = descricao;
        Preco = preco;
        QuantidadeDisponivel = quantidadeDisponivel;
        QuantidadeMinima = quantidadeMinima;
    }

    public void Atualizar(string? insumo, string? descricao, decimal? preco, int? quantidadeDisponivel, int? quantidadeMinima)
    {
        if (!string.IsNullOrEmpty(insumo)) Insumo = insumo;
        if (!string.IsNullOrEmpty(descricao)) Descricao = descricao;
        if (quantidadeDisponivel.HasValue) QuantidadeDisponivel = quantidadeDisponivel.Value;
        if (quantidadeMinima.HasValue) QuantidadeMinima = quantidadeMinima.Value;

        if (preco.HasValue)
        {
            if (preco.Value <= 0) throw new EstoquePrecoInvalidoException();
            Preco = preco.Value;
        }
    }
}