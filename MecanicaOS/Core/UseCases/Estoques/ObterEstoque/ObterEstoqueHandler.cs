using Core.Entidades;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Estoques.ObterEstoque
{
    public class ObterEstoqueHandler : UseCasesAbstrato<ObterEstoqueHandler, Estoque>
    {
        private readonly IEstoqueGateway _estoqueGateway;

        public ObterEstoqueHandler(
            IEstoqueGateway estoqueGateway,
            ILogServico<ObterEstoqueHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _estoqueGateway = estoqueGateway ?? throw new ArgumentNullException(nameof(estoqueGateway));
        }

        public async Task<ObterEstoqueResponse> Handle(ObterEstoqueCommand command)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, command.Id);

                var estoque = await _estoqueGateway.ObterPorIdAsync(command.Id)
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
