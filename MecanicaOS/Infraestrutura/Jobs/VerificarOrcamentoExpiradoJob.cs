using Core.DTOs.Entidades.Estoque;
using Core.DTOs.Entidades.OrdemServicos;
using Core.Entidades;
using Core.Enumeradores;
using Core.Especificacoes.OrdemServico;
using Core.Interfaces.Handlers.InsumosOS;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;

namespace Infraestrutura.Jobs;

public class VerificarOrcamentoExpiradoJob(IRepositorio<OrdemServicoEntityDto> ordemServicoRepository, 
    IDevolverInsumosHandler devolverInsumosHandler, 
    IUnidadeDeTrabalho udt, 
    ILogServico<VerificarOrcamentoExpiradoJob> logServico)
{
    private readonly IRepositorio<OrdemServicoEntityDto> _ordemServicoRepository = ordemServicoRepository;
    private readonly IDevolverInsumosHandler _devolverInsumosHandler = devolverInsumosHandler;
    private readonly IUnidadeDeTrabalho _uot = udt;
    private readonly ILogServico<VerificarOrcamentoExpiradoJob> _logServico = logServico;

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
            };

            //TODO: Refatorar para usar Controller, isso nao é responsabilidade do Job
            await _devolverInsumosHandler.Handle(ordensServico
                .SelectMany(os => os.InsumosOS)
                .Select(i => new InsumoOS()
                    {
                        Id = i.Id,
                        OrdemServicoId = i.OrdemServicoId,
                        EstoqueId = i.EstoqueId,
                        Quantidade = i.Quantidade,
                        Ativo = i.Ativo,
                        DataCadastro = i.DataCadastro,
                        DataAtualizacao = i.DataAtualizacao,
                        Estoque = new Estoque()
                        {
                            Id = i.Estoque.Id,
                            Insumo = i.Estoque.Insumo,
                            QuantidadeDisponivel = i.Estoque.QuantidadeDisponivel,
                            QuantidadeMinima = i.Estoque.QuantidadeMinima,
                            Descricao = i.Estoque.Descricao,
                            Preco = i.Estoque.Preco,
                            Ativo = i.Estoque.Ativo,
                            DataCadastro = i.Estoque.DataCadastro,
                            DataAtualizacao = i.Estoque.DataAtualizacao
                        }
                    })
                );

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