using Core.DTOs.Entidades.OrdemServicos;
using Core.Entidades;
using Core.Enumeradores;
using Core.Especificacoes.OrdemServico;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;
using MediatR;
using System.Text;
using System.Text.RegularExpressions;

namespace Infraestrutura.Notificacoes.OS;

public class OrdemServicoEmOrcamentoHandler(
    IRepositorio<OrdemServicoEntityDto> ordemServicoRepositorio,
    IOrcamentoUseCases orcamentoUseCases,
    IServicoEmail emailServico,
    ILogServico<OrdemServicoEmOrcamentoHandler> logServico,
    IUnidadeDeTrabalho udt) : INotificationHandler<OrdemServicoEmOrcamentoEvent>
{
    private readonly IRepositorio<OrdemServicoEntityDto> _ordemServicoRepositorio = ordemServicoRepositorio;
    private readonly IOrcamentoUseCases _orcamentoUseCases = orcamentoUseCases;
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
            var osDto = await _ordemServicoRepositorio.ObterUmAsync(especificacao);

            if (osDto is null) return;

            // Converter DTO para entidade para usar no UseCase
            var os = ConvertToEntity(osDto);

            var orcamento = _orcamentoUseCases.GerarOrcamentoUseCase(os);
            os.Orcamento = orcamento;
            os.Status = StatusOrdemServico.AguardandoAprovação;
            os.DataEnvioOrcamento = DateTime.UtcNow;

            await EnviarOrcamentoAsync(os);

            // Atualizar DTO com os novos valores
            osDto.Orcamento = os.Orcamento;
            osDto.Status = os.Status;
            osDto.DataEnvioOrcamento = os.DataEnvioOrcamento;

            await _ordemServicoRepositorio.EditarAsync(osDto);
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

    private static OrdemServico ConvertToEntity(OrdemServicoEntityDto dto)
    {
        return new OrdemServico
        {
            Id = dto.Id,
            Ativo = dto.Ativo,
            DataCadastro = dto.DataCadastro,
            DataAtualizacao = dto.DataAtualizacao,
            ClienteId = dto.ClienteId,
            VeiculoId = dto.VeiculoId,
            ServicoId = dto.ServicoId,
            Orcamento = dto.Orcamento,
            DataEnvioOrcamento = dto.DataEnvioOrcamento,
            Descricao = dto.Descricao,
            Status = dto.Status,
            InsumosOS = dto.InsumosOS.Select(insumoDto => new InsumoOS
            {
                Id = insumoDto.Id,
                Ativo = insumoDto.Ativo,
                DataCadastro = insumoDto.DataCadastro,
                DataAtualizacao = insumoDto.DataAtualizacao,
                OrdemServicoId = insumoDto.OrdemServicoId,
                EstoqueId = insumoDto.EstoqueId,
                Quantidade = insumoDto.Quantidade,
                Estoque = new Estoque
                {
                    Id = insumoDto.Estoque.Id,
                    Ativo = insumoDto.Estoque.Ativo,
                    DataCadastro = insumoDto.Estoque.DataCadastro,
                    DataAtualizacao = insumoDto.Estoque.DataAtualizacao,
                    QuantidadeMinima = insumoDto.Estoque.QuantidadeMinima,
                    QuantidadeDisponivel = insumoDto.Estoque.QuantidadeDisponivel,
                    Descricao = insumoDto.Estoque.Descricao,
                    Insumo = insumoDto.Estoque.Insumo,
                    Preco = insumoDto.Estoque.Preco
                }
            }).ToList(),
            Cliente = new Cliente
            {
                Id = dto.Cliente.Id,
                Nome = dto.Cliente.Nome,
                Contato = new Contato
                {
                    Email = dto.Cliente.Contato.Email
                }
            },
            Servico = new Servico
            {
                Id = dto.Servico.Id,
                Nome = dto.Servico.Nome,
                Descricao = dto.Servico.Descricao,
                Valor = dto.Servico.Valor,
                Disponivel = dto.Servico.Disponivel
            },
            Veiculo = new Veiculo
            {
                Id = dto.Veiculo.Id,
                Modelo = dto.Veiculo.Modelo,
                Placa = dto.Veiculo.Placa
            }
        };
    }
}