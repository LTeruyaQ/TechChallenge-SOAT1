using API.Models;
using Dominio.DTOs.Servico;
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
public class ServicosController : ControllerBase
{
    private readonly IServicoServico _servico;

    public ServicosController(IServicoServico servico)
    {
        _servico = servico;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Servico>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterTodos()
    {
        var servicos = await _servico.ObterTodosAsync();
        return Ok(servicos);
    }
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Servico), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var servico = await _servico.ObterServicoPorIdAsync(id);
        return Ok(servico);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Criar([FromBody] CadastrarServicoDto novoServico)
    {
        var servico = await _servico.CadastrarServicoAsync(novoServico);
        return CreatedAtAction(nameof(ObterPorId), new { id = servico.Id }, new { id = servico.Id });
    }
    
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Editar(Guid id, [FromBody] EditarServicoDto servicoAtualizado)
    {
        await _servico.EditarServicoAsync(id, servicoAtualizado);
        return NoContent();
    }
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Deletar(Guid id)
    {
        await _servico.DeletarServicoAsync(id);
        return NoContent();
    }
}
