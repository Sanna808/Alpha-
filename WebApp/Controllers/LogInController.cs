using Business.Services;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class LogInController : Controller
    {
        private readonly IAuthService _authService;

        public LogInController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Index(string returnUrl = "/")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            ViewBag.ReturnUrl = model.ReturnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var formData = new SignInFormData
            {
                Email = model.Email,
                Password = model.Password,
                IsPersistent = model.RememberMe
            };

            var result = await _authService.SignInAsync(formData);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Projects");
            }

            ViewBag.ErrorMessage = result.Error;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.SignOutAsync();
            return RedirectToAction("Index", "LogIn");
        }
    }
}