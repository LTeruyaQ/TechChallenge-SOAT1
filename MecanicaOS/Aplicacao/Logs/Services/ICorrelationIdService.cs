namespace Aplicacao.Logs.Services
{
    public interface ICorrelationIdService
    {
        string GetCorrelationId();
        void SetCorrelationId(Guid? correlationId = null);
    }
}
