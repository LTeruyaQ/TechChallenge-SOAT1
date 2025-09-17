using Core.Entidades;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Estoques.DeletarEstoque
{
    public class DeletarEstoqueHandler : UseCasesAbstrato<DeletarEstoqueHandler, Estoque>
    {
        private readonly IEstoqueGateway _estoqueGateway;

        public DeletarEstoqueHandler(
            IEstoqueGateway estoqueGateway,
            ILogServico<DeletarEstoqueHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _estoqueGateway = estoqueGateway ?? throw new ArgumentNullException(nameof(estoqueGateway));
        }

        public async Task<DeletarEstoqueResponse> Handle(Guid id)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, id);

                var estoque = await _estoqueGateway.ObterPorIdAsync(id)
                    ?? throw new DadosNaoEncontradosException("Estoque não encontrado");

                await _estoqueGateway.DeletarAsync(estoque);
                var sucesso = await Commit();

                LogFim(metodo, sucesso);

                return new DeletarEstoqueResponse { Sucesso = sucesso };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
