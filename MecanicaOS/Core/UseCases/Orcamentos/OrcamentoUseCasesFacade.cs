using Core.Entidades;
using Core.Interfaces.UseCases;
using Core.UseCases.Orcamentos.GerarOrcamento;

namespace Core.UseCases.Orcamentos
{
    /// <summary>
    /// Facade para manter compatibilidade com a interface IOrcamentoUseCases
    /// enquanto utiliza os novos casos de uso individuais
    /// </summary>
    public class OrcamentoUseCasesFacade : IOrcamentoUseCases
    {
        private readonly GerarOrcamentoHandler _gerarOrcamentoHandler;

        public OrcamentoUseCasesFacade(GerarOrcamentoHandler gerarOrcamentoHandler)
        {
            _gerarOrcamentoHandler = gerarOrcamentoHandler ?? throw new ArgumentNullException(nameof(gerarOrcamentoHandler));
        }

        public decimal GerarOrcamentoUseCase(OrdemServico ordemServico)
        {
            var useCase = new GerarOrcamentoUseCase(ordemServico);
            var response = _gerarOrcamentoHandler.Handle(useCase);
            return response.ValorOrcamento;
        }
    }
}
