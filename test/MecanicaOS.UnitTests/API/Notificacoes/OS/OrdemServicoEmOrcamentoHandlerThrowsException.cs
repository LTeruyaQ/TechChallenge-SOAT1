using API.Notificacoes.OS;
using Core.Interfaces.Controllers;
using Core.Interfaces.root;
using Core.Interfaces.Servicos;
using MediatR;

namespace MecanicaOS.UnitTests.API.Notificacoes.OS
{
    /// <summary>
    /// Versão de teste do OrdemServicoEmOrcamentoHandler que lança uma exceção FileNotFoundException
    /// </summary>
    public class OrdemServicoEmOrcamentoHandlerThrowsException : INotificationHandler<OrdemServicoEmOrcamentoEvent>
    {
        private readonly IOrdemServicoController _ordemServicoController;
        private readonly ILogServico<OrdemServicoEmOrcamentoHandlerMock> _logServico;

        public OrdemServicoEmOrcamentoHandlerThrowsException(ICompositionRoot compositionRoot)
        {
            _ordemServicoController = compositionRoot.CriarOrdemServicoController();
            _logServico = compositionRoot.CriarLogService<OrdemServicoEmOrcamentoHandlerMock>();
        }

        public async Task Handle(OrdemServicoEmOrcamentoEvent notification, CancellationToken cancellationToken)
        {
            var metodo = nameof(Handle);

            try
            {
                _logServico.LogInicio(metodo, notification.OrdemServicoId);

                await _ordemServicoController.CalcularOrcamentoAsync(notification.OrdemServicoId);

                var os = await _ordemServicoController.ObterPorId(notification.OrdemServicoId);

                // Simular que o arquivo de template não existe
                throw new FileNotFoundException("Template não encontrado", "EmailOrcamentoOS.html");
            }
            catch (Exception e)
            {
                _logServico.LogErro(metodo, e);
                throw;
            }
        }
    }
}
