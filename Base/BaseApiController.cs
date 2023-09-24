using Microsoft.AspNetCore.Mvc;

namespace computerwala.Base
{
    public class BaseApiController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
