using Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestrutura.Dados.Mapeamentos;

public class UsuarioMapeamento : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(e => e.DataCadastro).IsRequired();
        builder.Property(e => e.Ativo).IsRequired();
        builder.Property(e => e.DataAtualizacao).IsRequired(false);

        builder.Property(e => e.Email).IsRequired();
        builder.Property(e => e.Senha).IsRequired();
    }
}