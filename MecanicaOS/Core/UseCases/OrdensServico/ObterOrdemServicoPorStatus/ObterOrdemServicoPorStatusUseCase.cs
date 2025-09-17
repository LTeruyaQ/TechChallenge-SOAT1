using Core.Enumeradores;

namespace Core.UseCases.OrdensServico.ObterOrdemServicoPorStatus
{
    public class ObterOrdemServicoPorStatusUseCase
    {
        public StatusOrdemServico Status { get; set; }

        public ObterOrdemServicoPorStatusUseCase(StatusOrdemServico status)
        {
            Status = status;
        }
    }
}
