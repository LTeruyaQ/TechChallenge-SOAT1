using Core.DTOs.UseCases.Veiculo;

namespace Core.UseCases.Veiculos.AtualizarVeiculo
{
    public class AtualizarVeiculoCommand
    {
        public Guid Id { get; set; }
        public AtualizarVeiculoUseCaseDto Request { get; set; }

        public AtualizarVeiculoCommand(Guid id, AtualizarVeiculoUseCaseDto request)
        {
            Id = id;
            Request = request ?? throw new ArgumentNullException(nameof(request));
        }
    }
}
