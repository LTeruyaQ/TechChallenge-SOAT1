using Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestrutura.Dados.Mapeamentos;

    public class ClienteMapeamento : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("Clientes");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Nome)
                .IsRequired()
                .HasColumnType("varchar(200)");

            builder.Property(c => c.Sexo)
                .HasColumnType("varchar(10)");

            builder.Property(c => c.Documento)
                .IsRequired()
                .HasColumnType("varchar(20)");

            builder.Property(c => c.DataNascimento)
                .IsRequired()
                .HasColumnType("varchar(10)");

            builder.Property(c => c.TipoCliente)
                .IsRequired()
                .HasColumnType("varchar(20)");

            // Relacionamento 1:1 com Contato
            builder.HasOne(c => c.Contato)
                .WithOne(c => c.Cliente)
                .HasForeignKey<Contato>(c => c.IdCliente)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento 1:1 com Endereco
            builder.HasOne(c => c.Endereco)
                .WithOne(e => e.Cliente)
                .HasForeignKey<Endereco>(e => e.IdCliente)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento 1:1 com Usuario
            builder.HasOne(c => c.Usuario)
                .WithOne()
                .HasForeignKey<Usuario>(u => u.IdCliente)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento 1:N com Veiculos
            builder.HasMany(c => c.Veiculos)
                .WithOne(v => v.Cliente)
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices
            builder.HasIndex(c => c.Documento)
                .IsUnique();

            builder.HasIndex(c => c.Nome);
        }
    }
