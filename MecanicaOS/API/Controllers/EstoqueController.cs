using Dominio.DTOs.Estoque;
using Dominio.Entidades;
using Dominio.Interfaces.Servicos;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EstoqueController : ControllerBase
{
    private readonly IEstoqueServico _estoqueService;

    public EstoqueController(IEstoqueServico estoqueService)
    {
        _estoqueService = estoqueService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var estoque = await _estoqueService.ObterPorIdAsync(id);
        return Ok(estoque);
    }

    [HttpPost]
    public async Task<IActionResult> CadastrarEstoque([FromBody] EstoqueRegistrarDto dto)
    {
        var estoque = await _estoqueService.CadastrarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = estoque.Id }, new { id = estoque.Id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> AtualizarEstoque(Guid id, [FromBody] EstoqueAtualizarDto dto)
    {
        await _estoqueService.AtualizarAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletarEstoque(Guid id)
    {
        await _estoqueService.RemoverAsync(id);
        return NoContent();
    }
}