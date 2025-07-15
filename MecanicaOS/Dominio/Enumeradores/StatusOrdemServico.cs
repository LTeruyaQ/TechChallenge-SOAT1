namespace Dominio.Enumeradores;

public enum StatusOrdemServico
{
    Recebida,
    EmDiagnostico,
    AguardandoAprovação,
    EmExecucao,
    Finalizada,
    Entregue,
    Cancelada
}