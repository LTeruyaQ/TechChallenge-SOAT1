using API.Filters;
using API.Models;
using Aplicacao.DTOs.Requests.Cliente;
using Aplicacao.DTOs.Responses.Cliente;
using Aplicacao.Interfaces.Servicos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class ClienteController : BaseApiController
{
    private readonly IClienteServico _clienteService;

    public ClienteController(IClienteServico service)
    {
        _clienteService = service;
    }

    [HttpGet]
    [PermissaoNecessaria("cliente", "administrador")]
    [ProducesResponseType(typeof(IEnumerable<ClienteResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterTodos()
    {
        return Ok(await _clienteService.ObterTodosAsync());
    }
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(IEnumerable<ClienteResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        return Ok(await _clienteService.ObterPorIdAsync(id));
    }

    [HttpPost]
    [PermissaoNecessaria("administrador")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Criar([FromBody] CadastrarClienteRequest request)
    {
        var cliente = await _clienteService.CadastrarAsync(request);

        return CreatedAtAction(nameof(ObterPorId), new { id = cliente.Id }, cliente);
    }

    [HttpPut("{id:guid}")]
    [PermissaoNecessaria("administrador")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarClienteRequest request)
    {
        var resultadoValidacao = ValidarModelState();
        if (resultadoValidacao != null) return resultadoValidacao;

        return Ok(await _clienteService.AtualizarAsync(id, request));
    }

    [HttpDelete("{id:guid}")]
    [PermissaoNecessaria("administrador")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Remover(Guid id)
    {
        await _clienteService.RemoverAsync(id);
        return NoContent();
    }
}
