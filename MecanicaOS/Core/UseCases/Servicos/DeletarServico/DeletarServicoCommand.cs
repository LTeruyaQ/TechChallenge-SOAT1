namespace Core.UseCases.Servicos.DeletarServico
{
    public class DeletarServicoCommand
    {
        public Guid Id { get; set; }

        public DeletarServicoCommand(Guid id)
        {
            Id = id;
        }
    }
}
