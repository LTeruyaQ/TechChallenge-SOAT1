using Aplicacao.DTOs.Requests.Veiculo;
using Aplicacao.DTOs.Responses.Veiculo;
using Aplicacao.Interfaces.Servicos;
using Dominio.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("[controller]")]
[ApiController]
[Produces("application/json")]
[Consumes("application/json")]
public class VeiculoController : ControllerBase
{
    private readonly IVeiculoServico _veiculoServico;
    private readonly ILogger<VeiculoController> _logger;

    public VeiculoController(IVeiculoServico veiculoServico, ILogger<VeiculoController> logger)
    {
        _veiculoServico = veiculoServico;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarVeiculoRequest request)
    {
        var response = await _veiculoServico.CadastrarAsync(request);
        return CreatedAtAction(nameof(ObterPorId), new { id = response.Id }, response);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Deletar(Guid id)
    {
        var sucesso = await _veiculoServico.DeletarAsync(id);
        if (!sucesso)
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro ao remover o veículo." });

        return NoContent();
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Editar(Guid id, [FromBody] AtualizarVeiculoRequest request)
    {
        var response = await _veiculoServico.AtualizarAsync(id, request);
        return Ok(response);
    }

    [HttpGet("cliente/{clienteId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<VeiculoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterPorCliente(Guid clienteId)
    {
        var response = await _veiculoServico.ObterPorClienteAsync(clienteId);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(VeiculoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var response = await _veiculoServico.ObterPorIdAsync(id);
        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VeiculoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterTodos()
    {
        var response = await _veiculoServico.ObterTodosAsync();
        return Ok(response);
    }
}