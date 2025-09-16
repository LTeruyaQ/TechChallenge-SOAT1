using Adapters.DTOs.Requests.Cliente;
using Adapters.DTOs.Responses.Cliente;
using API.Models;
using Core.Interfaces.Servicos;
using Infraestrutura.Dados;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class ClienteController : BaseApiController
    {
        private readonly Adapters.Controllers.ClienteController _clienteController;

        public ClienteController(MecanicaContexto contexto, Mediator mediator, IIdCorrelacionalService idCorrelacionalService, HttpContextAccessor httpContext, IConfiguration configuration)
        {
            // Usando o CompositionRoot para criar o controller com dependências externas
            var compositionRoot = new CompositionRoot(contexto, mediator, idCorrelacionalService, httpContext, configuration);
            _clienteController = compositionRoot.CreateClienteController();
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Cliente")]
        [ProducesResponseType(typeof(IEnumerable<ClienteResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterTodos()
        {
            return Ok(await _clienteController.ObterTodos());
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            return Ok(await _clienteController.ObterPorId(id));
        }

        [HttpGet("documento/{documento}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterPorDocumento(string documento)
        {
            return Ok(await _clienteController.ObterPorDocumento(documento));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Criar([FromBody] CadastrarClienteRequest request)
        {
            var resultadoValidacao = ValidarModelState();
            if (resultadoValidacao != null) return resultadoValidacao;

            var cliente = await _clienteController.Cadastrar(request);

            return CreatedAtAction(nameof(ObterPorId), new { id = cliente.Id }, cliente);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarClienteRequest request)
        {
            var resultadoValidacao = ValidarModelState();
            if (resultadoValidacao != null) return resultadoValidacao;

            return Ok(await _clienteController.Atualizar(id, request));
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Remover(Guid id)
        {
            await _clienteController.Remover(id);
            return NoContent();
        }
    }
}
