using Dominio.Entidades.Abstratos;
using Dominio.Enumeradores;

namespace Dominio.Entidades;

public class Orcamento : Entidade
{
    public int DIAS_PARA_EXPIRACAO => 3;

    public Guid OrdemServicoId { get; set; }
    public OrdemServico OrdemServico { get; set; } = null!;
    public decimal Valor { get; set; }
    public DateTime? DataEnvio { get; set; }
    public StatusOrcamentoEnum Status { get; set; }
    
    public Orcamento() { }

    public Orcamento(Guid ordemServicoId, decimal valor)
    {
        if (valor <= 0) 
            throw new ArgumentException("O valor do orçamento deve ser maior que zero.", nameof(valor));
        
        OrdemServicoId = ordemServicoId;
        Valor = valor;
    }

    public void Atualizar(decimal? valor)
    {
        if (valor is null) 
            throw new ArgumentNullException(nameof(valor), "O valor do orçamento não pode ser nulo.");

        Valor = valor.Value;
        Status = StatusOrcamentoEnum.AguardandoAprovacao;

        DataAtualizacao = DateTime.UtcNow;
    }

    public void PrepararParaEnvio()
    {
        DataEnvio = DateTime.UtcNow;
        Status = StatusOrcamentoEnum.AguardandoAprovacao;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void ExpirarOrcamento()
    {
        if (Status != StatusOrcamentoEnum.AguardandoAprovacao)
            throw new InvalidOperationException("Orçamento não está aguardando aprovação.");
        
        Status = StatusOrcamentoEnum.Expirado;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void AprovarOrcamento()
    {
        if (Status != StatusOrcamentoEnum.AguardandoAprovacao)
            throw new InvalidOperationException("Orçamento não está aguardando aprovação.");
        
        Status = StatusOrcamentoEnum.Aprovado;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void RejeitarOrcamento()
    {
        if (Status != StatusOrcamentoEnum.AguardandoAprovacao)
            throw new InvalidOperationException("Orçamento não está aguardando aprovação.");
        
        Status = StatusOrcamentoEnum.Rejeitado;
        DataAtualizacao = DateTime.UtcNow;
    }

    public bool DeveExpirar()
    {
        return DataEnvio.HasValue
            && DataEnvio.Value.AddDays(DIAS_PARA_EXPIRACAO) <= DateTime.UtcNow;
    }
}
