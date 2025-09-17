using Core.DTOs.UseCases.OrdemServico;

namespace Core.UseCases.OrdensServico.CadastrarOrdemServico
{
    public class CadastrarOrdemServicoCommand
    {
        public CadastrarOrdemServicoUseCaseDto Request { get; set; }

        public CadastrarOrdemServicoCommand(CadastrarOrdemServicoUseCaseDto request)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
        }
    }
}
