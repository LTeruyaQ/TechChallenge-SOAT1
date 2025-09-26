using Core.Interfaces.Gateways;
using Core.Interfaces.Servicos;

namespace Adapters.Gateways
{
    public class LogServicoGateway<T> : ILogServicoGateway<T> where T : class
    {
        private readonly ILogServico<T> _logServico;

        public LogServicoGateway(ILogServico<T> logServico)
        {
            _logServico = logServico ?? throw new ArgumentNullException(nameof(logServico));
        }

        public void LogInicio(string metodo, object? props = null)
        {
            _logServico.LogInicio(metodo, props);
        }

        public void LogFim(string metodo, object? retorno = null)
        {
            _logServico.LogFim(metodo, retorno);
        }

        public void LogErro(string metodo, Exception ex)
        {
            _logServico.LogErro(metodo, ex);
        }
    }
}
