using Aplicacao.DTOs.Servico;
using Dominio.Exceptions;
using Dominio.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class VeiculosController : ControllerBase
    {
        private readonly IVeiculoVeiculo _Veiculo;

        public VeiculosController(IVeiculoVeiculo Veiculo)
        {
            _Veiculo = Veiculo;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            var Veiculos = await _Veiculo.ObterTodos();
            return Ok(Veiculos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var Veiculo = await _Veiculo.ObterVeiculoPorId(id);

            return Ok(Veiculo);
        }


        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CadastrarVeiculoDto novoVeiculo)
        {
            var Veiculo = await _Veiculo.CadastrarVeiculo(novoVeiculo);
            return Ok(Veiculo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(Guid id, [FromBody] EditarVeiculoDto VeiculoAtualizado)
        {
            try
            {
                await _Veiculo.EditarVeiculo(id, VeiculoAtualizado);
                return CreatedAtAction(nameof(ObterPorId), new { id }, VeiculoAtualizado);
            }
            catch (EntidadeNaoEncontradaException ex)
            {
                return NotFound(ex.Message);
            }
            catch
            {
                return BadRequest("Erro interno ao editar veiculo");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(Guid id)
        {
            try
            {
                await _Veiculo.DeletarVeiculo(id);
                return NoContent();
            }
            catch (EntidadeNaoEncontradaException ex)
            {
                return NotFound(ex.Message);
            }
            catch
            {
                return BadRequest("Erro interno ao deletar veiculo");
            }
        }
    }

}
