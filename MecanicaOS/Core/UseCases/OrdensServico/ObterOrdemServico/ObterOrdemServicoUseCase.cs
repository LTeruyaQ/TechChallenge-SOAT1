namespace Core.UseCases.OrdensServico.ObterOrdemServico
{
    public class ObterOrdemServicoUseCase
    {
        public Guid Id { get; set; }

        public ObterOrdemServicoUseCase(Guid id)
        {
            Id = id;
        }
    }
}
