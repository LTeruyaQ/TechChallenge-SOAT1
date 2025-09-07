using Aplicacao.Ports;
using Dominio.Entidades;
using Infraestrutura.Dados;
using Infraestrutura.Repositorios;

namespace Infraestrutura.Repositories
{
    public class EstoqueRepositorio : Repositorio<Estoque>, IEstoqueRepository
    {
        public EstoqueRepositorio(MecanicaContexto dbContext) : base(dbContext) { }
    }
}