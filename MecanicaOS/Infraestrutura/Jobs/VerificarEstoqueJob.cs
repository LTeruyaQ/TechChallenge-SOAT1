using Core.DTOs.Entidades.Estoque;
using Core.DTOs.Entidades.Usuarios;
using Core.Especificacoes.Estoque;
using Core.Especificacoes.Usuario;
using Core.Exceptions;
using Core.Interfaces.Jobs;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Hangfire;
using System.Text;

namespace Infraestrutura.Jobs;

public class VerificarEstoqueJob(
    IRepositorio<AlertaEstoqueEntityDto> alertaEstoqueRepositorio,
    IServicoEmail notificacaoEmail,
    ILogServico<VerificarEstoqueJob> logServico,
    IUnidadeDeTrabalho udt,
    IRepositorio<EstoqueEntityDto> estoqueRepositorio,
    IRepositorio<UsuarioEntityDto> usuarioRepositorio) : IVerificarEstoqueJob
{
    private readonly IRepositorio<EstoqueEntityDto> _estoqueRepositorio = estoqueRepositorio;
    private readonly IRepositorio<UsuarioEntityDto> _usuarioRepositorio = usuarioRepositorio;
    private readonly IRepositorio<AlertaEstoqueEntityDto> _alertaEstoqueRepositorio = alertaEstoqueRepositorio;
    private readonly ILogServico<VerificarEstoqueJob> _logServico = logServico;
    private readonly IServicoEmail _servicoEmail = notificacaoEmail;
    private readonly IUnidadeDeTrabalho _uot = udt;

    [DisableConcurrentExecution(timeoutInSeconds: 3600)]
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

    private async Task<List<EstoqueEntityDto>> ObterInsumosParaAlertaAsync()
    {
        IEnumerable<EstoqueEntityDto> insumosCriticos = await ObterInsumosAbaixoDaQuantidadeMinima();

        var dataAtual = DateTime.UtcNow;
        var insumosParaAlerta = new List<EstoqueEntityDto>();

        foreach (var insumo in insumosCriticos)
        {
            var alertasEnviadosHoje = await _alertaEstoqueRepositorio.ListarAsync(
                new ObterAlertaDoDiaPorEstoqueEspecificacao(
                    insumo.Id,
                    dataAtual));

            if (!alertasEnviadosHoje.Any())
            {
                insumosParaAlerta.Add(insumo);
            }
        }

        return insumosParaAlerta;
    }

    private async Task<IEnumerable<EstoqueEntityDto>> ObterInsumosAbaixoDaQuantidadeMinima()
    {
        var metodo = nameof(ObterInsumosAbaixoDaQuantidadeMinima);
        _logServico.LogInicio(metodo);
        try
        {
            var especificacao = new ObterEstoqueCriticoEspecificacao();
            var insumosCriticos = await _estoqueRepositorio.ListarAsync(especificacao);

            _logServico.LogFim(metodo, insumosCriticos);
            return insumosCriticos;
        }
        catch (Exception e)
        {
            _logServico.LogErro(metodo, e);
            throw;
        }
    }

    private async Task EnviarAlertaEstoqueAsync(IEnumerable<EstoqueEntityDto> insumosCriticos)
    {
        var metodo = nameof(EnviarAlertaEstoqueAsync);

        _logServico.LogInicio(metodo, insumosCriticos);
        try
        {
            var especificacao = new ObterUsuarioParaAlertaEstoqueEspecificacao();
            var usuariosAlerta = await _usuarioRepositorio.ListarAsync(especificacao);
            
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

    private async Task<string?> GerarConteudoEmailAsync(IEnumerable<EstoqueEntityDto> insumosCriticos)
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

    private async Task SalvarAlertaEnviadoAsync(IEnumerable<EstoqueEntityDto> insumosCriticos)
    {
        var metodo = nameof(SalvarAlertaEnviadoAsync);
        _logServico.LogInicio(metodo, insumosCriticos);
        try
        {
            var alertas = insumosCriticos
                .Select(insumo => new AlertaEstoqueEntityDto
                {
                    EstoqueId = insumo.Id
                });

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