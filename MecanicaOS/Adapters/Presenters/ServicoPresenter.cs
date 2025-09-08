using Adapters.Presenters.Interfaces;
using Aplicacao.DTOs.Requests.Servico;
using Core.DTOs.Servico;

namespace Adapters.Presenters
{
    public class ServicoPresenter : IServicoPresenter
    {
        public CadastrarServicoUseCaseDto ParaUseCaseDto(CadastrarServicoRequest request)
        {
            if (request == null)
                return null;

            return new CadastrarServicoUseCaseDto
            {
                Nome = request.Nome,
                Descricao = request.Descricao,
                Valor = request.Valor,
                Disponivel = request.Disponivel
            };
        }

        public EditarServicoUseCaseDto ParaUseCaseDto(EditarServicoRequest request)
        {
            if (request == null)
                return null;

            return new EditarServicoUseCaseDto
            {
                Nome = request.Nome,
                Descricao = request.Descricao,
                Valor = request.Valor,
                Disponivel = request.Disponivel
            };
        }
    }
}
