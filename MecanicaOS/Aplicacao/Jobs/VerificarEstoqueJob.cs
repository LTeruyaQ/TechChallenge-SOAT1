using Dominio.Entidades;
using Dominio.Especificacoes;
using Dominio.Especificacoes.Base.Interfaces;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using System.Text;

namespace Aplicacao.Jobs;

public class VerificarEstoqueJob
{
    private readonly IRepositorio<Estoque> _estoqueRepositorio;
    private readonly IRepositorio<Usuario> _usuarioRepositorio;
    private readonly IRepositorio<AlertaEstoque> _alertaEstoqueRepositorio;
    private readonly ILogServico<VerificarEstoqueJob> _logServico;
    private readonly IServicoEmail _servicoEmail;
    private readonly IUnidadeDeTrabalho _uot;

    public VerificarEstoqueJob(
        IRepositorio<Estoque> estoqueRepositorio,
        IRepositorio<Usuario> usuarioRepositorio,
        IRepositorio<AlertaEstoque> alertaEstoqueRepositorio,
        IServicoEmail notificacaoEmail,
        ILogServico<VerificarEstoqueJob> logServico,
        IUnidadeDeTrabalho uot)
    {
        _estoqueRepositorio = estoqueRepositorio;
        _usuarioRepositorio = usuarioRepositorio;
        _alertaEstoqueRepositorio = alertaEstoqueRepositorio;
        _servicoEmail = notificacaoEmail;
        _logServico = logServico;
        _uot = uot;
    }

    public async Task ExecutarAsync()
    {
        var metodo = nameof(ExecutarAsync);

        try
        {
            _logServico.LogInicio(metodo);

            var insumosCriticos = await ObterInsumosParaAlertaAsync();

            if (insumosCriticos.Any())
            {
                await EnviarAlertaEstoqueAsync(insumosCriticos);

                await SalvarAlertaEnviadoAsync(insumosCriticos);
            }

            _logServico.LogFim(metodo, insumosCriticos);
        }
        catch (Exception e)
        {
            _logServico.LogErro(metodo, e);

            throw;
        }
    }

    private async Task<List<Estoque>> ObterInsumosParaAlertaAsync()
    {
        var filtroInsumosCriticos = new ObterEstoqueCriticoEspecificacao();
        var insumosCriticos = await _estoqueRepositorio.ObterPorFiltroAsync(filtroInsumosCriticos);

        var dataAtual = DateTime.UtcNow;
        var insumosParaAlerta = new List<Estoque>();

        foreach (var insumo in insumosCriticos)
        {
            var alertaEnviadoHoje = await _alertaEstoqueRepositorio.ObterPorFiltroAsync(
                new ObterAlertaDoDiaPorEstoqueEspecificacao(
                    insumo.Id,
                    dataAtual));

            if (!alertaEnviadoHoje.Any())
            {
                insumosParaAlerta.Add(insumo);
            }
        }

        return insumosParaAlerta;
    }


    private async Task EnviarAlertaEstoqueAsync(IEnumerable<Estoque> insumosCriticos)
    {
        var especificacao = new ObterUsuarioParaAlertaEstoqueEspecificacao();
        var usuariosAlerta = await _usuarioRepositorio.ObterPorFiltroAsync(especificacao);

        var conteudo = await GerarConteudoEmailAsync(insumosCriticos);

        if (!string.IsNullOrEmpty(conteudo))
        {
            await _servicoEmail.EnviarAsync(
                usuariosAlerta.Select(u => u.Email),
                "Alerta de Estoque Baixo",
                conteudo
            );
        }
    }

    private static async Task<string?> GerarConteudoEmailAsync(IEnumerable<Estoque> insumosCriticos)
    {
        string templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", "EmailAlertaEstoque.html");
        string template = await File.ReadAllTextAsync(templatePath, Encoding.UTF8);

        var sbItens = new StringBuilder();
        foreach (var insumo in insumosCriticos)
        {
            sbItens.AppendLine($@"
            <li>
                <strong>Insumo:</strong> {insumo.Insumo}<br/>
                <strong>Quantidade Atual:</strong> {insumo.QuantidadeDisponivel}<br/>
                <strong>Quantidade Mínima:</strong> {insumo.QuantidadeMinima}
            </li>
            <br/>");
        }

        string conteudoFinal = template.Replace("{{INSUMOS}}", sbItens.ToString());

        return conteudoFinal;
    }

    private async Task SalvarAlertaEnviadoAsync(IEnumerable<Estoque> insumosCriticos)
    {
        var alertas = insumosCriticos
            .Select(insumo => new AlertaEstoque { EstoqueId = insumo.Id });

        await _alertaEstoqueRepositorio.CadastrarVariosAsync(alertas);

        await _uot.Commit();
    }
}