
using Core.DTOs.Repositories.Cliente;
using Core.DTOs.Repositories.Estoque;
using Core.DTOs.Repositories.OrdemServicos;
using Core.DTOs.Repositories.Servico;
using Core.DTOs.Repositories.Usuarios;
using Core.DTOs.Repositories.Veiculo;
using Infraestrutura.Dados.Mapeamentos;
using Microsoft.EntityFrameworkCore;

namespace Infraestrutura.Dados;

public class MecanicaContexto : DbContext
{
    public MecanicaContexto(DbContextOptions<MecanicaContexto> options) : base(options) { }

    public DbSet<ServicoRepositoryDto> Servicos { get; set; }
    public DbSet<EstoqueRepositoryDto> Estoques { get; set; }
    public DbSet<VeiculoRepositoryDto> Veiculos { get; set; }
    public DbSet<ClienteRepositoryDTO> Clientes { get; set; }
    public DbSet<EnderecoRepositoryDto> Enderecos { get; set; }
    public DbSet<ContatoRepositoryDTO> Contatos { get; set; }
    public DbSet<UsuarioRepositoryDto> Usuarios { get; set; }
    public DbSet<AlertaEstoqueRepositoryDto> AlertasEstoque { get; set; }
    public DbSet<OrdemServicoRepositoryDto> OrdensSevico { get; set; }
    public DbSet<InsumoOSRepositoryDto> InsumosOrdemServico { get; set; }

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
        modelBuilder.ApplyConfiguration(new ContatoMapeamento());
        modelBuilder.ApplyConfiguration(new UsuarioMapeamento());
        modelBuilder.ApplyConfiguration(new OrdemServicoMapeamento());

        base.OnModelCreating(modelBuilder);
    }
}
