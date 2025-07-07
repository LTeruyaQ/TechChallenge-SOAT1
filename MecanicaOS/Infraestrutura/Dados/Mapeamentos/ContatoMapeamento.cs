using Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestrutura.Dados.Mapeamentos;

public class ContatoMapeamento : IEntityTypeConfiguration<Contato>
{
    public void Configure(EntityTypeBuilder<Contato> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(e => e.DataCadastro).IsRequired();
        builder.Property(e => e.Ativo).IsRequired();
        builder.Property(e => e.DataAtualizacao).IsRequired(false);

        builder.Property(e => e.Email).IsRequired();
        builder.Property(e => e.Telefone).IsRequired();

        builder.HasOne(e => e.Cliente)
               .WithOne(c => c.Contato)
               .HasForeignKey<Cliente>(e => e.Contato)
               .OnDelete(DeleteBehavior.Cascade);
    }
}