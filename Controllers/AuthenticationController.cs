using computerwala.Base;
using computerwala.Models;
using DBService.Interfaces;
using DBService.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
            if (!response.Success)
            {
                return View();
            }
            else
            {
                if (response.Success)
                {
                    var user = JsonConvert.DeserializeObject<CWUserData>(response.Data);
                    HttpContext.Session.SetString("Login", user.username);
                    HttpContext.Session.SetString("UserId", user.Id);
                    UserId = user.Id;
                    return RedirectToAction("CWCalender", "Home");
                }
                else
                {
                    HttpContext.Session.Remove("Login");
                    return RedirectToAction("Index", "Home");
                }
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
