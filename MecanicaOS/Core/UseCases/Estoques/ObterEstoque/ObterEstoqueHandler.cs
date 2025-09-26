using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.Estoques;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Estoques.ObterEstoque
{
    public class ObterEstoqueHandler : UseCasesHandlerAbstrato<ObterEstoqueHandler>, IObterEstoqueHandler
    {
        private readonly IEstoqueGateway _estoqueGateway;

        public ObterEstoqueHandler(
            IEstoqueGateway estoqueGateway,
            ILogServicoGateway<ObterEstoqueHandler> logServicoGateway,
            IUnidadeDeTrabalhoGateway udtGateway,
            IUsuarioLogadoServicoGateway usuarioLogadoServicoGateway)
            : base(logServicoGateway, udtGateway, usuarioLogadoServicoGateway)
        {
            _estoqueGateway = estoqueGateway ?? throw new ArgumentNullException(nameof(estoqueGateway));
        }

        public async Task<ObterEstoqueResponse> Handle(Guid id)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, id);

                var estoque = await _estoqueGateway.ObterPorIdAsync(id)
                    ?? throw new DadosNaoEncontradosException("Estoque não encontrado");

                LogFim(metodo, estoque);

                return new ObterEstoqueResponse { Estoque = estoque };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
