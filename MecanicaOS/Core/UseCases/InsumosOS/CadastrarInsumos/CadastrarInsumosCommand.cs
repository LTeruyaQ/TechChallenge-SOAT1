using Core.DTOs.UseCases.OrdemServico.InsumoOS;

namespace Core.UseCases.InsumosOS.CadastrarInsumos
{
    public class CadastrarInsumosCommand
    {
        public Guid OrdemServicoId { get; set; }
        public List<CadastrarInsumoOSUseCaseDto> Request { get; set; }

        public CadastrarInsumosCommand(Guid ordemServicoId, List<CadastrarInsumoOSUseCaseDto> request)
        {
            OrdemServicoId = ordemServicoId;
            Request = request ?? throw new ArgumentNullException(nameof(request));
        }
    }
}
