using Adapters.DTOs.Requests.Usuario;
using Adapters.DTOs.Responses.Usuario;
using Adapters.Gateways;
using Adapters.Presenters;
using Adapters.Presenters.Interfaces;
using API.Models;
using Core.Entidades;
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
            IRepositorio<Usuario> repositorioUsuario,
            IRepositorio<Cliente> repositorioCliente,
            IRepositorio<Endereco> repositorioEndereco,
            IRepositorio<Contato> repositorioContato,
            IUnidadeDeTrabalho unidadeDeTrabalho,
            IUsuarioLogadoServico usuarioLogadoServico,
            IServicoSenha servicoSenha,
            IIdCorrelacionalService idCorrelacionalService,
            ILogger<ClienteUseCases> loggerClienteUseCase,
            ILogger<UsuarioUseCases> loggerUsuarioUseCase
            )
        {
            // Criando gateways
            IUsuarioGateway usuarioGateway = new UsuarioGateway(repositorioUsuario);
            IClienteGateway clienteGateway = new ClienteGateway(repositorioCliente);
            IEnderecoGateway EnderecoGateway = new EnderecoGateway(repositorioEndereco);
            IContatoGateway ContatoGateway = new ContatoGateway(repositorioContato);

            // Criando logs
            ILogServico<ClienteUseCases> logClienteUseCases = new LogServico<ClienteUseCases>(idCorrelacionalService, loggerClienteUseCase, usuarioLogadoServico);
            ILogServico<UsuarioUseCases> logUsuarioUseCases = new LogServico<UsuarioUseCases>(idCorrelacionalService, loggerUsuarioUseCase, usuarioLogadoServico);

            // Criando use cases
            IClienteUseCases clienteUseCases = new ClienteUseCases(
                clienteGateway,
                EnderecoGateway,
                ContatoGateway,
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
