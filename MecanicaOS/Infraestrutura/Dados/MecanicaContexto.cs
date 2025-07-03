using Dominio.Entidades;
using Infraestrutura.Dados.Mapeamentos;
using Microsoft.EntityFrameworkCore;

namespace Infraestrutura.Dados;

public class MecanicaContexto : DbContext
{
    public MecanicaContexto(DbContextOptions<MecanicaContexto> options) : base(options) { }

    public DbSet<Servico> Servicos { get; set; }
    public DbSet<Estoque> Estoques { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Endereco> Enderecos { get; set; }
    public DbSet<Contato> Contatos { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EstoqueMapeamento());

        base.OnModelCreating(modelBuilder);
    }
}
