using Core.DTOs.UseCases.Veiculo;

namespace Core.UseCases.Veiculos.CadastrarVeiculo
{
    public class CadastrarVeiculoCommand
    {
        public CadastrarVeiculoUseCaseDto Request { get; set; }

        public CadastrarVeiculoCommand(CadastrarVeiculoUseCaseDto request)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
        }
    }
}
