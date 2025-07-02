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

    public VerificarEstoqueJob(ICrudRepositorio<Estoque> estoqueRepositorio, IServicoNotificacaoEmail notificacaoEmail)
    {
        _estoqueRepositorio = estoqueRepositorio;
        _notificacaoEmail = notificacaoEmail;
    }

    public async Task ExecutarAsync()
    {
        IEspecificacao<Estoque> filtro = new EstoqueCriticoEspecificacao();

        IEnumerable<Estoque> insumosCriticos = await _estoqueRepositorio.ObterPorFiltro(filtro);

        if (insumosCriticos.Any())
        {
            await _notificacaoEmail.EnviarAlertaEstoqueAsync(insumosCriticos);
        }
    }
}