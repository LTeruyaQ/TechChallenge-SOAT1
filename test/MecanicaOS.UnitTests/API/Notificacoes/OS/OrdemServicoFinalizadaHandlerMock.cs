using API.Notificacoes.OS;
using Core.DTOs.Responses.OrdemServico;
using Core.Interfaces.Controllers;
using Core.Interfaces.root;
using Core.Interfaces.Servicos;
using MediatR;

namespace MecanicaOS.UnitTests.API.Notificacoes.OS
{
    /// <summary>
    /// Versão de teste do OrdemServicoFinalizadaHandler que não depende de arquivos de template
    /// </summary>
    public class OrdemServicoFinalizadaHandlerMock : INotificationHandler<OrdemServicoFinalizadaEvent>
    {
        private readonly ILogServico<OrdemServicoFinalizadaHandlerMock> _logServico;
        private readonly IServicoEmail _emailServico;
        private readonly IOrdemServicoController _ordemServicoController;

        public OrdemServicoFinalizadaHandlerMock(ICompositionRoot compositionRoot)
        {
            _logServico = compositionRoot.CriarLogService<OrdemServicoFinalizadaHandlerMock>();
            _emailServico = compositionRoot.CriarServicoEmail();
            _ordemServicoController = compositionRoot.CriarOrdemServicoController();
        }

        public async Task Handle(OrdemServicoFinalizadaEvent notification, CancellationToken cancellationToken)
        {
            var metodo = nameof(Handle);

            try
            {
                _logServico.LogInicio(metodo, notification.OrdemServicoId);

                var osDto = await _ordemServicoController.ObterPorId(notification.OrdemServicoId);

                if (osDto is null) return;

                string conteudo = GerarConteudoEmail(osDto);

                string[] emails = new string[] { osDto.Cliente.Contato.Email };
                await _emailServico.EnviarAsync(
                    emails,
                    "Serviço Finalizado",
                    conteudo);

                _logServico.LogFim(metodo, null);
            }
            catch (Exception e)
            {
                _logServico.LogErro(metodo, e);
                throw;
            }
        }

        private string GerarConteudoEmail(OrdemServicoResponse os)
        {
            // Versão simplificada para testes que não depende de arquivos
            return $@"
                <html>
                <body>
                    <h1>Serviço Finalizado para {os.Cliente.Nome}</h1>
                    <p>Serviço: {os.Servico.Nome}</p>
                    <p>Veículo: {os.Veiculo.Modelo}</p>
                    <p>Placa: {os.Veiculo.Placa}</p>
                </body>
                </html>
            ";
        }
    }
}
