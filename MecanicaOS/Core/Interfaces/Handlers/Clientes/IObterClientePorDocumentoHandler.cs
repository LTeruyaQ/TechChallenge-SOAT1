using Core.UseCases.Clientes.ObterClientePorDocumento;

namespace Core.Interfaces.Handlers.Clientes
{
    /// <summary>
    /// Interface para o handler de obtenção de cliente por documento
    /// </summary>
    public interface IObterClientePorDocumentoHandler
    {
        /// <summary>
        /// Manipula a operação de obtenção de cliente por documento
        /// </summary>
        /// <param name="documento">Documento do cliente a ser obtido</param>
        /// <returns>Resposta contendo o cliente encontrado</returns>
        Task<ObterClientePorDocumentoResponse> Handle(string documento);
    }
}
