using Dominio.Entidades;
using Dominio.Especificacoes;
using Dominio.Especificacoes.Base.Interfaces;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;

namespace Aplicacao.Jobs;

public class VerificarEstoqueJob
{
    private readonly IRepositorio<Estoque> _estoqueRepositorio;
    private readonly IRepositorio<Usuario> _usuarioRepositorio;
    private readonly IRepositorio<AlertaEstoque> _alertaEstoqueRepositorio;
    private readonly IServicoEmail _servicoEmail;
    private readonly ILogServico<VerificarEstoqueJob> _logServico;

    public VerificarEstoqueJob(
        IRepositorio<Estoque> estoqueRepositorio, 
        IRepositorio<Usuario> usuarioRepositorio,
        IRepositorio<AlertaEstoque> alertaEstoqueRepositorio,
        IServicoEmail notificacaoEmail, 
        ILogServico<VerificarEstoqueJob> logServico)
    {
        _estoqueRepositorio = estoqueRepositorio;
        _usuarioRepositorio = usuarioRepositorio;
        _alertaEstoqueRepositorio = alertaEstoqueRepositorio;
        _servicoEmail = notificacaoEmail;
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
                await EnviarAlertaEstoqueAsync(insumosCriticos);
            }

            _logServico.LogFim(metodo, insumosCriticos);
        }
        catch (Exception e)
        {
            _logServico.LogErro(metodo, e);

            throw;
        }
    }

    private async Task EnviarAlertaEstoqueAsync(IEnumerable<Estoque> insumosCriticos)
    {
        IEspecificacao<Usuario> especificacao = new ObterUsuarioParaAlertaEstoqueEspecificacao();

        IEnumerable<Usuario> usuariosAlerta = await _usuarioRepositorio.ObterPorFiltroAsync(especificacao);

        string? conteudo = await GerarConteudoEmail(insumosCriticos);

        if (!string.IsNullOrEmpty(conteudo))
        {
            await _servicoEmail.EnviarAsync(
                usuariosAlerta.Select(u => u.Email),
                "Alerta de Estoque Baixo",
                conteudo
            );
        }
    }

    private async Task<string?> GerarConteudoEmail(IEnumerable<Estoque> insumosCriticos)
    {
        string conteudo = "Foram identificados insumos com quantidade igual ou inferior ao limite mínimo definido no sistema.\r\n" +
                          "Itens com estoque crítico:\r\n" +
                          "-----------------------------------------------------";

        bool enviarAlertaHoje = false;

        foreach (var insumo in insumosCriticos)
        {
            IEspecificacao<AlertaEstoque> expressaoAlerta = new ObterAlertaDoDiaPorEstoqueEspecificacao (insumo.Id, DateTime.UtcNow);

            var alertasHoje = await _alertaEstoqueRepositorio.ObterPorFiltroAsync(expressaoAlerta);

            if (!alertasHoje.Any())
            {
                conteudo += $"• Insumo: {insumo.Insumo}\n" +
                            $"  Quantidade Atual: {insumo.QuantidadeDisponivel}\n" +
                            $"  Quantidade Mínima: {insumo.QuantidadeMinima}\n";

                enviarAlertaHoje = true;
            }
        }

        conteudo += "-----------------------------------------------------" +
            "Recomenda-se a verificação e reposição dos itens listados para evitar impacto nas operações.\r\n\r\n" +
            "Este é um aviso automático gerado pelo sistema OficinaOS";

        return enviarAlertaHoje ? conteudo : null;
    }
}