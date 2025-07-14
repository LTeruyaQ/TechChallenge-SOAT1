using Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestrutura.Dados.Mapeamentos;

public class OrdemServicoMapeamento : IEntityTypeConfiguration<OrdemServico>
{
    public void Configure(EntityTypeBuilder<OrdemServico> builder)
    {
        builder.Property(o => o.Descricao).HasMaxLength(1000);

        builder.Property(o => o.Status).HasConversion<string>().HasMaxLength(80);
    }
}