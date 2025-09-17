namespace Core.UseCases.Veiculos.DeletarVeiculo
{
    public class DeletarVeiculoCommand
    {
        public Guid Id { get; set; }

        public DeletarVeiculoCommand(Guid id)
        {
            Id = id;
        }
    }
}
