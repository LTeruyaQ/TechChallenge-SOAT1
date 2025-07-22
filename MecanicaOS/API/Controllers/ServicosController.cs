using API.Filters;
using API.Models;
using Aplicacao.DTOs.Requests.Servico;
using Aplicacao.DTOs.Responses.Servico;
using Aplicacao.Interfaces.Servicos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class ServicosController : BaseApiController
{
    private readonly IServicoServico _servico;

    public ServicosController(IServicoServico servico)
    {
        _servico = servico;
    }

    [HttpGet]
    [PermissaoNecessaria("cliente", "admin")]
    [ProducesResponseType(typeof(IEnumerable<ServicoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterTodos()
    {
        var servicos = await _servico.ObterTodosAsync();
        return Ok(servicos);
    }

    [HttpGet("disponiveis")]
    [PermissaoNecessaria("cliente", "admin")]
    [ProducesResponseType(typeof(IEnumerable<ServicoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterServicosDisponiveis()
    {
        var servicos = await _servico.ObterServicosDisponiveisAsync();
        return Ok(servicos);
    }

    [HttpGet("{id:guid}")]
    [PermissaoNecessaria("cliente", "admin")]
    [ProducesResponseType(typeof(ServicoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var servico = await _servico.ObterServicoPorIdAsync(id);
        return Ok(servico);
    }

    [HttpPost]
    [PermissaoNecessaria("admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Criar([FromBody] CadastrarServicoRequest request)
    {
        var resultadoValidacao = ValidarModelState();
        if (resultadoValidacao != null) return resultadoValidacao;

        var servico = await _servico.CadastrarServicoAsync(request);
        return CreatedAtAction(nameof(ObterPorId), new { id = servico.Id }, servico);
    }

    [HttpPut("{id:guid}")]
    [PermissaoNecessaria("admin")]
    [ProducesResponseType(typeof(ServicoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] EditarServicoRequest request)
    {
        var resultadoValidacao = ValidarModelState();
        if (resultadoValidacao != null) return resultadoValidacao;

        var servicoAtualizado = await _servico.EditarServicoAsync(id, request);
        return Ok(servicoAtualizado);
    }

    [HttpDelete("{id:guid}")]
    [PermissaoNecessaria("admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Deletar(Guid id)
    {
        await _servico.DeletarServicoAsync(id);
        return NoContent();
    }
}
