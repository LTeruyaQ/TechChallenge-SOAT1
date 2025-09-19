using Core.DTOs.Entidades.OrdemServicos;
using Core.Enumeradores;
using Core.Especificacoes.OrdemServico;
using Core.Interfaces.Controllers;
using Core.Interfaces.Repositorios;
using Core.Interfaces.root;
using Core.Interfaces.Servicos;

namespace API.Jobs;

/// <summary>
/// TODO: Migrar pra usar controller
/// </summary>
public class VerificarOrcamentoExpiradoJob
{
    private readonly ICompositionRoot _compositionRoot;
    private readonly IRepositorio<OrdemServicoEntityDto> _ordemServicoRepository;
    private readonly IInsumoOSController _insumoOSController;
    private readonly IUnidadeDeTrabalho _uot;
    private readonly ILogServico<VerificarOrcamentoExpiradoJob> _logServico;

    public VerificarOrcamentoExpiradoJob(ICompositionRoot compositionRoot)
    {
        _compositionRoot = compositionRoot;
        _ordemServicoRepository = _compositionRoot.CriarRepositorio<OrdemServicoEntityDto>();
        _insumoOSController = _compositionRoot.CriarInsumoOSController();
        _uot = _compositionRoot.CriarUnidadeDeTrabalho();
        _logServico = _compositionRoot.CriarLogService<VerificarOrcamentoExpiradoJob>();
    }

    public async Task ExecutarAsync()
    {
        var metodo = nameof(ExecutarAsync);

        try
        {
            _logServico.LogInicio(metodo);

            var especificacao = new ObterOSOrcamentoExpiradoEspecificacao();
            var ordensServico = await _ordemServicoRepository.ListarAsync(especificacao);

            if (!ordensServico.Any())
                return;

            foreach (var os in ordensServico)
            {
                os.Status = StatusOrdemServico.OrcamentoExpirado;
                os.DataAtualizacao = DateTime.UtcNow;
            }
            ;

            //TODO: Refatorar para usar Controller, isso nao é responsabilidade do Job
            //await _devolverInsumosHandler.Handle(ordensServico
            //    .SelectMany(os => os.InsumosOS)
            //    .Select(i => new InsumoOS()
            //    {
            //        Id = i.Id,
            //        OrdemServicoId = i.OrdemServicoId,
            //        EstoqueId = i.EstoqueId,
            //        Quantidade = i.Quantidade,
            //        Ativo = i.Ativo,
            //        DataCadastro = i.DataCadastro,
            //        DataAtualizacao = i.DataAtualizacao,
            //        Estoque = new Estoque()
            //        {
            //            Id = i.Estoque.Id,
            //            Insumo = i.Estoque.Insumo,
            //            QuantidadeDisponivel = i.Estoque.QuantidadeDisponivel,
            //            QuantidadeMinima = i.Estoque.QuantidadeMinima,
            //            Descricao = i.Estoque.Descricao,
            //            Preco = i.Estoque.Preco,
            //            Ativo = i.Estoque.Ativo,
            //            DataCadastro = i.Estoque.DataCadastro,
            //            DataAtualizacao = i.Estoque.DataAtualizacao
            //        }
            //    })
            //    );

            await _ordemServicoRepository.EditarVariosAsync(ordensServico);

            await _uot.Commit();

            _logServico.LogFim(metodo);
        }
        catch (Exception e)
        {
            _logServico.LogErro(metodo, e);

            throw;
        }
    }
}