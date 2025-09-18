using API.Models;
using Aplicacao.DTOs.Requests.Autenticacao;
using Aplicacao.DTOs.Requests.Usuario;
using Aplicacao.DTOs.Responses.Autenticacao;
using Aplicacao.Interfaces.Servicos;
using Dominio.Interfaces.Servicos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AutenticacaoController : BaseApiController
    {
        private readonly IAutenticacaoServico _autenticacaoServico;
        private readonly IUsuarioServico _usuarioServico;
        private readonly IServicoEmail servicoEmail;

        public AutenticacaoController(IAutenticacaoServico autenticacaoServico, IUsuarioServico usuarioServico, IServicoEmail servicoEmail)
        {
            _autenticacaoServico = autenticacaoServico ?? throw new ArgumentNullException(nameof(autenticacaoServico));
            _usuarioServico = usuarioServico ?? throw new ArgumentNullException(nameof(usuarioServico));
            this.servicoEmail = servicoEmail;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AutenticacaoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] AutenticacaoRequest request)
        {
            return ValidarModelState() ?? Ok(await _autenticacaoServico.AutenticarAsync(request));
        }

        [HttpPost("Registrar")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Registrar(CadastrarUsuarioRequest request)
        {
            var usuario = await _usuarioServico.CadastrarAsync(request);
            return CreatedAtAction(nameof(Login), new AutenticacaoRequest { Email = usuario.Email, Senha = usuario.Senha }, usuario);
        }

        [HttpGet("Validar-Token")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult ValidarToken()
        {
            return Ok(new { mensagem = "Token válido" });
        }
    }
}
