using Microsoft.AspNetCore.Mvc;

namespace computerwala.Areas.Sophisticated.Controllers
{
    [Route("Holidays")]
    [Area("Sophisticated")]
    public class HolidaysController : Controller
    {
        [Route("")]
        [HttpGet]
        public IActionResult Holidays()
        {
            return View();
        }        



    }
}
