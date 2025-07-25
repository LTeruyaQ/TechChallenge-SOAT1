﻿using Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestrutura.Dados.Mapeamentos;

public class UsuarioMapeamento : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.DataCadastro).IsRequired();
        builder.Property(c => c.Ativo).IsRequired();
        builder.Property(c => c.DataAtualizacao).IsRequired(false);

        builder.Property(c => c.Email).IsRequired().HasMaxLength(254);
        builder.Property(c => c.Senha).IsRequired();
        builder.Property(u => u.TipoUsuario).HasConversion<string>().HasMaxLength(50);
    }
}