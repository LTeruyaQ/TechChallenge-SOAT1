using Dominio.Entidades;
using Dominio.Especificacoes;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using MediatR;
using System.Text;

namespace Aplicacao.Notificacoes.OS;

public class OrdemServicoFinalizadaHandler(IRepositorio<OrdemServico> ordemServicoRepositorio, ILogServico<OrdemServicoFinalizadaHandler> logServico, IServicoEmail emailServico) : INotificationHandler<OrdemServicoFinalizadaEvent>
{
    private readonly IRepositorio<OrdemServico> _ordemServicoRepositorio = ordemServicoRepositorio;
    private readonly ILogServico<OrdemServicoFinalizadaHandler> _logServico = logServico;
    private readonly IServicoEmail _emailServico = emailServico;

    public async Task Handle(OrdemServicoFinalizadaEvent notification, CancellationToken cancellationToken)
    {
        var metodo = nameof(Handle);

        try
        {
            _logServico.LogInicio(metodo, notification.OrdemServicoId);

            var especificacao = new ObterOrdemServicoPorIdComIncludeEspecificacao(notification.OrdemServicoId);
            var os = await _ordemServicoRepositorio.ObterUmSemRastreamentoAsync(especificacao);

            if (os is null) return;

            string conteudo = await GerarConteudoEmailAsync(os);

            await _emailServico.EnviarAsync(
                [os.Cliente.Contato.Email],
                "Serviço Finalizado",
                conteudo);
            
            _logServico.LogFim(metodo);
        }
        catch (Exception e)
        {
            _logServico.LogErro(metodo, e);

            throw;
        }
    }

    private static async Task<string> GerarConteudoEmailAsync(OrdemServico os)
    {
        const string templateFileName = "EmailOrdemServicoFinalizada.html";

        string templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", templateFileName);
        string template = await File.ReadAllTextAsync(templatePath, Encoding.UTF8);

        template = template
            .Replace("{{NOME_CLIENTE}}", os.Cliente.Nome)
            .Replace("{{NOME_SERVICO}}", os.Servico.Nome)
            .Replace("{{MODELO_VEICULO}}", os.Veiculo.Modelo)
            .Replace("{{PLACA_VEICULO}}", os.Veiculo.Placa);

        return template;
    }
}