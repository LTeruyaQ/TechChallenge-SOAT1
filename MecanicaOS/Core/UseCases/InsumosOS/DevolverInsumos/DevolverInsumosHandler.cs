using Core.Entidades;
using Core.Exceptions;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;
using Core.UseCases.Abstrato;

namespace Core.UseCases.InsumosOS.DevolverInsumos
{
    public class DevolverInsumosHandler : UseCasesAbstrato<DevolverInsumosHandler, InsumoOS>
    {
        private readonly IEstoqueUseCases _estoqueUseCases;

        public DevolverInsumosHandler(
            IEstoqueUseCases estoqueUseCases,
            ILogServico<DevolverInsumosHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _estoqueUseCases = estoqueUseCases ?? throw new ArgumentNullException(nameof(estoqueUseCases));
        }

        public async Task<DevolverInsumosResponse> Handle(IEnumerable<InsumoOS> insumosOS)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, new { InsumosCount = insumosOS.Count() });

                foreach (var insumoOS in insumosOS)
                {
                    var estoque = await _estoqueUseCases.ObterPorIdUseCaseAsync(insumoOS.EstoqueId);

                    // Devolver quantidade ao estoque
                    estoque.QuantidadeDisponivel += insumoOS.Quantidade;

                    await _estoqueUseCases.AtualizarUseCaseAsync(estoque.Id, new Core.DTOs.UseCases.Estoque.AtualizarEstoqueUseCaseDto
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
