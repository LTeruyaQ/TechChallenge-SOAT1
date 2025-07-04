using Aplicacao.DTOs.Requests.Servico;
using Aplicacao.DTOs.Responses.Servico;
using Aplicacao.Interfaces.Servicos;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("[controller]")]
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
    [ProducesResponseType(typeof(IEnumerable<ServicoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterTodos()
    {
        var servicos = await _servico.ObterTodosAsync();
        return Ok(servicos);
    }

    [HttpGet("disponiveis")]
    [ProducesResponseType(typeof(IEnumerable<ServicoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterServicosDisponiveis()
    {
        var servicos = await _servico.ObterServicosDisponiveisAsync();
        return Ok(servicos);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ServicoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var servico = await _servico.ObterServicoPorIdAsync(id) 
            ?? throw new KeyNotFoundException("Serviço não encontrado");
        return Ok(servico);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Criar([FromBody] CadastrarServicoRequest request)
    {
        var servico = await _servico.CadastrarServicoAsync(request);
        return CreatedAtAction(nameof(ObterPorId), new { id = servico.Id }, servico);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ServicoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] EditarServicoRequest request)
    {
        var servicoAtualizado = await _servico.EditarServicoAsync(id, request);
        return Ok(servicoAtualizado);
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
