using Core.Interfaces.Gateways;
using Core.Interfaces.Handlers.Orcamentos;

namespace Core.UseCases.Orcamentos.GerarOrcamento
{
    public class GerarOrcamentoHandler : IGerarOrcamentoHandler
    {
        private readonly ILogServicoGateway<GerarOrcamentoHandler> _logServicoGateway;

        public GerarOrcamentoHandler(
            ILogServicoGateway<GerarOrcamentoHandler> logServicoGateway,
            IUnidadeDeTrabalhoGateway udtGateway,
            IUsuarioLogadoServicoGateway usuarioLogadoServicoGateway)
        {
            _logServicoGateway = logServicoGateway ?? throw new ArgumentNullException(nameof(logServicoGateway));
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
                throw;
            }
        }
    }
}
