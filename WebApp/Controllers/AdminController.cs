using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public class AdminController : Controller
{

    [Route("admin/members")]

    public IActionResult Members()
    {
        return View();
    }

    [Route("admin/clients")]

    public IActionResult Clients()
    {
        return View();
    }
}
