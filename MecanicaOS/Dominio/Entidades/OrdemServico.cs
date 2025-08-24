using Dominio.Entidades.Abstratos;
using Dominio.Enumeradores;
using Dominio.Exceptions;

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
        if (clienteId != null) ClienteId = clienteId.Value;
        if (veiculoId != null) VeiculoId = veiculoId.Value;
        if (servicoId != null) ServicoId = servicoId.Value;
        if (!string.IsNullOrEmpty(descricao)) Descricao = descricao;
        if (status != null) Status = status.Value;
    
        DataAtualizacao = DateTime.UtcNow;
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
        Orcamento?.AprovarOrcamento();
    }

    public void Cancelar()
    {
        DataAtualizacao = DateTime.UtcNow;
        Status = StatusOrdemServico.Cancelada;
        Orcamento?.RejeitarOrcamento();
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
        Orcamento?.ExpirarOrcamento();
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