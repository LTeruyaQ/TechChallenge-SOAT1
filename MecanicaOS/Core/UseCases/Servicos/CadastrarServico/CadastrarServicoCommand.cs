using Core.DTOs.UseCases.Servico;

namespace Core.UseCases.Servicos.CadastrarServico
{
    public class CadastrarServicoCommand
    {
        public CadastrarServicoUseCaseDto Request { get; set; }

        public CadastrarServicoCommand(CadastrarServicoUseCaseDto request)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
        }
    }
}
