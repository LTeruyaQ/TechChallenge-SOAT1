using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Especificacoes.OrdemServico;
using Dominio.Exceptions;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using MediatR;
using System.Text;
using System.Text.RegularExpressions;

namespace Aplicacao.Notificacoes.OS;

public class OrdemServicoEmOrcamentoHandler(
    IRepositorio<OrdemServico> ordemServicoRepositorio,
    IRepositorio<Orcamento> orcamentoRepositorio,
    IServicoEmail emailServico,
    ILogServico<OrdemServicoEmOrcamentoHandler> logServico,
    IUnidadeDeTrabalho udt) : INotificationHandler<OrdemServicoEmOrcamentoEvent>
{
    private readonly IRepositorio<Orcamento> _orcamentoRepositorio = orcamentoRepositorio;
    private readonly IRepositorio<OrdemServico> _ordemServicoRepositorio = ordemServicoRepositorio;
    private readonly IServicoEmail _emailServico = emailServico;
    private readonly IUnidadeDeTrabalho _uot = udt;
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

            await CadastrarOrcamento(os);
            
            await EnviarOrcamento(os);
            
            _logServico.LogFim(metodo);
        }
        catch (Exception e)
        {
            _logServico.LogErro(metodo, e);

            throw;
        }
    }

    private async Task CadastrarOrcamento(OrdemServico os)
    {
        os.GerarOrcamento();

        await _orcamentoRepositorio.CadastrarAsync(os.Orcamento!);

        if (await _uot.Commit())
            throw new PersistirDadosException("Erro ao cadastrar o orçamento na base de dados");
    }

    private async Task EnviarOrcamento(OrdemServico os)
    {
        os.PrepararOrcamentoParaEnvio();

        await EnviarOrcamentoAsync(os);

        await _ordemServicoRepositorio.EditarAsync(os);

        if (await _uot.Commit())
            throw new PersistirDadosException("Erro ao salvar o envio do orçamento na base de dados");
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
            .Replace("{{VALOR_TOTAL}}", os.Orcamento!.Valor.ToString("N2"));

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