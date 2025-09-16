using Adapters.DTOs.Requests.Usuario;
using Adapters.DTOs.Responses.Usuario;
using API.Models;
using Core.Interfaces.Servicos;
using Infraestrutura.Dados;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsuarioController : BaseApiController
    {
        private readonly Adapters.Controllers.UsuarioController _usuarioController;

        public UsuarioController(
            MecanicaContexto contexto,
            Mediator mediator,
            IServicoEmail servicoEmail,
            IIdCorrelacionalService idCorrelacionalService,
            HttpContextAccessor httpContext)
        {
            // Usando o CompositionRoot para criar o controller com dependências externas
            var compositionRoot = new CompositionRoot(contexto, mediator, servicoEmail, idCorrelacionalService, httpContext);
            _usuarioController = compositionRoot.CreateUsuarioController();
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UsuarioResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterTodos()
        {
            var usuarios = await _usuarioController.ObterTodosAsync();
            return Ok(usuarios);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var usuario = await _usuarioController.ObterPorIdAsync(id);
            if (usuario == null)
                return NotFound(new ErrorResponse { Message = "Usuário não encontrado" });

            return Ok(usuario);
        }

        [HttpGet("email/{email}")]
        [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterPorEmail(string email)
        {
            var usuario = await _usuarioController.ObterPorEmailAsync(email);
            if (usuario == null)
                return NotFound(new ErrorResponse { Message = "Usuário não encontrado" });

            return Ok(usuario);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Criar([FromBody] CadastrarUsuarioRequest request)
        {
            var resultadoValidacao = ValidarModelState();
            if (resultadoValidacao != null) return resultadoValidacao;

            var usuario = await _usuarioController.CadastrarAsync(request);
            return CreatedAtAction(nameof(ObterPorId), new { id = usuario.Id }, usuario);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarUsuarioRequest request)
        {
            var resultadoValidacao = ValidarModelState();
            if (resultadoValidacao != null) return resultadoValidacao;

            var usuarioAtualizado = await _usuarioController.AtualizarAsync(id, request);
            return Ok(usuarioAtualizado);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Deletar(Guid id)
        {
            await _usuarioController.DeletarAsync(id);
            return NoContent();
        }
    }
}
