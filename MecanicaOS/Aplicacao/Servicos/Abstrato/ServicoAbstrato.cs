using Aplicacao.Interfaces;

namespace Aplicacao.Servicos.Abstrato
{
    public abstract class ServicoAbstrato<T> where T : class
    {
        private readonly ILogServico<T> _logServico;

        protected ServicoAbstrato(ILogServico<T> logServico)
        {
            _logServico = logServico;
        }

        #region Logs
        protected void LogInicio(string metodo, object? props = null)
           => _logServico.LogInicio(metodo, props);

        protected void LogFim(string metodo, object? retorno = null)
            => _logServico.LogFim(metodo, retorno);

        protected void LogErro(string metodo, Exception ex)
            => _logServico.LogErro(metodo, ex);
        #endregion
    }
}
