using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;

namespace Core.UseCases.Abstrato
{
    public abstract class UseCasesAbstrato<T> where T : class
    {
        private readonly ILogServico<T> _logServico;
        private readonly IUnidadeDeTrabalho _udt;
        protected readonly IUsuarioLogadoServico _usuarioLogadoServico;

        protected UseCasesAbstrato(ILogServico<T> logServico, IUnidadeDeTrabalho udt, IUsuarioLogadoServico usuarioLogadoServico)
        {
            _logServico = logServico ?? throw new ArgumentNullException(nameof(logServico));
            _udt = udt ?? throw new ArgumentNullException(nameof(udt));
            _usuarioLogadoServico = usuarioLogadoServico ?? throw new ArgumentNullException(nameof(usuarioLogadoServico));
        }

        protected async Task<bool> Commit()
        {
            return await _udt.Commit();
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
