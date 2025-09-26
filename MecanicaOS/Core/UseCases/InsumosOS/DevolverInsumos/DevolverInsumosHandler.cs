using Core.DTOs.Requests.OrdemServico.InsumoOS;
using Core.DTOs.UseCases.Estoque;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.Estoques;
using Core.Interfaces.Handlers.InsumosOS;
using Core.UseCases.Abstrato;

namespace Core.UseCases.InsumosOS.DevolverInsumos
{
    public class DevolverInsumosHandler : UseCasesHandlerAbstrato<DevolverInsumosHandler>, IDevolverInsumosHandler
    {
        private readonly IObterEstoqueHandler _obterEstoqueHandler;
        private readonly IAtualizarEstoqueHandler _atualizarEstoqueHandler;

        public DevolverInsumosHandler(
            IObterEstoqueHandler obterEstoqueHandler,
            IAtualizarEstoqueHandler atualizarEstoqueHandler,
            ILogServicoGateway<DevolverInsumosHandler> logServicoGateway,
            IUnidadeDeTrabalhoGateway udtGateway,
            IUsuarioLogadoServicoGateway usuarioLogadoServicoGateway)
            : base(logServicoGateway, udtGateway, usuarioLogadoServicoGateway)
        {
            _obterEstoqueHandler = obterEstoqueHandler ?? throw new ArgumentNullException(nameof(obterEstoqueHandler));
            _atualizarEstoqueHandler = atualizarEstoqueHandler ?? throw new ArgumentNullException(nameof(atualizarEstoqueHandler));
        }

        public async Task<DevolverInsumosResponse> Handle(IEnumerable<DevolverInsumoOSRequest> insumosOS)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, new { InsumosCount = insumosOS.Count() });

                foreach (var insumoOS in insumosOS)
                {
                    var estoqueResponse = await _obterEstoqueHandler.Handle(insumoOS.EstoqueId);
                    var estoque = estoqueResponse.Estoque
                        ?? throw new DadosNaoEncontradosException($"Estoque com ID {insumoOS.EstoqueId} não encontrado");

                    estoque.QuantidadeDisponivel += insumoOS.Quantidade;

                    await _atualizarEstoqueHandler.Handle(estoque.Id, new AtualizarEstoqueUseCaseDto
                    {
                        Insumo = estoque.Insumo,
                        QuantidadeDisponivel = estoque.QuantidadeDisponivel,
                        QuantidadeMinima = estoque.QuantidadeMinima,
                        Preco = estoque.Preco
                    });
                }

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao devolver insumos ao estoque");

                LogFim(metodo, true);

                return new DevolverInsumosResponse { Sucesso = true };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
