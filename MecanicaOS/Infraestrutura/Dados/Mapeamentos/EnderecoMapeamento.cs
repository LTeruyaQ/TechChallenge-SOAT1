using Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestrutura.Dados.Mapeamentos;

public class EnderecoMapeamento : IEntityTypeConfiguration<Endereco>
{
    public void Configure(EntityTypeBuilder<Endereco> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(e => e.DataCadastro).IsRequired();
        builder.Property(e => e.Ativo).IsRequired();
        builder.Property(e => e.DataAtualizacao).IsRequired(false);
        builder.Property(e => e.Rua).IsRequired();
        builder.Property(e => e.Bairro).IsRequired();
        builder.Property(e => e.Numero).IsRequired(false);
        builder.Property(e => e.Cidade).IsRequired();
        builder.Property(e => e.CEP).IsRequired(false);
        builder.Property(e => e.Complemento).IsRequired(false);


        builder.HasOne(e => e.Cliente)
               .WithOne(c => c.Endereco)
               .HasForeignKey<Cliente>(e => e.Endereco)
               .OnDelete(DeleteBehavior.Cascade);
    }
}