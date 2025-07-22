using API.Filters;
using API.Models;
using Aplicacao.DTOs.Requests.OrdemServico;
using Aplicacao.DTOs.Requests.OrdemServico.InsumoOS;
using Aplicacao.DTOs.Responses.OrdemServico;
using Aplicacao.DTOs.Responses.OrdemServico.InsumoOrdemServico;
using Aplicacao.Interfaces.Servicos;
using Dominio.Enumeradores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class OrdemServicoController : BaseApiController
{
    private readonly IOrdemServicoServico _ordemServico;
    private readonly IInsumoOSServico _insumoOSServico;

    public OrdemServicoController(IOrdemServicoServico ordemServico, IInsumoOSServico insumoOSServico)
    {
        _ordemServico = ordemServico;
        _insumoOSServico = insumoOSServico;
    }

    [HttpGet]
    [PermissaoNecessaria("admin")]
    [ProducesResponseType(typeof(IEnumerable<OrdemServicoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterTodos()
    {
        var ordemServicos = await _ordemServico.ObterTodosAsync();
        return Ok(ordemServicos);
    }

    [HttpGet("{id:guid}")]
    [PermissaoNecessaria("cliente", "admin")]
    [ProducesResponseType(typeof(OrdemServicoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var ordemServico = await _ordemServico.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Ordem de Serviço não encontrado");
        return Ok(ordemServico);
    }

    [HttpGet("{status}")]
    [PermissaoNecessaria("admin")]
    [ProducesResponseType(typeof(IEnumerable<OrdemServicoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterPorStatus(StatusOrdemServico status)
    {
        var ordemServico = await _ordemServico.ObterPorStatusAsync(status)
            ?? throw new KeyNotFoundException("Ordem de Serviço não encontrado");
        return Ok(ordemServico);
    }

    [HttpPost]
    [PermissaoNecessaria("cliente", "admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Criar([FromBody] CadastrarOrdemServicoRequest request)
    {
        var ordemServico = await _ordemServico.CadastrarAsync(request);
        return CreatedAtAction(nameof(ObterPorId), new { id = ordemServico.Id }, ordemServico);
    }

    [HttpPut("{id:guid}")]
    [PermissaoNecessaria("admin")]
    [ProducesResponseType(typeof(OrdemServicoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarOrdemServicoRequest request)
    {
        var ordemServicoAtualizado = await _ordemServico.AtualizarAsync(id, request);
        return Ok(ordemServicoAtualizado);
    }

    [HttpPost("{ordemServicoId}/insumos")]
    [PermissaoNecessaria("admin")]
    [ProducesResponseType(typeof(InsumoOSResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AdicionarInsumosOS(Guid ordemServicoId, List<CadastrarInsumoOSRequest> request)
    {
        var insumosOS = await _insumoOSServico.CadastrarInsumosAsync(ordemServicoId, request);
        return Ok(insumosOS);
    }

    [HttpPatch("{id}/aceitar-orcamento")]
    [PermissaoNecessaria("cliente", "admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AceitarOrcamento(Guid id)
    {
        await _ordemServico.AceitarOrcamentoAsync(id);
        return NoContent();
    }

    [HttpPatch("{id}/recusar-orcamento")]
    [PermissaoNecessaria("cliente", "admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RecusarOrcamento(Guid id)
    {
        await _ordemServico.RecusarOrcamentoAsync(id);
        return NoContent();
    }
}