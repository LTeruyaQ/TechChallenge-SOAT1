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
        try
        {
            var response = await _veiculoServico.CadastrarAsync(request);
            return CreatedAtAction(nameof(ObterPorId), new { id = response.Id }, response);
        }
        catch (RegistroNaoEncontradoException ex)
        {
            _logger.LogWarning(ex, "Erro ao cadastrar veículo: {Mensagem}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao cadastrar veículo");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno ao processar sua solicitação." });
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Deletar(Guid id)
    {
        try
        {
            var sucesso = await _veiculoServico.DeletarAsync(id);
            if (!sucesso)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro ao remover o veículo." });

            return NoContent();
        }
        catch (RegistroNaoEncontradoException ex)
        {
            _logger.LogWarning(ex, "Veículo não encontrado: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover veículo: {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno ao processar sua solicitação." });
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Editar(Guid id, [FromBody] AtualizarVeiculoRequest request)
    {
        try
        {
            var response = await _veiculoServico.AtualizarAsync(id, request);
            return Ok(response);
        }
        catch (RegistroNaoEncontradoException ex)
        {
            _logger.LogWarning(ex, "Veículo não encontrado: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar veículo: {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno ao processar sua solicitação." });
        }
    }

    [HttpGet("cliente/{clienteId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<VeiculoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterPorCliente(Guid clienteId)
    {
        try
        {
            var response = await _veiculoServico.ObterPorClienteAsync(clienteId);
            return Ok(response);
        }
        catch (RegistroNaoEncontradoException ex)
        {
            _logger.LogWarning(ex, "Nenhum veículo encontrado para o cliente: {ClienteId}", clienteId);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar veículos do cliente: {ClienteId}", clienteId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno ao processar sua solicitação." });
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(VeiculoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        try
        {
            var response = await _veiculoServico.ObterPorIdAsync(id);
            return Ok(response);
        }
        catch (RegistroNaoEncontradoException ex)
        {
            _logger.LogWarning(ex, "Veículo não encontrado: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar veículo: {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno ao processar sua solicitação." });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VeiculoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterTodos()
    {
        try
        {
            var response = await _veiculoServico.ObterTodosAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar veículos");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno ao processar sua solicitação." });
        }
    }
}