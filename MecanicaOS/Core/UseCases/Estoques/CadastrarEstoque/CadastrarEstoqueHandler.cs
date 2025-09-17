using Core.Entidades;
using Core.Exceptions;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.UseCases.Abstrato;

namespace Core.UseCases.Estoques.CadastrarEstoque
{
    public class CadastrarEstoqueHandler : UseCasesAbstrato<CadastrarEstoqueHandler, Estoque>
    {
        private readonly IEstoqueGateway _estoqueGateway;

        public CadastrarEstoqueHandler(
            IEstoqueGateway estoqueGateway,
            ILogServico<CadastrarEstoqueHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
            : base(logServico, udt, usuarioLogadoServico)
        {
            _estoqueGateway = estoqueGateway ?? throw new ArgumentNullException(nameof(estoqueGateway));
        }

        public async Task<CadastrarEstoqueResponse> Handle(CadastrarEstoqueCommand command)
        {
            string metodo = nameof(Handle);

            try
            {
                LogInicio(metodo, command.Request);

                // Verificar se já existe um estoque com o mesmo nome
                var estoques = await _estoqueGateway.ObterTodosAsync();
                if (estoques.Any(e => e.Insumo.Equals(command.Request.Insumo, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new DadosJaCadastradosException("Produto já cadastrado");
                }

                Estoque estoque = new()
                {
                    Insumo = command.Request.Insumo,
                    Descricao = command.Request.Descricao,
                    QuantidadeDisponivel = command.Request.QuantidadeDisponivel,
                    QuantidadeMinima = command.Request.QuantidadeMinima,
                    Preco = command.Request.Preco
                };

                await _estoqueGateway.CadastrarAsync(estoque);

                if (!await Commit())
                    throw new PersistirDadosException("Erro ao cadastrar estoque");

                LogFim(metodo, estoque);

                return new CadastrarEstoqueResponse { Estoque = estoque };
            }
            catch (Exception e)
            {
                LogErro(metodo, e);
                throw;
            }
        }
    }
}
