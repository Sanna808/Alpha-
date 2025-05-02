using Business.Services;
using Domain.Extentions;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class SignUpController : Controller
    {
        private readonly IAuthService _authService;

        public SignUpController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SignUpViewModel model)
        {
            ViewBag.ErrorMessage = null;

            if (!ModelState.IsValid)
                return View(model);

            var signUpFormData = model.MapTo<SignUpFormData>();

            var result = await _authService.SignUpAsync(signUpFormData);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "LogIn");
            }

            ViewBag.ErrorMessage = result.Error;
            return View(model);
        }
    }
}
