using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace computerwala.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {

            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry, the resource you request could not be found";
                    ViewBag.Path = statusCodeResult.OriginalPath;
                    ViewBag.QS = statusCodeResult.OriginalQueryString;
                    _logger.LogWarning($"404 error occured path = {statusCodeResult.OriginalPath}" +
                        $"and QueryString is = {statusCodeResult.OriginalQueryString}");
                    break;

                default:
                    break;
            }

            return View("404");
        }

        [Route("Error")]
        [AllowAnonymous]
        public IActionResult Error()
        {


            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerFeature>();

            ViewBag.ExceptionPath = exceptionDetails.Path;
            ViewBag.ExceptionMessage = exceptionDetails.Error.Message;
            ViewBag.StackTrace = exceptionDetails.Error.StackTrace;

            _logger.LogError($"The path {exceptionDetails.Path} threw an exception");

            return View("Error");
        }
    }



}
