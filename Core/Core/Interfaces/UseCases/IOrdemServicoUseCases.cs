﻿using Core.DTOs.OrdemServico;
using Core.Entidades;
using Core.Enumeradores;

namespace Core.Interfaces.UseCases
{
    public interface IOrdemServicoUseCases
    {
        Task AceitarOrcamentoUseCaseAsync(Guid id);
        Task<OrdemServico> AtualizarUseCaseAsync(Guid id, AtualizarOrdemServicoUseCaseDto request);
        Task<OrdemServico> CadastrarUseCaseAsync(CadastrarOrdemServicoUseCaseDto request);
        Task<OrdemServico?> ObterPorIdUseCaseAsync(Guid id);
        Task<IEnumerable<OrdemServico>> ObterPorStatusUseCaseAsync(StatusOrdemServico status);
        Task<IEnumerable<OrdemServico>> ObterTodosUseCaseAsync();
        Task RecusarOrcamentoUseCaseAsync(Guid id);
    }
}