namespace Dominio.Interfaces.Servicos
{
    public interface ICorrelationIdService
    {
        string GetCorrelationId();
        void SetCorrelationId(Guid? correlationId = null);
    }
}
