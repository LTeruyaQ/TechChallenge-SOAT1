using API.Models;
using Aplicacao.DTOs.Requests.Servico;
using Aplicacao.DTOs.Requests.Veiculo;
using Aplicacao.DTOs.Responses.Estoque;
using Aplicacao.DTOs.Responses.Servico;
using Aplicacao.Interfaces.Servicos;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ClienteController : Controller
{
    private readonly IClienteServico service;

    public ClienteController(IClienteServico service)
    {
        this.service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClienteResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {   
        return Ok(await service.ObterTodosAsync());
    }


    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Post([FromBody] CadastrarClienteRequest request)
    {   
        return Ok(await service.CadastrarAsync(request));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Put(Guid id, [FromBody] AtualizarClienteRequest request)
    {   
        return Ok(await service.AtualizarAsync(id, request));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await service.DeletarAsync(id);
        return NoContent();
    }
}
