using Adapters.DTOs.Requests.Veiculo;
using Adapters.DTOs.Responses.Veiculo;
using Adapters.Gateways;
using Adapters.Presenters;
using Adapters.Presenters.Interfaces;
using API.Models;
using Core.Interfaces.Gateways;
using Core.DTOs.Entidades.Veiculo;
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
    public class VeiculoController : BaseApiController
    {
        private readonly Adapters.Controllers.VeiculoController _veiculoController;
        private readonly ILogger<VeiculoController> _logger;

        public VeiculoController(
            IRepositorio<VeiculoEntityDto> repositorioVeiculo,
            IUnidadeDeTrabalho unidadeDeTrabalho,
            IUsuarioLogadoServico usuarioLogadoServico,
            ILogger<VeiculoController> logger,
            ILogger<VeiculoUseCases> loggerVeiculoUseCase,
            IIdCorrelacionalService idCorrelacionalService)
        {
            _logger = logger;

            // Criando gateways
            IVeiculoGateway veiculoGateway = new VeiculoGateway(repositorioVeiculo);

            // Criando logs
            ILogServico<VeiculoUseCases> logVeiculoUseCases = new LogServico<VeiculoUseCases>(
                idCorrelacionalService,
                loggerVeiculoUseCase,
                usuarioLogadoServico
            );

            // Criando use cases
            IVeiculoUseCases veiculoUseCases = new VeiculoUseCases(
                logVeiculoUseCases,
                unidadeDeTrabalho,
                usuarioLogadoServico,
                veiculoGateway
            );

            // Criando presenter
            IVeiculoPresenter veiculoPresenter = new VeiculoPresenter();

            // Criando controller
            _veiculoController = new Adapters.Controllers.VeiculoController(veiculoUseCases, veiculoPresenter);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Cadastrar([FromBody] CadastrarVeiculoRequest request)
        {
            var resultadoValidacao = ValidarModelState();
            if (resultadoValidacao != null) return resultadoValidacao;

            _logger.LogInformation("Iniciando cadastro de veículo");
            var response = await _veiculoController.Cadastrar(request);
            _logger.LogInformation("Veículo cadastrado com sucesso: {Id}", response.Id);
            return CreatedAtAction(nameof(ObterPorId), new { id = response.Id }, response);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Deletar(Guid id)
        {
            _logger.LogInformation("Iniciando remoção de veículo: {Id}", id);
            var sucesso = await _veiculoController.Deletar(id);
            if (!sucesso)
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Message = "Ocorreu um erro ao remover o veículo." });

            _logger.LogInformation("Veículo removido com sucesso: {Id}", id);
            return NoContent();
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(VeiculoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Editar(Guid id, [FromBody] AtualizarVeiculoRequest request)
        {
            var resultadoValidacao = ValidarModelState();
            if (resultadoValidacao != null) return resultadoValidacao;

            _logger.LogInformation("Iniciando atualização de veículo: {Id}", id);
            var response = await _veiculoController.Atualizar(id, request);
            _logger.LogInformation("Veículo atualizado com sucesso: {Id}", id);
            return Ok(response);
        }

        [HttpGet("cliente/{clienteId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<VeiculoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterPorCliente(Guid clienteId)
        {
            _logger.LogInformation("Obtendo veículos do cliente: {ClienteId}", clienteId);
            var response = await _veiculoController.ObterPorCliente(clienteId);
            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(VeiculoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            _logger.LogInformation("Obtendo veículo por ID: {Id}", id);
            var response = await _veiculoController.ObterPorId(id);
            if (response == null)
                return NotFound(new ErrorResponse { Message = "Veículo não encontrado" });

            return Ok(response);
        }

        [HttpGet("placa/{placa}")]
        [ProducesResponseType(typeof(VeiculoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterPorPlaca(string placa)
        {
            _logger.LogInformation("Obtendo veículo por placa: {Placa}", placa);
            var response = await _veiculoController.ObterPorPlaca(placa);
            if (response == null)
                return NotFound(new ErrorResponse { Message = "Veículo não encontrado" });

            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<VeiculoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterTodos()
        {
            _logger.LogInformation("Obtendo todos os veículos");
            var response = await _veiculoController.ObterTodos();
            return Ok(response);
        }
    }
}