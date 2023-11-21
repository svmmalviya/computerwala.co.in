using Microsoft.AspNetCore.Mvc;

namespace computerwala.Areas.Admin.Controllers
{
    [Route("Admin")]
    [Area("Sophisticated")]
    public class AdminController : Controller
    {

        [Route("Dashboard")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
