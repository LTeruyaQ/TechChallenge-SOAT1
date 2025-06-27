using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ClienteController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
