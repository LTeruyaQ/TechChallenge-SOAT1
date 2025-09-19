using Core.DTOs.UseCases.Servico;
using Core.UseCases.Servicos.CadastrarServico;

namespace Core.Interfaces.Handlers.Servicos
{
    /// <summary>
    /// Interface para o handler de cadastro de serviços
    /// </summary>
    public interface ICadastrarServicoHandler
    {
        /// <summary>
        /// Manipula a operação de cadastro de serviço
        /// </summary>
        /// <param name="request">DTO com os dados do serviço a ser cadastrado</param>
        /// <returns>Resposta contendo o serviço cadastrado</returns>
        Task<CadastrarServicoResponse> Handle(CadastrarServicoUseCaseDto request);
    }
}
