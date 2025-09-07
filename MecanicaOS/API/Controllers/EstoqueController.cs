using API.Models;
using Aplicacao.DTOs.Responses.Estoque;
using Aplicacao.Interfaces.Servicos;
using Aplicacao.UseCases.Estoque.AtualizarEstoque;
using Aplicacao.UseCases.Estoque.CriarEstoque;
using Aplicacao.UseCases.Estoque.DeletarEstoque;
using Dominio.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize(Roles = "Admin")]
public class EstoqueController(IEstoqueServico estoqueService, 
    ILogger<EstoqueController> logger, 
    ICriarEstoqueUseCase criarEstoqueUseCase, 
    IAtualizarEstoqueUseCase atualizarEstoqueUseCase,
    IDeletarEstoqueUseCase deletarEstoqueUseCase) : BaseApiController
{
    private readonly ICriarEstoqueUseCase criarEstoqueUseCase = criarEstoqueUseCase;
    private readonly IAtualizarEstoqueUseCase atualizarEstoqueUseCase = atualizarEstoqueUseCase;
    private readonly IDeletarEstoqueUseCase deletarEstoqueUseCase = deletarEstoqueUseCase;
    private readonly IEstoqueServico _estoqueService = estoqueService;
    private readonly ILogger<EstoqueController> _logger = logger;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EstoqueResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterTodos()
    {
        var estoques = await _estoqueService.ObterTodosAsync();
        return Ok(estoques);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EstoqueResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var estoque = await _estoqueService.ObterPorIdAsync(id);
        return Ok(estoque);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CriarEstoqueResponse>> Criar([FromBody] CriarEstoqueRequest request)
    {
        try
        {
            _logger.LogInformation("Iniciando criação de estoque {@Request}", request);

            var response = await criarEstoqueUseCase.ExecuteAsync(request);

            _logger.LogInformation("Estoque criado com sucesso {@Response}", response);

            return CreatedAtAction(nameof(ObterPorId), new { id = response.Id }, response);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Regra de negócio violada ao criar estoque");
            return BadRequest(new ErrorResponse(400, ex.Message));
        }
        catch (PersistirDadosException ex)
        {
            _logger.LogError(ex, "Erro ao persistir dados");
            return StatusCode(500, new ErrorResponse(500, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar estoque");
            return StatusCode(500, new ErrorResponse(500, "Erro interno no servidor"));
        }
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CriarEstoqueResponse>> AtualizarParcial(Guid id, [FromBody] AtualizarEstoqueRequest request)
    {
        try
        {
            _logger.LogInformation("Iniciando atualização parcial de estoque {@Request} para ID {Id}", request, id);

            var response = await atualizarEstoqueUseCase.ExecuteAsync(id, request);

            _logger.LogInformation("Estoque atualizado com sucesso {@Response}", response);

            return Ok(response);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Regra de negócio violada ao atualizar estoque ID {Id}", id);
            return BadRequest(new ErrorResponse(400, ex.Message));
        }
        catch (PersistirDadosException ex)
        {
            _logger.LogError(ex, "Erro ao persistir dados ao atualizar estoque ID {Id}", id);
            return StatusCode(500, new ErrorResponse(500, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao atualizar estoque ID {Id}", id);
            return StatusCode(500, new ErrorResponse(500, "Erro interno no servidor"));
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Remover(Guid id)
    {
        try
        {
            _logger.LogInformation("Iniciando exclusão de estoque {@Id}", id);

            var sucesso = await deletarEstoqueUseCase.ExecuteAsync(id);

            if (!sucesso)
            {
                _logger.LogError("Erro ao remover o estoque {@Id}", id);
                return StatusCode(500, new ErrorResponse(500, "Erro ao remover o item do estoque"));
            }

            _logger.LogInformation("Estoque removido com sucesso {@Id}", id);
            return NoContent();
        }
        catch (DadosNaoEncontradosException ex)
        {
            _logger.LogWarning(ex, "Estoque não encontrado {@Id}", id);
            return NotFound(new ErrorResponse(404, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao remover estoque {@Id}", id);
            return StatusCode(500, new ErrorResponse(500, "Erro interno no servidor"));
        }
    }
}