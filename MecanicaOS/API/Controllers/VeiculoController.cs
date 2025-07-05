using API.Models;
using Dominio.DTOs.Veiculo;
using Dominio.Entidades;
using Dominio.Interfaces.Servicos;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
[Consumes("application/json")]
public class VeiculoController : ControllerBase
{
    private readonly IVeiculoServico _veiculoService;

    public VeiculoController(IVeiculoServico veiculoService)
    {
        _veiculoService = veiculoService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarVeiculoDto dto)
    {
        var veiculo = await _veiculoService.CadastrarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = veiculo.Id }, new { id = veiculo.Id });
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Deletar(Guid id)
    {
        await _veiculoService.RemoverAsync(id);
        return NoContent();
    }
    
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Editar(Guid id, [FromBody] EditarVeiculoDto dto)
    {
        await _veiculoService.AtualizarAsync(id, dto);
        return NoContent();
    }

    [HttpGet("cliente/{clienteId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<Veiculo>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterPorCliente(Guid clienteId)
    {
        var veiculos = await _veiculoService.ObterPorClienteAsync(clienteId);
        return Ok(veiculos);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Veiculo), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var veiculo = await _veiculoService.ObterPorIdAsync(id);
        return Ok(veiculo);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Veiculo>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterTodos()
    {
        var veiculos = await _veiculoService.ObterTodosAsync();
        return Ok(veiculos);
    }
}