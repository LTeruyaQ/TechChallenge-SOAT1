using Adapters.Controllers;
using Adapters.DTOs.Requests.OrdemServico;
using Adapters.DTOs.Requests.OrdemServico.InsumoOS;
using Adapters.DTOs.Responses.OrdemServico;
using API.Models;
using Core.Enumeradores;
using Core.Interfaces.Servicos;
using Infraestrutura.Dados;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class OrdemServicoController : BaseApiController
    {
        private readonly Adapters.Controllers.OrdemServicoController _ordemServicoController;
        private readonly InsumoOSController _insumoOSController;

        public OrdemServicoController(MecanicaContexto contexto, Mediator mediator, IServicoEmail servicoEmail, IIdCorrelacionalService idCorrelacionalService, HttpContextAccessor httpContext)
        {
            // Usando o CompositionRoot para criar os controllers com dependências externas
            var compositionRoot = new CompositionRoot(contexto, mediator, servicoEmail, idCorrelacionalService, httpContext);
            _ordemServicoController = compositionRoot.CreateOrdemServicoController();
            _insumoOSController = compositionRoot.CreateInsumoOSController();
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrdemServicoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterTodos()
        {
            var ordemServicos = await _ordemServicoController.ObterTodos();
            return Ok(ordemServicos);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(OrdemServicoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var ordemServico = await _ordemServicoController.ObterPorId(id);
            if (ordemServico == null)
                return NotFound(new ErrorResponse { Message = "Ordem de Serviço não encontrada" });

            return Ok(ordemServico);
        }

        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(IEnumerable<OrdemServicoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterPorStatus(StatusOrdemServico status)
        {
            var ordensServico = await _ordemServicoController.ObterPorStatus(status);
            if (ordensServico == null || !ordensServico.Any())
                return NotFound(new ErrorResponse { Message = "Nenhuma ordem de serviço encontrada com o status informado" });

            return Ok(ordensServico);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Criar([FromBody] CadastrarOrdemServicoRequest request)
        {
            var resultadoValidacao = ValidarModelState();
            if (resultadoValidacao != null) return resultadoValidacao;

            var ordemServico = await _ordemServicoController.Cadastrar(request);
            return CreatedAtAction(nameof(ObterPorId), new { id = ordemServico.Id }, ordemServico);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(OrdemServicoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarOrdemServicoRequest request)
        {
            var resultadoValidacao = ValidarModelState();
            if (resultadoValidacao != null) return resultadoValidacao;

            var ordemServicoAtualizado = await _ordemServicoController.Atualizar(id, request);
            return Ok(ordemServicoAtualizado);
        }

        [HttpPost("{ordemServicoId}/insumos")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<Core.DTOs.Entidades.OrdemServicos.InsumoOSEntityDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AdicionarInsumosOS(Guid ordemServicoId, List<CadastrarInsumoOSRequest> request)
        {
            var resultadoValidacao = ValidarModelState();
            if (resultadoValidacao != null) return resultadoValidacao;

            var insumosOS = await _insumoOSController.CadastrarInsumos(ordemServicoId, request);
            return Ok(insumosOS);
        }

        [HttpPatch("{id}/aceitar-orcamento")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AceitarOrcamento(Guid id)
        {
            await _ordemServicoController.AceitarOrcamento(id);
            return NoContent();
        }

        [HttpPatch("{id}/recusar-orcamento")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RecusarOrcamento(Guid id)
        {
            await _ordemServicoController.RecusarOrcamento(id);
            return NoContent();
        }
    }
}