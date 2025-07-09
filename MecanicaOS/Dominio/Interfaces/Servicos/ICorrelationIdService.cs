namespace Dominio.Interfaces.Servicos
{
    public interface IIdCorrelacionalService
    {
        string GetCorrelationId();
        void SetCorrelationId(Guid? correlationId = null);
    }
}
