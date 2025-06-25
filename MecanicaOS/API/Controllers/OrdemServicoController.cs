using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class OrdemServicoController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
