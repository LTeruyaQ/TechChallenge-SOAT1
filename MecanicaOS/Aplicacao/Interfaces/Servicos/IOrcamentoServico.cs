using Dominio.Entidades;

namespace Aplicacao.Interfaces.Servicos;

public interface IOrcamentoServico
{
    decimal GerarOrcamento(OrdemServico ordemServico);
}
