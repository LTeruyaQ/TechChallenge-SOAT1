using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("[controller]")]
[ApiController]
[Produces("application/json")]
[Consumes("application/json")]
public abstract class BaseApiController : ControllerBase
{
    protected IActionResult ValidarModelState()
    {
        if (!ModelState.IsValid)
        {
            var erros = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .Where(m => !string.IsNullOrEmpty(m));

            return BadRequest(new { erros });
        }

        return null;
    }
}
