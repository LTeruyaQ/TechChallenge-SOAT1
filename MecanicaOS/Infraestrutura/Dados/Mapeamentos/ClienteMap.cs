using Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestrutura.Dados.Mapeamentos;

public class ClienteMap : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(e => e.Nome).HasMaxLength(100);        
        builder.Property(e => e.Sexo).IsRequired().HasMaxLength(10);
        builder.Property(e => e.TipoCliente).IsRequired().HasMaxLength(10);
        builder.Property(e => e.Documento).IsRequired().HasMaxLength(20);
        builder.Property(e => e.DataCadastro).IsRequired();
        builder.Property(e => e.DataNascimento).IsRequired();
        builder.Property(e => e.Ativo).IsRequired();
        builder.Property(e => e.DataAtualizacao).IsRequired(false);        

        builder.HasOne(e => e.Endereco)
               .WithOne(c => c.Cliente)
               .HasForeignKey<Endereco>(e => e.Cliente)
               .OnDelete(DeleteBehavior.Cascade);
    }
}