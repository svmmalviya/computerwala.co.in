using computerwala.Base;
using DBService.Interfaces;
using DBService.Models;
using Microsoft.AspNetCore.Mvc;

namespace computerwala.Controllers
{
    public class AuthenticationController : BaseController
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IAuthentication _authentication;

        public AuthenticationController(IAuthentication authentication, ILogger<AuthenticationController> logger)
        {
            _authentication = authentication;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(CWLoginCredentials cWLoginCredentials)
        {
            var response = await _authentication.CheckLogin(cWLoginCredentials.Username, cWLoginCredentials.Pwd);
            if (response == null)
            {
                return View();
            }
            else
            {
                HttpContext.Session.SetString("Login", cWLoginCredentials.Username.ToString());
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("Login");
            return RedirectToAction("Index", "Home");
        }

    }
}
