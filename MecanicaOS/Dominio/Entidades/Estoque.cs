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
        if (quantidadeMinima > quantidadeDisponivel)
            throw new EstoqueQuantidadeMinimaException();

        if (preco <= 0)
            throw new EstoquePrecoInvalidoException();

        Insumo = insumo;
        Descricao = descricao;
        Preco = preco;
        QuantidadeDisponivel = quantidadeDisponivel;
        QuantidadeMinima = quantidadeMinima;
    }
}