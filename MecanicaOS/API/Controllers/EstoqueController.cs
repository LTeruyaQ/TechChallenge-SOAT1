using API.Models;
using Dominio.DTOs.Estoque;
using Dominio.Entidades;
using Dominio.Exceptions;
using Dominio.Interfaces.Servicos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

[Route("api/[controller]")]
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

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Estoque), StatusCodes.Status200OK)]
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
    public async Task<IActionResult> CadastrarEstoque([FromBody] EstoqueRegistrarDto dto)
    {
        var estoque = await _estoqueService.CadastrarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = estoque.Id }, new { id = estoque.Id });
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AtualizarEstoque(Guid id, [FromBody] EstoqueAtualizarDto dto)
    {
        await _estoqueService.AtualizarAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeletarEstoque(Guid id)
    {
        await _estoqueService.RemoverAsync(id);
        return NoContent();
    }
}