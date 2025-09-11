using Adapters.Controllers;
using Adapters.DTOs.Requests.Servico;
using Adapters.DTOs.Responses.Servico;
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
    [Authorize]
    public class ServicosController : BaseApiController
    {
        private readonly ServicoController _servicoController;

        public ServicosController(
            IRepositorio<Servico> repositorioServico,
            IUnidadeDeTrabalho unidadeDeTrabalho,
            IUsuarioLogadoServico usuarioLogadoServico,
            IIdCorrelacionalService idCorrelacionalService,
            ILogger<ServicoUseCases> logger)
        {
            // Criando gateways
            IServicoGateway servicoGateway = new ServicoGateway(repositorioServico);

            // Criando logs
            ILogServico<ServicoUseCases> logServicoUseCases = new LogServico<ServicoUseCases>(
                idCorrelacionalService,
                logger,
                usuarioLogadoServico
            );

            // Criando use cases
            IServicoUseCases servicoUseCases = new ServicoUseCases(
                repositorioServico,
                logServicoUseCases,
                unidadeDeTrabalho,
                usuarioLogadoServico,
                servicoGateway
            );

            // Criando presenter
            IServicoPresenter servicoPresenter = new ServicoPresenter();

            // Criando controller
            _servicoController = new ServicoController(servicoUseCases, servicoPresenter);
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
            // O método do controller da arquitetura limpa espera um id, mas o método da API não usa esse parâmetro
            // Passamos 0 como um valor padrão já que o método não usa esse parâmetro
            var servicos = await _servicoController.ObterServicosDisponiveis(0);
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

            try
            {
                var servico = await _servicoController.Criar(request);
                return CreatedAtAction(nameof(ObterPorId), new { id = servico.Id }, servico);
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
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ServicoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Atualizar(Guid id, [FromBody] EditarServicoRequest request)
        {
            var resultadoValidacao = ValidarModelState();
            if (resultadoValidacao != null) return resultadoValidacao;

            try
            {
                var servicoAtualizado = await _servicoController.Atualizar(id, request);
                return Ok(servicoAtualizado);
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
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Deletar(Guid id)
        {
            try
            {
                await _servicoController.Deletar(id);
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
