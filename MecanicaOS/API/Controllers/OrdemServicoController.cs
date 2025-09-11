using Adapters.Controllers;
using Adapters.DTOs.Requests.OrdemServico;
using Adapters.DTOs.Requests.OrdemServico.InsumoOS;
using Adapters.DTOs.Responses.OrdemServico;
using Adapters.Gateways;
using Adapters.Presenters;
using Adapters.Presenters.Interfaces;
using API.Models;
using Core.Entidades;
using Core.Enumeradores;
using Core.Interfaces.Gateways;
using Core.Interfaces.Jobs;
using Core.Interfaces.Repositorios;
using Core.Interfaces.Servicos;
using Core.Interfaces.UseCases;
using Core.UseCases;
using Infraestrutura.Jobs;
using Infraestrutura.Logs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class OrdemServicoController : BaseApiController
    {
        private readonly Adapters.Controllers.OrdemServicoController _ordemServicoController;
        private readonly InsumoOSController _insumoOSController;

        public OrdemServicoController(
            IRepositorio<OrdemServico> repositorioOrdemServico,
            IRepositorio<InsumoOS> repositorioInsumoOS,
            IRepositorio<Estoque> repositorioEstoque,
            IUnidadeDeTrabalho unidadeDeTrabalho,
            IUsuarioLogadoServico usuarioLogadoServico,
            IVerificarEstoqueJob verificarEstoqueJob,
            IIdCorrelacionalService idCorrelacionalService,
            ILogger<OrdemServicoUseCases> loggerOrdemServicoUseCases,
            ILogger<EstoqueUseCases> loggerEstoqueUseCases,
            ILogger<InsumoOSUseCases> loggerInsumoOSUseCases)
        {
            // Criando logs
            ILogServico<OrdemServicoUseCases> logOrdemServicoUseCases = new LogServico<OrdemServicoUseCases>(idCorrelacionalService, loggerOrdemServicoUseCases, usuarioLogadoServico);
            ILogServico<EstoqueUseCases> logEstoqueUseCases = new LogServico<EstoqueUseCases>(idCorrelacionalService, loggerEstoqueUseCases, usuarioLogadoServico);
            ILogServico<InsumoOSUseCases> logInsumoOSUseCases = new LogServico<InsumoOSUseCases>(idCorrelacionalService, loggerInsumoOSUseCases, usuarioLogadoServico);

            // Criando gateways
            IOrdemServicoGateway ordemServicoGateway = new OrdemServicoGateway(repositorioOrdemServico);
            IInsumosGateway insumosGateway = new InsumosGateway(repositorioInsumoOS);
            IEstoqueGateway estoqueGateway = new EstoqueGateway(repositorioEstoque);
            IVerificarEstoqueJobGateway verificarEstoqueJobGateway = new VerificarEstoqueJobGateway(verificarEstoqueJob);

            // Criando presenter
            IOrdemServicoPresenter ordemServicoPresenter = new OrdemServicoPresenter();

            // Criando use cases
            IEstoqueUseCases estoqueUseCases = new EstoqueUseCases(
                estoqueGateway,
                logEstoqueUseCases,
                unidadeDeTrabalho,
                usuarioLogadoServico);

            IOrdemServicoUseCases ordemServicoUseCases = new OrdemServicoUseCases(
                logOrdemServicoUseCases,
                unidadeDeTrabalho,
                ordemServicoGateway,
                usuarioLogadoServico);

            IInsumoOSUseCases insumoOSUseCases = new InsumoOSUseCases(
                ordemServicoUseCases,
                estoqueUseCases,
                insumosGateway,
                logInsumoOSUseCases,
                unidadeDeTrabalho,
                usuarioLogadoServico,
                verificarEstoqueJobGateway);

            // Criando controllers
            _ordemServicoController = new Adapters.Controllers.OrdemServicoController(ordemServicoUseCases, ordemServicoPresenter);
            _insumoOSController = new InsumoOSController(insumoOSUseCases, ordemServicoPresenter);
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
        [ProducesResponseType(typeof(IEnumerable<Core.Entidades.InsumoOS>), StatusCodes.Status200OK)]
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