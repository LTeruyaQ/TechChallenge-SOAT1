using Dominio.Entidades;
using Dominio.Especificacoes;
using Dominio.Especificacoes.Base.Interfaces;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;

namespace Aplicacao.Jobs;

public class VerificarEstoqueJob
{
    private readonly ICrudRepositorio<Estoque> _estoqueRepositorio;
    private readonly IServicoNotificacaoEmail _notificacaoEmail;
    private readonly ILogServico<VerificarEstoqueJob> _logServico;

    public VerificarEstoqueJob(ICrudRepositorio<Estoque> estoqueRepositorio, 
        IServicoNotificacaoEmail notificacaoEmail, ILogServico<VerificarEstoqueJob> logServico)
    {
        _estoqueRepositorio = estoqueRepositorio;
        _notificacaoEmail = notificacaoEmail;
        _logServico = logServico;
    }

    public async Task ExecutarAsync()
    {
        var metodo = nameof(ExecutarAsync);
        _logServico.LogInicio(metodo);
        try
        {
            IEspecificacao<Estoque> filtro = new ObterEstoqueCriticoEspecificacao();

            IEnumerable<Estoque> insumosCriticos = await _estoqueRepositorio.ObterPorFiltroAsync(filtro);

            if (insumosCriticos.Any())
            {
                await _notificacaoEmail.EnviarAlertaEstoqueAsync(insumosCriticos);
            }

            _logServico.LogFim(metodo, insumosCriticos);
        }
        catch (Exception e)
        {
            _logServico.LogErro(metodo, e);
            throw;
        }
    }
}