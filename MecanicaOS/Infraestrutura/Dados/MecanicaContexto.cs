using Dominio.Entidades;
using Dominio.Entidades.Abstratos;
using Infraestrutura.Dados.Mapeamentos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infraestrutura.Dados;

public class MecanicaContexto : DbContext
{
    private readonly IMediator _mediator;
    public MecanicaContexto(DbContextOptions<MecanicaContexto> options, IMediator mediator) : base(options)
    {
        _mediator = mediator;
    }

    public DbSet<Servico> Servicos { get; set; }
    public DbSet<Estoque> Estoques { get; set; }
    public DbSet<Veiculo> Veiculos { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Endereco> Enderecos { get; set; }
    public DbSet<Contato> Contatos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<AlertaEstoque> AlertasEstoque { get; set; }
    public DbSet<OrdemServico> OrdensSevico { get; set; }
    public DbSet<Orcamento> Orcamentos { get; set; }
    public DbSet<InsumoOS> InsumosOrdemServico { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
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

            if (entry.Entity is Entidade entidade && entidade.Eventos.Any())
            {
                foreach (var evento in entidade.Eventos)
                {
                    await _mediator.Publish(evento, cancellationToken);
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EstoqueMapeamento());
        modelBuilder.ApplyConfiguration(new VeiculoMapeamento());
        modelBuilder.ApplyConfiguration(new ClienteMapeamento());
        modelBuilder.ApplyConfiguration(new ContatoMapeamento());
        modelBuilder.ApplyConfiguration(new UsuarioMapeamento());
        modelBuilder.ApplyConfiguration(new OrdemServicoMapeamento());

        base.OnModelCreating(modelBuilder);
    }
}
