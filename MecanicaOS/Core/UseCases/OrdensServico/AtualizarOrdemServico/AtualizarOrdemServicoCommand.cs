using Core.DTOs.UseCases.OrdemServico;

namespace Core.UseCases.OrdensServico.AtualizarOrdemServico
{
    public class AtualizarOrdemServicoCommand
    {
        public Guid Id { get; set; }
        public AtualizarOrdemServicoUseCaseDto Request { get; set; }

        public AtualizarOrdemServicoCommand(Guid id, AtualizarOrdemServicoUseCaseDto request)
        {
            Id = id;
            Request = request ?? throw new ArgumentNullException(nameof(request));
        }
    }
}
