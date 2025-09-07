using Aplicacao.Interfaces.Gateways;
using Dominio.Entidades;
using Infraestrutura.Dados;
using Infraestrutura.Gateways;

namespace Infraestrutura.Repositories
{
    public class EstoqueGateway : RepositorioGateway<Estoque>, IEstoqueGateway
    {
        public EstoqueGateway(MecanicaContexto dbContext) : base(dbContext) { }
    }
}