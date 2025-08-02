namespace Dominio.Enumeradores;

public enum StatusOrdemServico
{
    Recebida,
    EmDiagnostico,
    AguardandoAprovacao,
    EmExecucao,
    Finalizada,
    Entregue,
    Cancelada,
    OrcamentoExpirado
}