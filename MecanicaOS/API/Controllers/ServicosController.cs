using Dominio.DTOs.Servico;
using Dominio.Exceptions;
using Dominio.Interfaces.Servicos;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ServicosController : ControllerBase
    {
        private readonly IServicoServico _servico;

        public ServicosController(IServicoServico servico)
        {
            _servico = servico;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            var servicos = await _servico.ObterTodosAsync();
            return Ok(servicos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var servico = await _servico.ObterServicoPorIdAsync(id);

            return Ok(servico);
        }


        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CadastrarServicoDto novoServico)
        {
            var servico = await _servico.CadastrarServicoAsync(novoServico);
            return Ok(servico);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(Guid id, [FromBody] EditarServicoDto servicoAtualizado)
        {
            try
            {
                await _servico.EditarServicoAsync(id, servicoAtualizado);
                return CreatedAtAction(nameof(ObterPorId), new { id }, servicoAtualizado);
            }
            catch (EntidadeNaoEncontradaException ex)
            {
                return NotFound(ex.Message);
            }
            catch
            {
                return BadRequest("Erro interno ao editar serviço");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(Guid id)
        {
            try
            {
                await _servico.DeletarServicoAsync(id);
                return NoContent();
            }
            catch (EntidadeNaoEncontradaException ex)
            {
                return NotFound(ex.Message);
            }
            catch
            {
                return BadRequest("Erro interno ao deletar serviço");
            }
        }
    }

}
