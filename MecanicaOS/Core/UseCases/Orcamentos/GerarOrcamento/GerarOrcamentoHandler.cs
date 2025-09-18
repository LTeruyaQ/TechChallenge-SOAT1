using Core.Interfaces.Handlers.Orcamentos;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;

namespace Core.UseCases.Orcamentos.GerarOrcamento
{
    public class GerarOrcamentoHandler : IGerarOrcamentoHandler
    {
        private readonly ILogServico<GerarOrcamentoHandler> _logServico;

        public GerarOrcamentoHandler(
            ILogServico<GerarOrcamentoHandler> logServico,
            IUnidadeDeTrabalho udt,
            IUsuarioLogadoServico usuarioLogadoServico)
        {
            _logServico = logServico ?? throw new ArgumentNullException(nameof(logServico));
        }

        public GerarOrcamentoResponse Handle(GerarOrcamentoUseCase useCase)
        {
            string metodo = nameof(Handle);

            try
            {
                decimal precoServico = useCase.OrdemServico.Servico!.Valor;
                decimal precoInsumos = useCase.OrdemServico.InsumosOS.Sum(i =>
                    i.Quantidade * i.Estoque.Preco);

                decimal valorTotal = precoServico + precoInsumos;

                return new GerarOrcamentoResponse { ValorOrcamento = valorTotal };
            }
            catch (Exception e)
            {
                // Log simplificado por enquanto
                throw;
            }
        }
    }
}
