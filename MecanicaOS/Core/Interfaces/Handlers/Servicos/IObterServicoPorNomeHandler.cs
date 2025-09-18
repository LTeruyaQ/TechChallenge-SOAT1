using Core.UseCases.Servicos.ObterServicoPorNome;
using System.Threading.Tasks;

namespace Core.Interfaces.Handlers.Servicos
{
    /// <summary>
    /// Interface para o handler de obtenção de serviço por nome
    /// </summary>
    public interface IObterServicoPorNomeHandler
    {
        /// <summary>
        /// Manipula a operação de obtenção de serviço por nome
        /// </summary>
        /// <param name="nome">Nome do serviço a ser obtido</param>
        /// <returns>Resposta contendo o serviço encontrado</returns>
        Task<ObterServicoPorNomeResponse> Handle(string nome);
    }
}
