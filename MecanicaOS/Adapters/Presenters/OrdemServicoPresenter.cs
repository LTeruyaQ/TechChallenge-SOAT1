using Adapters.DTOs.Requests.OrdemServico;
using Adapters.DTOs.Requests.OrdemServico.InsumoOS;
using Adapters.Presenters.Interfaces;
using Core.DTOs.OrdemServico;
using Core.DTOs.OrdemServico.InsumoOS;

namespace Adapters.Presenters
{
    public class OrdemServicoPresenter : IOrdemServicoPresenter
    {
        public CadastrarOrdemServicoUseCaseDto ParaUseCaseDto(CadastrarOrdemServicoRequest request)
        {
            if (request == null)
                return null;

            return new CadastrarOrdemServicoUseCaseDto
            {
                ClienteId = request.ClienteId,
                VeiculoId = request.VeiculoId,
                ServicoId = request.ServicoId,
                Descricao = request.Descricao
            };
        }

        public AtualizarOrdemServicoUseCaseDto ParaUseCaseDto(AtualizarOrdemServicoRequest request)
        {
            if (request == null)
                return null;

            return new AtualizarOrdemServicoUseCaseDto
            {
                ClienteId = request.ClienteId,
                VeiculoId = request.VeiculoId,
                ServicoId = request.ServicoId,
                Descricao = request.Descricao,
                Status = request.Status
            };
        }

        public CadastrarInsumoOSUseCaseDto ParaUseCaseDto(CadastrarInsumoOSRequest request)
        {
            if (request == null)
                return null;

            return new CadastrarInsumoOSUseCaseDto
            {
                EstoqueId = request.EstoqueId,
                Quantidade = request.Quantidade
            };
        }
    }
}
