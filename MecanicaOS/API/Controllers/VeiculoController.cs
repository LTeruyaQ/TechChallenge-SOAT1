using Dominio.DTOs.Veiculo;
using Dominio.Interfaces.Servicos;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/veiculos")]
[ApiController]
public class VeiculosController : ControllerBase
{
    private readonly IVeiculoServico _veiculoService;

    public VeiculosController(IVeiculoServico veiculoService)
    {
        _veiculoService = veiculoService;
    }

    [HttpPost]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarVeiculoDto dto)
    {
        var veiculo = await _veiculoService.CadastrarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = veiculo.Id }, new { id = veiculo.Id });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deletar(Guid id)
    {
        await _veiculoService.RemoverAsync(id);
        return NoContent();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Editar(Guid id, [FromBody] EditarVeiculoDto dto)
    {
        await _veiculoService.AtualizarAsync(id, dto);
        return NoContent();
    }

    [HttpGet("cliente/{clienteId:guid}")]
    public async Task<IActionResult> ObterPorCliente(Guid clienteId)
    {
        var veiculos = await _veiculoService.ObterPorClienteAsync(clienteId);
        return Ok(veiculos);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var veiculo = await _veiculoService.ObterPorIdAsync(id);
        return Ok(veiculo);
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodos()
    {
        var veiculos = await _veiculoService.ObterTodosAsync();
        return Ok(veiculos);
    }
}