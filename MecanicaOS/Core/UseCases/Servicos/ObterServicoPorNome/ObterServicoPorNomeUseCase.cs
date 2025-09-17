namespace Core.UseCases.Servicos.ObterServicoPorNome
{
    public class ObterServicoPorNomeUseCase
    {
        public string Nome { get; set; }

        public ObterServicoPorNomeUseCase(string nome)
        {
            Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        }
    }
}
