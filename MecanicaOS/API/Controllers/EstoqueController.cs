using API.Models;
using Aplicacao.DTOs.Requests.Estoque;
using Aplicacao.DTOs.Responses.Estoque;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("[controller]")]
[ApiController]
[Produces("application/json")]
[Consumes("application/json")]
public class EstoqueController : ControllerBase
{
    private readonly IEstoqueServico _estoqueService;

    public EstoqueController(IEstoqueServico estoqueService)
    {
        _estoqueService = estoqueService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EstoqueResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterTodos()
    {
        var estoques = await _estoqueService.ObterTodosAsync();
        return Ok(estoques);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EstoqueResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var estoque = await _estoqueService.ObterPorIdAsync(id);
        return Ok(estoque);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CadastrarEstoque([FromBody] CadastrarEstoqueRequest request)
    {
        var estoque = await _estoqueService.CadastrarAsync(request);
        return CreatedAtAction(nameof(ObterPorId), new { id = estoque.Id }, estoque);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(EstoqueResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AtualizarEstoque(Guid id, [FromBody] AtualizarEstoqueRequest request)
    {
        var estoqueAtualizado = await _estoqueService.AtualizarAsync(id, request);
        return Ok(estoqueAtualizado);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoverEstoque(Guid id)
    {
        var sucesso = await _estoqueService.DeletarAsync(id);
        if (!sucesso)
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Erro ao remover o item do estoque" });

        return NoContent();
    }
}