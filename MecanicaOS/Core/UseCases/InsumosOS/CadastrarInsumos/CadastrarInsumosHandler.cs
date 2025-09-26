using Core.DTOs.UseCases.Estoque;
using Core.DTOs.UseCases.OrdemServico.InsumoOS;
using Core.Entidades;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.InsumosOS;
using Core.Interfaces.UseCases;
using Core.UseCases.Abstrato;

namespace Core.UseCases.InsumosOS.CadastrarInsumos
{
    public class CadastrarInsumosHandler : UseCasesHandlerAbstrato<CadastrarInsumosHandler>, ICadastrarInsumosHandler
    {
        private readonly IOrdemServicoUseCases _ordemServicoUseCases;
        private readonly IEstoqueUseCases _estoqueUseCases;
        private readonly IVerificarEstoqueJobGateway _verificarEstoqueJobGateway;
        public CadastrarInsumosHandler(
            IOrdemServicoUseCases ordemServicoUseCases,
            IEstoqueUseCases estoqueUseCases,
            ILogGateway<CadastrarInsumosHandler> logServicoGateway,
            IUnidadeDeTrabalhoGateway udtGateway,
            IUsuarioLogadoServicoGateway usuarioLogadoServicoGateway,
            IVerificarEstoqueJobGateway verificarEstoqueJobGateway)
            : base(logServicoGateway, udtGateway, usuarioLogadoServicoGateway)
        {
            _ordemServicoUseCases = ordemServicoUseCases ?? throw new ArgumentNullException(nameof(ordemServicoUseCases));
            _estoqueUseCases = estoqueUseCases ?? throw new ArgumentNullException(nameof(estoqueUseCases));
            _verificarEstoqueJobGateway = verificarEstoqueJobGateway ?? throw new ArgumentNullException(nameof(verificarEstoqueJobGateway));
        }

        public async Task<CadastrarInsumosResponse> Handle(Guid ordemServicoId, List<CadastrarInsumoOSUseCaseDto> request)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, new { ordemServicoId, InsumosCount = request.Count });

                var ordemServico = await _ordemServicoUseCases.ObterPorIdUseCaseAsync(ordemServicoId)
                    ?? throw new DadosNaoEncontradosException("Ordem de serviço não encontrada");

                var insumosOS = new List<InsumoOS>();

                foreach (var insumoDto in request)
                {
                    var estoque = await _estoqueUseCases.ObterPorIdUseCaseAsync(insumoDto.EstoqueId);

                    if (estoque.QuantidadeDisponivel < insumoDto.Quantidade)
                        throw new DadosInvalidosException($"Estoque insuficiente para o insumo {estoque.Insumo}");

                    var insumoOS = new InsumoOS
                    {
                        OrdemServicoId = ordemServicoId,
                        EstoqueId = insumoDto.EstoqueId,
                        Quantidade = insumoDto.Quantidade,
                        Estoque = estoque
                    };

                    insumosOS.Add(insumoOS);

                    estoque.QuantidadeDisponivel -= insumoDto.Quantidade;
                    await _estoqueUseCases.AtualizarUseCaseAsync(estoque.Id, new AtualizarEstoqueUseCaseDto
                    {
                        Insumo = estoque.Insumo,
                        QuantidadeDisponivel = estoque.QuantidadeDisponivel,
                        QuantidadeMinima = estoque.QuantidadeMinima,
                        Preco = estoque.Preco
                    });

                    if (estoque.QuantidadeDisponivel <= estoque.QuantidadeMinima)
                    {
                        await _verificarEstoqueJobGateway.VerificarEstoqueAsync();
                    }
                }

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao cadastrar insumos na ordem de serviço");

                var response = new CadastrarInsumosResponse { InsumosOS = insumosOS.ToList() };
                LogFim(metodo, response);

                return response;
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
