using Microsoft.EntityFrameworkCore;

namespace Infraestrutura.Dados;

public class MecanicaContexto : DbContext
{
    public MecanicaContexto(DbContextOptions<MecanicaContexto> options) : base(options) { }
}
