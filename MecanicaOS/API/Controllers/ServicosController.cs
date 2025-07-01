using Dominio.Entidades;
using Dominio.Interfaces.Services;
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
        public async Task<ActionResult<IEnumerable<Servico>>> ObterTodos()
        {
            var servicos = await _servico.ObterTodos();
            return Ok(servicos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Servico>> ObterPorId(Guid id)
        {
            var servico = await _servico.ObterServicoPorId(id);
            
            return Ok(servico);
        }

        [HttpPost]
        public async Task<ActionResult> Criar([FromBody] Servico novoServico)
        {
            await _servico.CadastrarServico(novoServico);
            return CreatedAtAction(nameof(ObterPorId), new { id = novoServico.Id }, novoServico);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Editar(Guid id, [FromBody] Servico servicoAtualizado)
        {
            try
            {
                await _servico.EditarServico(id, servicoAtualizado);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Deletar(Guid id)
        {
            try
            {
                await _servico.DeletarServico(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }

}
