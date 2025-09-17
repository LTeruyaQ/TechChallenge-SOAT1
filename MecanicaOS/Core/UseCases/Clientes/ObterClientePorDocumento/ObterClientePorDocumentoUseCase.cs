namespace Core.UseCases.Clientes.ObterClientePorDocumento
{
    public class ObterClientePorDocumentoUseCase
    {
        public string Documento { get; set; }

        public ObterClientePorDocumentoUseCase(string documento)
        {
            Documento = documento ?? throw new ArgumentNullException(nameof(documento));
        }
    }
}
