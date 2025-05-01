using Business.Services;
using Domain.Extentions;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class SignUpController(IAuthService authService) : Controller
{
    private readonly IAuthService _authService = authService;

    public IActionResult Index()
    {
        return View();
    }


    [HttpPost]

    public async Task<IActionResult> Index(SignUpViewModel model)
    {
        ViewBag.ErrorMessage = null;

        if (!ModelState.IsValid)
            return View(model);

        var signInFormData = model.MapTo<SignInFormData>();
        var result = await _authService.SignInAsync(signInFormData);
        if (result.Succeeded) 
        { 
            return RedirectToAction("LogIn");
        }

        ViewBag.ErrorMessage = result.Error;
        return View(model);
    }
}
