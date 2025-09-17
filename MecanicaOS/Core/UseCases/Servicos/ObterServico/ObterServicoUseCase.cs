namespace Core.UseCases.Servicos.ObterServico
{
    public class ObterServicoUseCase
    {
        public Guid Id { get; set; }

        public ObterServicoUseCase(Guid id)
        {
            Id = id;
        }
    }
}
