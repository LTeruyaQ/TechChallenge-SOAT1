using Core.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestrutura.Dados.Mapeamentos;

public partial class EstoqueMapeamento : IEntityTypeConfiguration<Estoque>
{
    public void Configure(EntityTypeBuilder<Estoque> builder)
    {
        builder.Property(e => e.Insumo).HasMaxLength(100);

        builder.Property(e => e.Descricao).HasMaxLength(500);
    }
}
