using Aplicacao.DTOs.Estoque;
using Dominio.Entidades;
using Dominio.Exceptions;
using Dominio.Interfaces.Services;
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

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return BadRequest("ID inválido.");

            Estoque estoque = await _estoqueService.ObterPorIdAsync(id);

            return Ok(estoque);
        }
        catch (EntidadeNaoEncontradaException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Ocorreu um erro inesperado.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CadastrarEstoque([FromBody] EstoqueRegistrarDto dto)
    {
        try
        {
            Estoque estoque = await _estoqueService.CadastrarAsync(dto);

            return CreatedAtAction(nameof(ObterPorId), new { id = estoque.Id }, estoque);
        }
        catch (Exception)
        {
            return StatusCode(500, "Erro interno ao cadastrar o estoque.");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarEstoque(Guid id, [FromBody] EstoqueAtualizarDto dto)
    {
        try
        {
            if (id == Guid.Empty)
                return BadRequest("ID inválido.");

            await _estoqueService.AtualizarAsync(id, dto);

            return NoContent();
        }
        catch (EntidadeNaoEncontradaException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Erro interno ao atualizar o estoque.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarEstoque(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return BadRequest("ID inválido.");

            await _estoqueService.RemoverAsync(id);

            return NoContent();
        }
        catch (EntidadeNaoEncontradaException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Erro interno ao deletar o estoque.");
        }
    }
}