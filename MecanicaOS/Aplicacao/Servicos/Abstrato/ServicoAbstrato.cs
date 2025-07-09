using Dominio.Entidades.Abstratos;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;

namespace Aplicacao.Servicos.Abstrato
{
    public abstract class ServicoAbstrato<T, R> where T : class where R : Entidade
    {
        private readonly ILogServico<T> _logServico;
        private readonly IUnidadeDeTrabalho _uot;
        protected readonly ICrudRepositorio<R> _repositorio;

        protected ServicoAbstrato(ICrudRepositorio<R> repositorio, ILogServico<T> logServico, IUnidadeDeTrabalho uot)
        {
            _logServico = logServico;
            _uot = uot;
            _repositorio = repositorio;
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
