using Microsoft.AspNetCore.Mvc;

namespace computerwala.Base
{
    public class BaseController : Controller
    {

        public string GetClientIpAddress()
        {
            string clientIp = "";

            if (HttpContext != null)
                clientIp = HttpContext.Connection.RemoteIpAddress.ToString();
            return clientIp;
        }
    }
}
