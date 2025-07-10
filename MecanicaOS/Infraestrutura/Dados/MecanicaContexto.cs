using Dominio.Entidades;
using Infraestrutura.Dados.Mapeamentos;
using Microsoft.EntityFrameworkCore;

namespace Infraestrutura.Dados;

public class MecanicaContexto : DbContext
{
    public MecanicaContexto(DbContextOptions<MecanicaContexto> options) : base(options) { }

    public DbSet<Servico> Servicos { get; set; }
    public DbSet<Estoque> Estoques { get; set; }
    public DbSet<Veiculo> Veiculos { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Endereco> Enderecos { get; set; }
    public DbSet<Contato> Contatos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries().Where(entry => entry.Entity.GetType().GetProperty("DataCadastro") != null))
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property("DataCadastro").CurrentValue = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Property("DataCadastro").IsModified = false;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EstoqueMapeamento());
        modelBuilder.ApplyConfiguration(new VeiculoMapeamento());
        modelBuilder.ApplyConfiguration(new ClienteMapeamento());
        modelBuilder.ApplyConfiguration(new UsuarioMapeamento());

        base.OnModelCreating(modelBuilder);
    }
}
