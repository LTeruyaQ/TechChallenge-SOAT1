using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Estoques.ObterTodosEstoques
{
    public class ObterTodosEstoquesHandler : UseCasesAbstrato<ObterTodosEstoquesHandler>
    {
        private readonly IEstoqueGateway _estoqueGateway;

        public ObterTodosEstoquesHandler(
            IEstoqueGateway estoqueGateway,
            ILogServico<ObterTodosEstoquesHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _estoqueGateway = estoqueGateway ?? throw new ArgumentNullException(nameof(estoqueGateway));
        }

        public async Task<ObterTodosEstoquesResponse> Handle()
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo);

                var estoques = await _estoqueGateway.ObterTodosAsync();

                LogFim(metodo, estoques);

                return new ObterTodosEstoquesResponse { Estoques = estoques };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
