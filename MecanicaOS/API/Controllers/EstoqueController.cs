using Adapters.DTOs.Requests.Estoque;
using Adapters.DTOs.Responses.Estoque;
using Adapters.Gateways;
using Adapters.Presenters;
using Adapters.Presenters.Interfaces;
using API.Models;
using Core.Entidades;
using Core.DTOs.Repositories.Estoque;
using Core.Interfaces.Gateways;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;
using Core.UseCases;
using Infraestrutura.Logs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EstoqueController : BaseApiController
    {
        private readonly Adapters.Controllers.EstoqueController _estoqueController;

        public EstoqueController(
            IRepositorio<EstoqueRepositoryDto> repositorioEstoque,
            IUnidadeDeTrabalho unidadeDeTrabalho,
            IUsuarioLogadoServico usuarioLogadoServico,
            IdCorrelacionalService idCorrelacionalService,
            ILogger<EstoqueUseCases> loggerEstoqueUseCases)
        {

            // Criando gateways
            IEstoqueGateway estoqueGateway = new EstoqueGateway(repositorioEstoque);

            // Criando logs
            ILogServico<EstoqueUseCases> logEstoqueUseCases = new LogServico<EstoqueUseCases>(idCorrelacionalService, loggerEstoqueUseCases, usuarioLogadoServico);

            // Criando use cases
            IEstoqueUseCases estoqueUseCases = new EstoqueUseCases(
                estoqueGateway,
                logEstoqueUseCases,
                unidadeDeTrabalho,
                usuarioLogadoServico);

            // Criando presenter
            IEstoquePresenter estoquePresenter = new EstoquePresenter();

            // Criando controller
            _estoqueController = new Adapters.Controllers.EstoqueController(estoqueUseCases, estoquePresenter);
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