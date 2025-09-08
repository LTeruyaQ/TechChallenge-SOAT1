using Adapters.DTOs.Requests.OrdemServico;
using Adapters.DTOs.Requests.OrdemServico.InsumoOS;
using Adapters.DTOs.Responses.OrdemServico;
using Adapters.DTOs.Responses.OrdemServico.InsumoOrdemServico;
using Adapters.Presenters.Interfaces;
using Core.DTOs.OrdemServico;
using Core.DTOs.OrdemServico.InsumoOS;
using Core.Entidades;

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

        public OrdemServicoResponse ParaResponse(OrdemServico ordemServico)
        {
            if (ordemServico == null)
                return null;

            return new OrdemServicoResponse
            {
                Id = ordemServico.Id,
                ClienteId = ordemServico.ClienteId,
                VeiculoId = ordemServico.VeiculoId,
                ServicoId = ordemServico.ServicoId,
                Orcamento = ordemServico.Orcamento,
                Descricao = ordemServico.Descricao,
                Status = ordemServico.Status,
                DataEnvioOrcamento = ordemServico.DataEnvioOrcamento,
                Insumos = ordemServico.Insumos?.Select(i => new InsumoOSResponse
                {
                    OrdemServicoId = i.OrdemServicoId,
                    EstoqueId = i.EstoqueId,
                    Quantidade = i.Quantidade
                })
            };
        }

        public IEnumerable<OrdemServicoResponse> ParaResponse(IEnumerable<OrdemServico> ordensServico)
        {
            if (ordensServico == null)
                return new List<OrdemServicoResponse>();

            return ordensServico.Select(ParaResponse).ToList();
        }
    }
}
