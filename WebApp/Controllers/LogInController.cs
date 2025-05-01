using Business.Services;
using Domain.Extentions;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class LogInController(IAuthService authService) : Controller
{
    private readonly IAuthService _authService = authService;

    public IActionResult Index(string returnUrl = "~/")
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]

    public async Task<IActionResult> Index(LoginViewModel model, string returnUrl = "~/")
    {
        ViewBag.ErrorMessage = null;
        ViewBag.ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        var signUpFormData = model.MapTo<SignUpFormData>();
        var result = await _authService.SignUpAsync(signUpFormData);
        if (result.Succeeded)
        {
            return LocalRedirect(returnUrl);
        }

        ViewBag.ErrorMessage = result.Error;
        return View(model);
    }
}
