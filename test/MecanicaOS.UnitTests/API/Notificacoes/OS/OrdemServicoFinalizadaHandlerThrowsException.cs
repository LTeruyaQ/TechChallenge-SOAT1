using API.Notificacoes.OS;
using Core.Interfaces.Controllers;
using Core.Interfaces.root;
using Core.Interfaces.Servicos;
using MediatR;

namespace MecanicaOS.UnitTests.API.Notificacoes.OS
{
    /// <summary>
    /// Versão de teste do OrdemServicoFinalizadaHandler que lança uma exceção FileNotFoundException
    /// </summary>
    public class OrdemServicoFinalizadaHandlerThrowsException : INotificationHandler<OrdemServicoFinalizadaEvent>
    {
        private readonly IOrdemServicoController _ordemServicoController;
        private readonly ILogServico<OrdemServicoFinalizadaHandlerMock> _logServico;

        public OrdemServicoFinalizadaHandlerThrowsException(ICompositionRoot compositionRoot)
        {
            _ordemServicoController = compositionRoot.CriarOrdemServicoController();
            _logServico = compositionRoot.CriarLogService<OrdemServicoFinalizadaHandlerMock>();
        }

        public async Task Handle(OrdemServicoFinalizadaEvent notification, CancellationToken cancellationToken)
        {
            var metodo = nameof(Handle);

            try
            {
                _logServico.LogInicio(metodo, notification.OrdemServicoId);

                var osDto = await _ordemServicoController.ObterPorId(notification.OrdemServicoId);

                if (osDto is null) return;

                // Simular que o arquivo de template não existe
                throw new FileNotFoundException("Template não encontrado", "EmailOrdemServicoFinalizada.html");
            }
            catch (Exception e)
            {
                _logServico.LogErro(metodo, e);
                throw;
            }
        }
    }
}
