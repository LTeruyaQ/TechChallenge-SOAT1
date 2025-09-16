using Core.DTOs.Entidades.OrdemServicos;
using Core.Especificacoes.OrdemServico;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using MediatR;
using System.Text;

namespace Infraestrutura.Notificacoes.OS;

public class OrdemServicoFinalizadaHandler(IRepositorio<OrdemServicoEntityDto> ordemServicoRepositorio, ILogServico<OrdemServicoFinalizadaHandler> logServico, IServicoEmail emailServico) : INotificationHandler<OrdemServicoFinalizadaEvent>
{
    private readonly IRepositorio<OrdemServicoEntityDto> _ordemServicoRepositorio = ordemServicoRepositorio;
    private readonly ILogServico<OrdemServicoFinalizadaHandler> _logServico = logServico;
    private readonly IServicoEmail _emailServico = emailServico;

    public async Task Handle(OrdemServicoFinalizadaEvent notification, CancellationToken cancellationToken)
    {
        var metodo = nameof(Handle);

        try
        {
            _logServico.LogInicio(metodo, notification.OrdemServicoId);

            var especificacao = new ObterOrdemServicoPorIdComIncludeEspecificacao(notification.OrdemServicoId);
            var osDto = await _ordemServicoRepositorio.ObterUmSemRastreamentoAsync(especificacao);

            if (osDto is null) return;

            string conteudo = await GerarConteudoEmailAsync(osDto);

            await _emailServico.EnviarAsync(
                [osDto.Cliente.Contato.Email],
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

    private static async Task<string> GerarConteudoEmailAsync(OrdemServicoEntityDto os)
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