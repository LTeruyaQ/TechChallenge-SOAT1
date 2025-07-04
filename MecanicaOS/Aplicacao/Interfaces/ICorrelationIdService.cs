namespace Aplicacao.Interfaces
{
    public interface ICorrelationIdService
    {
        string GetCorrelationId();
        void SetCorrelationId(Guid? correlationId = null);
    }
}
