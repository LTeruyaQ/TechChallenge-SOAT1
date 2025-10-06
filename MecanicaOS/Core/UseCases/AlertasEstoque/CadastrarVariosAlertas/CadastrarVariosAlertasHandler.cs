using Core.DTOs.UseCases.AlertaEstoque;
using Core.Entidades;
using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.AlertasEstoque;

namespace Core.UseCases.AlertasEstoque.CadastrarVariosAlertas
{
    public class CadastrarVariosAlertasHandler : ICadastrarVariosAlertasHandler
    {
        private readonly IAlertaEstoqueGateway _alertaEstoqueGateway;

        public CadastrarVariosAlertasHandler(IAlertaEstoqueGateway alertaEstoqueGateway)
        {
            _alertaEstoqueGateway = alertaEstoqueGateway ?? throw new ArgumentNullException(nameof(alertaEstoqueGateway));
        }

        public async Task Handle(IEnumerable<CadastrarAlertaEstoqueUseCaseDto> alertas)
        {
            if (alertas == null || !alertas.Any())
                return;

            // Handler (Core) cria as entidades a partir dos DTOs
            var entidades = alertas.Select(dto => new AlertaEstoque
            {
                EstoqueId = dto.EstoqueId,
                DataCadastro = dto.DataEnvio
            });

            await _alertaEstoqueGateway.CadastrarVariosAsync(entidades);
        }
    }
}
