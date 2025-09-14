using Adapters.DTOs.Requests.Cliente;
using Adapters.DTOs.Responses.Cliente;
using Adapters.Gateways;
using Adapters.Presenters;
using Adapters.Presenters.Interfaces;
using API.Models;
using Core.Entidades;
using Core.DTOs.Repositories.Cliente;
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
    [Authorize]
    public class ClienteController : BaseApiController
    {
        private readonly Adapters.Controllers.ClienteController _clienteController;

        public ClienteController(
            IRepositorio<ClienteRepositoryDTO> repositorioCliente,
            IRepositorio<EnderecoRepositoryDto> repositorioEndereco,
            IRepositorio<ContatoRepositoryDTO> repositorioContato,
            IUnidadeDeTrabalho unidadeDeTrabalho,
            IUsuarioLogadoServico usuarioLogadoServico,
            IdCorrelacionalService idCorrelacionalService,
            ILogger<ClienteUseCases> loggerClienteUseCases)
        {
            // Criando gateways
            IClienteGateway clienteGateway = new ClienteGateway(repositorioCliente);
            IEnderecoGateway enderecoGateway = new EnderecoGateway(repositorioEndereco);
            IContatoGateway contatoGateway = new ContatoGateway(repositorioContato);

            // Criando logs
            ILogServico<ClienteUseCases> logClienteUseCases = new LogServico<ClienteUseCases>(idCorrelacionalService, loggerClienteUseCases, usuarioLogadoServico);

            // Criando use cases
            IClienteUseCases clienteUseCases = new ClienteUseCases(
                clienteGateway,
                enderecoGateway,
                contatoGateway,
                logClienteUseCases,
                unidadeDeTrabalho,
                usuarioLogadoServico);

            // Criando presenter
            IClientePresenter clientePresenter = new ClientePresenter();

            // Criando controller
            _clienteController = new Adapters.Controllers.ClienteController(clienteUseCases, clientePresenter);
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
