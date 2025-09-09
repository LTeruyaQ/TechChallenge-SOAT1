using Adapters.Controllers;
using Adapters.DTOs.Requests.Usuario;
using Adapters.DTOs.Responses.Usuario;
using Adapters.Gateways;
using Adapters.Presenters;
using Adapters.Presenters.Interfaces;
using API.Models;
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
    public class UsuarioController : BaseApiController
    {
        private readonly Adapters.Controllers.UsuarioController _usuarioController;

        public UsuarioController(
            IRepositorio<Core.Entidades.Usuario> repositorioUsuario,
            IRepositorio<Core.Entidades.Cliente> repositorioCliente,
            IUnidadeDeTrabalho unidadeDeTrabalho,
            IUsuarioLogadoServico usuarioLogadoServico,
            IServicoSenha servicoSenha)
        {
            // Criando gateways
            IUsuarioGateway usuarioGateway = new UsuarioGateway(repositorioUsuario);
            IClienteGateway clienteGateway = new ClienteGateway(repositorioCliente);
            
            // Criando logs
            ILogServico<ClienteUseCases> logClienteUseCases = new LogServico<ClienteUseCases>();
            ILogServico<UsuarioUseCases> logUsuarioUseCases = new LogServico<UsuarioUseCases>();
            
            // Criando use cases
            IClienteUseCases clienteUseCases = new ClienteUseCases(
                clienteGateway, 
                null, // enderecoGateway não é necessário para este controller
                null, // contatoGateway não é necessário para este controller
                logClienteUseCases, 
                unidadeDeTrabalho, 
                usuarioLogadoServico);
                
            IUsuarioUseCases usuarioUseCases = new UsuarioUseCases(
                logUsuarioUseCases, 
                unidadeDeTrabalho, 
                clienteUseCases, 
                servicoSenha, 
                usuarioLogadoServico, 
                usuarioGateway);
                
            // Criando presenter
            IUsuarioPresenter usuarioPresenter = new UsuarioPresenter();
                
            // Criando controller
            _usuarioController = new Adapters.Controllers.UsuarioController(usuarioUseCases, usuarioPresenter);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UsuarioResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterTodos()
        {
            try
            {
                var usuarios = await _usuarioController.ObterTodosAsync();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Message = ex.Message });
            }
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            try
            {
                var usuario = await _usuarioController.ObterPorIdAsync(id);
                if (usuario == null)
                    return NotFound(new ErrorResponse { Message = "Usuário não encontrado" });
                    
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Message = ex.Message });
            }
        }
        
        [HttpGet("email/{email}")]
        [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterPorEmail(string email)
        {
            try
            {
                var usuario = await _usuarioController.ObterPorEmailAsync(email);
                if (usuario == null)
                    return NotFound(new ErrorResponse { Message = "Usuário não encontrado" });
                    
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Message = ex.Message });
            }
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

            try
            {
                var usuario = await _usuarioController.CadastrarAsync(request);
                return CreatedAtAction(nameof(ObterPorId), new { id = usuario.Id }, usuario);
            }
            catch (Core.Exceptions.DadosJaCadastradosException ex)
            {
                return Conflict(new ErrorResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Message = ex.Message });
            }
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

            try
            {
                var usuarioAtualizado = await _usuarioController.AtualizarAsync(id, request);
                return Ok(usuarioAtualizado);
            }
            catch (Core.Exceptions.DadosNaoEncontradosException ex)
            {
                return NotFound(new ErrorResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Message = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Deletar(Guid id)
        {
            try
            {
                await _usuarioController.DeletarAsync(id);
                return NoContent();
            }
            catch (Core.Exceptions.DadosNaoEncontradosException ex)
            {
                return NotFound(new ErrorResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Message = ex.Message });
            }
        }
    }
}
