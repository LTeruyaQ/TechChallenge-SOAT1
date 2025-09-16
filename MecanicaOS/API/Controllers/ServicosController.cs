using Adapters.Controllers;
using Adapters.DTOs.Requests.Servico;
using Adapters.DTOs.Responses.Servico;
using API.Models;
using Core.Interfaces.Servicos;
using Infraestrutura.Dados;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class ServicosController : BaseApiController
    {
        private readonly ServicoController _servicoController;

        public ServicosController(MecanicaContexto contexto, Mediator mediator, IServicoEmail servicoEmail, IIdCorrelacionalService idCorrelacionalService, HttpContextAccessor httpContext)
        {
            var compositionRoot = new CompositionRoot(contexto, mediator, servicoEmail, idCorrelacionalService, httpContext);
            _servicoController = compositionRoot.CreateServicoController();
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ServicoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterTodos()
        {
            var servicos = await _servicoController.ObterTodos();
            return Ok(servicos);
        }

        [HttpGet("disponiveis")]
        [ProducesResponseType(typeof(IEnumerable<ServicoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterServicosDisponiveis()
        {
            var servicos = await _servicoController.ObterServicosDisponiveis();
            return Ok(servicos);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ServicoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var servico = await _servicoController.ObterPorId(id);
            if (servico == null)
                return NotFound(new ErrorResponse { Message = "Serviço não encontrado" });

            return Ok(servico);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Criar([FromBody] CadastrarServicoRequest request)
        {
            var resultadoValidacao = ValidarModelState();
            if (resultadoValidacao != null) return resultadoValidacao;

            var servico = await _servicoController.Criar(request);
            return CreatedAtAction(nameof(ObterPorId), new { id = servico.Id }, servico);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ServicoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Atualizar(Guid id, [FromBody] EditarServicoRequest request)
        {
            var resultadoValidacao = ValidarModelState();
            if (resultadoValidacao != null) return resultadoValidacao;

            var servicoAtualizado = await _servicoController.Atualizar(id, request);
            return Ok(servicoAtualizado);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Deletar(Guid id)
        {
            await _servicoController.Deletar(id);
            return NoContent();
        }
    }
}
