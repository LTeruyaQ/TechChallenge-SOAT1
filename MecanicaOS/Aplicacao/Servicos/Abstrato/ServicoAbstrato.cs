using Aplicacao.Interfaces;
using Dominio.Interfaces.Repositorios;

namespace Aplicacao.Servicos.Abstrato
{
    public abstract class ServicoAbstrato<T> where T : class
    {
        private readonly ILogServico<T> _logServico;
        private readonly IUnidadeDeTrabalho _uot;

        protected ServicoAbstrato(ILogServico<T> logServico, IUnidadeDeTrabalho uot)
        {
            _logServico = logServico;
            _uot = uot;
        }

        protected async Task<bool> Commit()
        {
            return await _uot.Commit();
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
