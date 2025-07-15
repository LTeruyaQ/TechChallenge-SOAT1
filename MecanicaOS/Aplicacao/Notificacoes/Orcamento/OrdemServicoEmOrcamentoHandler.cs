using Aplicacao.Interfaces.Servicos;
using Dominio.Entidades;
using Dominio.Enumeradores;
using Dominio.Interfaces.Repositorios;
using Dominio.Interfaces.Servicos;
using MediatR;

namespace Aplicacao.Notificacoes.Orcamento;

public class OrdemServicoEmOrcamentoHandler : INotificationHandler<OrdemServicoEmOrcamentoEvent>
{
    private readonly IRepositorio<OrdemServico> _ordemServicoRepositorio;
    private readonly IOrcamentoServico _orcamentoServico;
    private readonly IServicoEmail _emailServico;

    public OrdemServicoEmOrcamentoHandler(
        IRepositorio<OrdemServico> ordemServicoRepositorio,
        IOrcamentoServico orcamentoServico,
        IServicoEmail emailServico)
    {
        _ordemServicoRepositorio = ordemServicoRepositorio;
        _orcamentoServico = orcamentoServico;
        _emailServico = emailServico;
    }

    public async Task Handle(OrdemServicoEmOrcamentoEvent notification, CancellationToken cancellationToken)
    {
        var os = await _ordemServicoRepositorio.ObterPorIdAsync(notification.OrdemServicoId);
        if (os is null) return;

        os.Orcamento = _orcamentoServico.GerarOrcamento(os);
        os.Status = StatusOrdemServico.AguardandoAprovação;

        await EnviarOrcamentoAsync(os);

        await _ordemServicoRepositorio.EditarAsync(os);
    }

    private async Task EnviarOrcamentoAsync(OrdemServico os)
    {
        string conteudo = GerarConteudoEmailOrcamento();

        await _emailServico.EnviarAsync(
            [os.Cliente.Contato.Email],
            "Orçamento de Serviço",
            conteudo);
    }

    private string GerarConteudoEmailOrcamento()
    {
        return string.Empty;
    }
}