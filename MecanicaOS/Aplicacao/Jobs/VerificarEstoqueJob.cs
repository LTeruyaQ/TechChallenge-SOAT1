using Dominio.Entidades;
using Dominio.Especificacoes;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using System.Text;

namespace Aplicacao.Jobs;

public class VerificarEstoqueJob(
    IRepositorio<Estoque> estoqueRepositorio,
    IRepositorio<Usuario> usuarioRepositorio,
    IRepositorio<AlertaEstoque> alertaEstoqueRepositorio,
    IServicoEmail notificacaoEmail,
    ILogServico<VerificarEstoqueJob> logServico,
    IUnidadeDeTrabalho uot)
{
    private readonly IRepositorio<Estoque> _estoqueRepositorio = estoqueRepositorio;
    private readonly IRepositorio<Usuario> _usuarioRepositorio = usuarioRepositorio;
    private readonly IRepositorio<AlertaEstoque> _alertaEstoqueRepositorio = alertaEstoqueRepositorio;
    private readonly ILogServico<VerificarEstoqueJob> _logServico = logServico;
    private readonly IServicoEmail _servicoEmail = notificacaoEmail;
    private readonly IUnidadeDeTrabalho _uot = uot;

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
        IEnumerable<Estoque> insumosCriticos = await ObterInsumosAbaixoDaQuantidadeMinima();

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

    private async Task<IEnumerable<Estoque>> ObterInsumosAbaixoDaQuantidadeMinima()
    {
        var metodo = nameof(ObterInsumosAbaixoDaQuantidadeMinima);
        _logServico.LogInicio(metodo);
        try
        {
            var filtroInsumosCriticos = new ObterEstoqueCriticoEspecificacao();
            var insumosCriticos = await _estoqueRepositorio.ObterPorFiltroAsync(filtroInsumosCriticos);

            _logServico.LogFim(metodo, insumosCriticos);
            return insumosCriticos;
        }
        catch (Exception e)
        {
            _logServico.LogErro(metodo, e);
            throw;
        }
    }

    private async Task EnviarAlertaEstoqueAsync(IEnumerable<Estoque> insumosCriticos)
    {
        var metodo = nameof(EnviarAlertaEstoqueAsync);

        _logServico.LogInicio(metodo, insumosCriticos);
        try
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

            _logServico.LogFim(metodo);
        }
        catch (Exception e)
        {
            _logServico.LogErro(metodo, e);
            throw;
        }
    }

    private async Task<string?> GerarConteudoEmailAsync(IEnumerable<Estoque> insumosCriticos)
    {
        var metodo = nameof(GerarConteudoEmailAsync);
        _logServico.LogInicio(metodo, insumosCriticos);

        try
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

            _logServico.LogFim(metodo, conteudoFinal);
            return conteudoFinal;
        }
        catch (Exception e)
        {
            _logServico.LogErro(metodo, e);
            throw;
        }
    }

    private async Task SalvarAlertaEnviadoAsync(IEnumerable<Estoque> insumosCriticos)
    {
        var metodo = nameof(SalvarAlertaEnviadoAsync);
        _logServico.LogInicio(metodo, insumosCriticos);
        try
        {
            var alertas = insumosCriticos
                .Select(insumo => new AlertaEstoque { EstoqueId = insumo.Id });

            await _alertaEstoqueRepositorio.CadastrarVariosAsync(alertas);

            if (!await _uot.Commit())
                throw new PersistirDadosException("Falha ao cadastrar alertas de estoque");

            _logServico.LogFim(metodo);
        }
        catch (Exception e)
        {
            _logServico.LogErro(metodo, e);
            throw;
        }
    }
}