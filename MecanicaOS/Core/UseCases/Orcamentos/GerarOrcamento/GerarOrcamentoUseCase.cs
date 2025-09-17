using Core.Entidades;

namespace Core.UseCases.Orcamentos.GerarOrcamento
{
    public class GerarOrcamentoUseCase
    {
        public OrdemServico OrdemServico { get; set; }

        public GerarOrcamentoUseCase(OrdemServico ordemServico)
        {
            OrdemServico = ordemServico ?? throw new ArgumentNullException(nameof(ordemServico));
        }
    }
}
