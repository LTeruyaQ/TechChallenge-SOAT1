using AutoMapper;
using Dominio.Entidades.Abstratos;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;

namespace Aplicacao.Servicos.Abstrato
{
    public abstract class ServicoAbstrato<T, R> where T : class where R : Entidade
    {
        private readonly ILogServico<T> _logServico;
        private readonly IUnidadeDeTrabalho _uot;
        protected readonly IRepositorio<R> _repositorio;
        protected readonly IMapper _mapper;

        protected ServicoAbstrato(IRepositorio<R> repositorio, ILogServico<T> logServico, IUnidadeDeTrabalho uot, IMapper mapper)
        {
            _logServico = logServico ?? throw new ArgumentNullException(nameof(logServico));
            _uot = uot ?? throw new ArgumentNullException(nameof(uot));
            _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
