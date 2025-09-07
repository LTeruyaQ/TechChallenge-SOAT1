using API.Models;
using API.Presenters;
using Aplicacao.UseCases.Estoque;
using Aplicacao.UseCases.Estoque.AtualizarEstoque;
using Aplicacao.UseCases.Estoque.CriarEstoque;
using Aplicacao.UseCases.Estoque.DeletarEstoque;
using Aplicacao.UseCases.Estoque.ListaEstoque;
using Aplicacao.UseCases.Estoque.ObterEstoque;
using Dominio.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize(Roles = "Admin")]
public class EstoqueController(
    ILogger<EstoqueController> logger,
    ICriarEstoqueUseCase criarEstoqueUseCase,
    IAtualizarEstoqueUseCase atualizarEstoqueUseCase,
    IDeletarEstoqueUseCase deletarEstoqueUseCase,
    IObterEstoquePorIdUseCase obterEstoquePorIdUseCase,
    IListarEstoqueUseCase listarEstoqueUseCase) : BaseApiController
{
    private readonly ICriarEstoqueUseCase criarEstoqueUseCase = criarEstoqueUseCase;
    private readonly IAtualizarEstoqueUseCase atualizarEstoqueUseCase = atualizarEstoqueUseCase;
    private readonly IDeletarEstoqueUseCase deletarEstoqueUseCase = deletarEstoqueUseCase;
    private readonly IObterEstoquePorIdUseCase obterEstoquePorIdUseCase = obterEstoquePorIdUseCase;
    private readonly IListarEstoqueUseCase listarEstoqueUseCase = listarEstoqueUseCase;
    private readonly ILogger<EstoqueController> _logger = logger;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EstoqueResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterTodos()
    {
        try
        {
            _logger.LogInformation("Iniciando consulta de estoques");

            var estoques = await listarEstoqueUseCase.ExecutarAsync();
            var response = EstoquePresenter.ParaIEnumerableResponse(estoques);

            _logger.LogInformation("Estoques consultados com sucesso");

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao consultar estoques");
            return StatusCode(500, new ErrorResponse(500, "Erro interno no servidor"));
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EstoqueResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        try
        {
            _logger.LogInformation("Iniciando consulta de estoque {@Id}", id);

            var estoque = await obterEstoquePorIdUseCase.ExecutarAsync(id);
            var response = EstoquePresenter.ParaResponse(estoque);

            _logger.LogInformation("Estoque consultado com sucesso {@Id}", id);

            return Ok(estoque);
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

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EstoqueResponse>> Criar([FromBody] CriarEstoqueRequest request)
    {
        try
        {
            _logger.LogInformation("Iniciando criação de estoque {@Request}", request);

            var estoque = await criarEstoqueUseCase.ExecutarAsync(request);
            var response = EstoquePresenter.ParaResponse(estoque);

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
    public async Task<ActionResult<EstoqueResponse>> Atualizar(Guid id, [FromBody] AtualizarEstoqueRequest request)
    {
        try
        {
            _logger.LogInformation("Iniciando atualização parcial de estoque {@Request} para ID {Id}", request, id);

            var estoque = await atualizarEstoqueUseCase.ExecutarAsync(id, request);
            var response = EstoquePresenter.ParaResponse(estoque);

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

            var sucesso = await deletarEstoqueUseCase.ExecutarAsync(id);

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