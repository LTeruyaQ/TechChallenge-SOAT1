using Core.DTOs.Requests.Estoque;
using Core.Interfaces.Controllers;
using Core.Interfaces.root;

namespace Adapters.Controllers
{
    public class AlertaEstoqueController : IAlertaEstoqueController
    {
        private readonly ICompositionRoot _compositionRoot;

        public AlertaEstoqueController(ICompositionRoot compositionRoot)
        {
            _compositionRoot = compositionRoot;
        }

        public async Task CadastrarAlertas(IEnumerable<CadastrarAlertaEstoqueRequest> request)
        {
            // TODO: Implementar lógica para cadastrar alertas de estoque
            // Por ora, método vazio para não quebrar a compilação
            await Task.CompletedTask;
        }

        public async Task<bool> VerificarAlertaEnviadoHoje(Guid estoqueId)
        {
            // TODO: Implementar lógica para verificar se alerta foi enviado hoje
            // Por ora, sempre retorna false para permitir envio
            return await Task.FromResult(false);
        }
    }
}
