using Dominio.Entidades;

namespace Dominio.Interfaces.Servicos;

public interface IServicoNotificacaoEmail
{
    Task EnviarAlertaEstoqueAsync(IEnumerable<Estoque> itensCriticos);
}