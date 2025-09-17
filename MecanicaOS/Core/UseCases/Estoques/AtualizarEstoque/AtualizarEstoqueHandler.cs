using Core.DTOs.UseCases.Estoque;
using Core.Entidades;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Estoques.AtualizarEstoque
{
    public class AtualizarEstoqueHandler : UseCasesAbstrato<AtualizarEstoqueHandler, Estoque>
    {
        private readonly IEstoqueGateway _estoqueGateway;

        public AtualizarEstoqueHandler(
            IEstoqueGateway estoqueGateway,
            ILogServico<AtualizarEstoqueHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _estoqueGateway = estoqueGateway ?? throw new ArgumentNullException(nameof(estoqueGateway));
        }

        public async Task<AtualizarEstoqueResponse> Handle(Guid id, AtualizarEstoqueUseCaseDto request)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, new { id, request });

                var estoque = await _estoqueGateway.ObterPorIdAsync(id)
                    ?? throw new DadosNaoEncontradosException("Estoque não encontrado");

                if (request.Insumo != null) estoque.Insumo = request.Insumo;
                if (request.Descricao != null) estoque.Descricao = request.Descricao;
                if (request.Preco.HasValue) estoque.Preco = request.Preco.Value;
                if (request.QuantidadeDisponivel.HasValue) estoque.QuantidadeDisponivel = request.QuantidadeDisponivel.Value;
                if (request.QuantidadeMinima.HasValue) estoque.QuantidadeMinima = request.QuantidadeMinima.Value;

                estoque.DataAtualizacao = DateTime.UtcNow;

                await _estoqueGateway.EditarAsync(estoque);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao atualizar estoque");

                LogFim(metodo, estoque);

                return new AtualizarEstoqueResponse { Estoque = estoque };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
