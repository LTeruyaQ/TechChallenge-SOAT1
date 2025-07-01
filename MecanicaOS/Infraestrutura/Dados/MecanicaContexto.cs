using Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Infraestrutura.Dados;

public class MecanicaContexto : DbContext
{
    public DbSet<Servico> Servicos { get; set; }
    public MecanicaContexto(DbContextOptions<MecanicaContexto> options) : base(options) { }

    //TODO: adicionar entidades aqui
}
