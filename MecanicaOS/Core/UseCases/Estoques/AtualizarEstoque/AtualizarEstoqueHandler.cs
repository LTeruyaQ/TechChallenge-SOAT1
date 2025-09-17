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

        public async Task<AtualizarEstoqueResponse> Handle(AtualizarEstoqueCommand command)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, new { command.Id, command.Request });

                var estoque = await _estoqueGateway.ObterPorIdAsync(command.Id)
                    ?? throw new DadosNaoEncontradosException("Estoque não encontrado");

                if (command.Request.Insumo != null) estoque.Insumo = command.Request.Insumo;
                if (command.Request.Descricao != null) estoque.Descricao = command.Request.Descricao;
                if (command.Request.Preco.HasValue) estoque.Preco = command.Request.Preco.Value;
                if (command.Request.QuantidadeDisponivel.HasValue) estoque.QuantidadeDisponivel = command.Request.QuantidadeDisponivel.Value;
                if (command.Request.QuantidadeMinima.HasValue) estoque.QuantidadeMinima = command.Request.QuantidadeMinima.Value;

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
