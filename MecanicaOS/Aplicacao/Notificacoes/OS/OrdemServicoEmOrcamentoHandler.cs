using Aplicacao.Interfaces.Servicos;
using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Especificacoes.OrdemServico;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using MediatR;
using System.Text;
using System.Text.RegularExpressions;

namespace Aplicacao.Notificacoes.OS;

public class OrdemServicoEmOrcamentoHandler(
    IRepositorio<OrdemServico> ordemServicoRepositorio,
    IOrcamentoServico orcamentoServico,
    IServicoEmail emailServico,
    ILogServico<OrdemServicoEmOrcamentoHandler> logServico,
    IUnidadeDeTrabalho uot) : INotificationHandler<OrdemServicoEmOrcamentoEvent>
{
    private readonly IRepositorio<OrdemServico> _ordemServicoRepositorio = ordemServicoRepositorio;
    private readonly IOrcamentoServico _orcamentoServico = orcamentoServico;
    private readonly IServicoEmail _emailServico = emailServico;
    private readonly IUnidadeDeTrabalho _uot = uot;
    private readonly ILogServico<OrdemServicoEmOrcamentoHandler> _logServico = logServico;

    public async Task Handle(OrdemServicoEmOrcamentoEvent notification, CancellationToken cancellationToken)
    {
        var metodo = nameof(Handle);

        try
        {
            _logServico.LogInicio(metodo, notification.OrdemServicoId);

            var especificacao = new ObterOrdemServicoPorIdComIncludeEspecificacao(notification.OrdemServicoId);
            var os = await _ordemServicoRepositorio.ObterUmAsync(especificacao);

            if (os is null) return;

            os.Orcamento = _orcamentoServico.GerarOrcamento(os);
            os.Status = StatusOrdemServico.AguardandoAprovação;
            os.DataEnvioOrcamento = DateTime.UtcNow;

            await EnviarOrcamentoAsync(os);

            await _ordemServicoRepositorio.EditarAsync(os);
            await _uot.Commit();

            _logServico.LogFim(metodo);
        }
        catch (Exception e)
        {
            _logServico.LogErro(metodo, e);

            throw;
        }
    }

    private async Task EnviarOrcamentoAsync(OrdemServico os)
    {
        string conteudo = await GerarConteudoEmailAsync(os);

        await _emailServico.EnviarAsync(
            [os.Cliente.Contato.Email],
            "Orçamento de Serviço",
            conteudo);
    }

    private static async Task<string> GerarConteudoEmailAsync(OrdemServico os)
    {
        const string templateFileName = "EmailOrcamentoOS.html";

        string templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", templateFileName);
        string template = await File.ReadAllTextAsync(templatePath, Encoding.UTF8);

        template = template
            .Replace("{{NOME_CLIENTE}}", os.Cliente.Nome)
            .Replace("{{NOME_SERVICO}}", os.Servico.Nome)
            .Replace("{{VALOR_SERVICO}}", os.Servico.Valor.ToString("N2"))
            .Replace("{{VALOR_TOTAL}}", os.Orcamento!.Value.ToString("N2"));

        string insumosHtml = GerarHtmlInsumos(os.InsumosOS);
        template = Regex.Replace(template, @"{{#each INSUMOS}}(.*?){{/each}}", insumosHtml, RegexOptions.Singleline);

        return template;
    }

    private static string GerarHtmlInsumos(IEnumerable<InsumoOS> insumosOS)
    {
        return string.Join(Environment.NewLine, insumosOS.Select(i =>
        {
            var descricao = i.Estoque.Insumo;
            var quantidade = i.Quantidade;
            var precoTotal = quantidade * i.Estoque.Preco;
            return $"""
                        <tr>
                            <td>{descricao} ({quantidade} und)</td>
                            <td>R$ {precoTotal:N2}</td>
                        </tr>
                    """;
        }));
    }
}