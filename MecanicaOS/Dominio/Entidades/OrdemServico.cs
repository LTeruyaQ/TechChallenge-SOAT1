using Aplicacao.Notificacoes.OS;
using Dominio.Entidades.Abstratos;
using Dominio.Enumeradores;
using Dominio.Exceptions;
using MediatR;

namespace Dominio.Entidades;

public class OrdemServico : Entidade
{
    public Guid ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public Guid VeiculoId { get; set; }
    public Veiculo Veiculo { get; set; } = null!;
    public Guid ServicoId { get; set; }
    public Servico Servico { get; set; } = null!;
    public Guid? OrcamentoId { get; set; }
    public Orcamento? Orcamento { get; set; } = null!;
    public string? Descricao { get; set; }
    public StatusOrdemServico Status { get; set; }
    public ICollection<InsumoOS> InsumosOS { get; set; } = [];

    public OrdemServico() { }

    public void Atualizar(Guid? clienteId, Guid? veiculoId, Guid? servicoId, string? descricao, StatusOrdemServico? status)
    {
        DataAtualizacao = DateTime.UtcNow;
        if (clienteId.HasValue) ClienteId = clienteId.Value;
        if (veiculoId.HasValue) VeiculoId = veiculoId.Value;
        if (servicoId.HasValue) ServicoId = servicoId.Value;
        if (!string.IsNullOrEmpty(descricao)) Descricao = descricao;
        if (status.HasValue)
            AlterarStatus(status.Value);
    }

    private void AlterarStatus(StatusOrdemServico status)
    {
        Status = status;
        AtribuirEventos();
    }

    private void AtribuirEventos()
    {
        if (ObterEventoStatus() is INotification evento && evento != null)
            AdicionarEvento(evento);
    }

    private INotification? ObterEventoStatus()
    {
        return Status switch
        {
            StatusOrdemServico.EmDiagnostico => new OrdemServicoEmOrcamentoEvent(Id),
            StatusOrdemServico.Cancelada => new OrdemServicoCanceladaEvent(Id),
            StatusOrdemServico.Finalizada => new OrdemServicoFinalizadaEvent(Id),
            _ => null
        };
    }

    public void ValidarStatusAguardando()
    {
        if (Status != StatusOrdemServico.AguardandoAprovação)
            throw new DadosInvalidosException("Orçamento não está aguardando aprovação.");
    }

    public void ValidarOrcamentoPresente()
    {
        if (Orcamento is null)
            throw new DadosInvalidosException("Orçamento não foi populado.");
    }

    public void AprovarOrcamento()
    {
        DataAtualizacao = DateTime.UtcNow;
        Status = StatusOrdemServico.EmExecucao;
        Orcamento?.Aprovar();
    }

    public void Cancelar()
    {
        DataAtualizacao = DateTime.UtcNow;
        AlterarStatus(StatusOrdemServico.Cancelada);
        Orcamento?.Rejeitar();
    }

    public bool DeveExpirar()
    {
        return Orcamento.DeveExpirar();
    }

    public bool OrcamentoExpirou()
    {
        return Status == StatusOrdemServico.OrcamentoExpirado;
    }

    public void Expirar()
    {
        Status = StatusOrdemServico.OrcamentoExpirado;
        DataAtualizacao = DateTime.UtcNow;
        Orcamento?.Expirar();
    }

    public void GerarOrcamento()
    {
        decimal precoServico = Servico!.Valor;
        decimal precoInsumos = InsumosOS.Sum(i =>
            i.Quantidade * i.Estoque.Preco);

        Orcamento = new Orcamento(Id, precoServico + precoInsumos);
        OrcamentoId = Orcamento.Id;
    }

    public void PrepararOrcamentoParaEnvio()
    {
        Status = StatusOrdemServico.AguardandoAprovação;
        Orcamento.PrepararParaEnvio();
    }
}