using Adapters.DTOs.Requests.Estoque;
using Adapters.DTOs.Responses.Estoque;
using API.Models;
using Core.Interfaces.Servicos;
using Infraestrutura.Dados;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EstoqueController : BaseApiController
    {
        private readonly Adapters.Controllers.EstoqueController _estoqueController;

        public EstoqueController(MecanicaContexto contexto, Mediator mediator, IIdCorrelacionalService idCorrelacionalService, HttpContextAccessor httpContext, IConfiguration configuration)
        {
            // Usando o CompositionRoot para criar o controller com dependências externas
            var compositionRoot = new CompositionRoot(contexto, mediator, idCorrelacionalService, httpContext, configuration);
            _estoqueController = compositionRoot.CreateEstoqueController();
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EstoqueResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterTodos()
        {
            var estoques = await _estoqueController.ObterTodos();
            return Ok(estoques);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(EstoqueResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var estoque = await _estoqueController.ObterPorId(id);
            return Ok(estoque);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Criar([FromBody] CadastrarEstoqueRequest request)
        {
            var resultadoValidacao = ValidarModelState();
            if (resultadoValidacao != null) return resultadoValidacao;

            var estoque = await _estoqueController.Cadastrar(request);
            return CreatedAtAction(nameof(ObterPorId), new { id = estoque.Id }, estoque);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(EstoqueResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarEstoqueRequest request)
        {
            var resultadoValidacao = ValidarModelState();
            if (resultadoValidacao != null) return resultadoValidacao;

            var estoqueAtualizado = await _estoqueController.Atualizar(id, request);
            return Ok(estoqueAtualizado);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Remover(Guid id)
        {
            var sucesso = await _estoqueController.Deletar(id);
            if (!sucesso)
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Erro ao remover o item do estoque" });

            return NoContent();
        }
    }
}