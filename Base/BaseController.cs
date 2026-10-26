using Microsoft.AspNetCore.Mvc;

namespace computerwala.Base
{
    public class BaseController : Controller
    {
        public static string UserId = string.Empty; 

        public BaseController()
        {
        }

        public string GetClientIpAddress()
        {
            string clientIp = "";

            if (HttpContext != null)
                clientIp = HttpContext.Connection.RemoteIpAddress.ToString();
            return clientIp;
        }
    }
}
