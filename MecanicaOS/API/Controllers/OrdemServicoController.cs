using API.Models;
using Aplicacao.DTOs.Requests.OrdermServico;
using Aplicacao.DTOs.Responses.OrdemServico;
using Aplicacao.Interfaces.Servicos;
using Dominio.Enumeradores;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("[controller]")]
[ApiController]
[Produces("application/json")]
[Consumes("application/json")]
public class OrdemServicoController : ControllerBase
{
    private readonly IOrdemServicoServico _ordemServico;

    public OrdemServicoController(IOrdemServicoServico ordemServico)
    {
        _ordemServico = ordemServico;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrdemServicoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterTodos()
    {
        var ordemServicos = await _ordemServico.ObterTodosAsync();
        return Ok(ordemServicos);
    }

    [HttpGet("{id:guid}")]
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
    [ProducesResponseType(typeof(OrdemServicoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarOrdemServicoRequest request)
    {
        var ordemServicoAtualizado = await _ordemServico.AtualizarAsync(id, request);
        return Ok(ordemServicoAtualizado);
    }
}
