using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class OrdemServicoController : BaseApiController
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Ordem de Serviço API está funcionando!");
    }
}
