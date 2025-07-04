using Aplicacao.DTOs.Veiculo;
using Dominio.Exceptions;
using Dominio.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VeiculosController : ControllerBase
    {
        private readonly IVeiculoServico _veiculoService;

        public VeiculosController(IVeiculoServico veiculoService)
        {
            _veiculoService = veiculoService;
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] CadastrarVeiculoDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var veiculo = await _veiculoService.CadastrarAsync(dto);

                return CreatedAtAction(nameof(ObterPorId), new { id = veiculo.Id }, veiculo);
            }
            catch (DadosInvalidosException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (EntidadeNaoEncontradaException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Erro ao tentar cadastrar o veículo.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest("ID inválido.");

                await _veiculoService.RemoverAsync(id);

                return NoContent();
            }
            catch (EntidadeNaoEncontradaException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Erro ao tentar deletar o veículo.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(Guid id, [FromBody] EditarVeiculoDto dto)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest("ID inválido.");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _veiculoService.AtualizarAsync(id, dto);

                return NoContent();
            }
            catch (DadosInvalidosException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (EntidadeNaoEncontradaException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Erro ao tentar atualizar o veículo.");
            }
        }

        [HttpGet("cliente/{clienteId}")]
        public async Task<IActionResult> ObterPorCliente(Guid clienteId)
        {
            try
            {
                if (clienteId == Guid.Empty)
                    return BadRequest("ID do cliente inválido.");

                var veiculos = await _veiculoService.ObterPorClienteAsync(clienteId);
                return Ok(veiculos);
            }
            catch (EntidadeNaoEncontradaException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro inesperado ao obter os veículos do cliente.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest("ID inválido.");

                var veiculo = await _veiculoService.ObterPorIdAsync(id);

                if (veiculo == null)
                    return NotFound("Veículo não encontrado.");

                return Ok(veiculo);
            }
            catch (EntidadeNaoEncontradaException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro inesperado ao obter o veículo.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            try
            {
                var veiculos = await _veiculoService.ObterTodosAsync();
                return Ok(veiculos);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro inesperado ao listar os veículos.");
            }
        }
    }
}