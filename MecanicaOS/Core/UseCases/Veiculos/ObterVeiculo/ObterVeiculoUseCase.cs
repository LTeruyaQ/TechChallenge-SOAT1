namespace Core.UseCases.Veiculos.ObterVeiculo
{
    public class ObterVeiculoUseCase
    {
        public Guid Id { get; set; }

        public ObterVeiculoUseCase(Guid id)
        {
            Id = id;
        }
    }
}
