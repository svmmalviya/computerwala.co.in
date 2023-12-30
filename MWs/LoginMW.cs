using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace computerwala.MWs
{
    public class LoginMW
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MaintenanceMW> _logger;
        private readonly IWebHostEnvironment _environment;

        public LoginMW(RequestDelegate next, IConfiguration configuration, ILogger<MaintenanceMW> logger, IWebHostEnvironment webHost)
        {
            _configuration = configuration;
            this._logger = logger;
            _next = next;
            _environment = webHost;
        }



        public LoginMW()
        {

        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("In Login MW");

            var sessionLoginVal = Convert.ToInt32(context.Session.GetInt32("IsLoggedIn"));
            var isLoggin = sessionLoginVal != 0;

            if (context.Request.GetDisplayUrl().ToLower().Contains("/admin"))
                if (!isLoggin)
                {
                    if (!context.Response.HasStarted)
                    {
                        var returnUrl = context.Request.GetDisplayUrl();
                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            context.Session.SetString("ReturnUrl", returnUrl);
                        }
                        context.Response.Redirect("/Home/LoginView", permanent: false);
                    }
                    return;
                }

            await _next(context);
        }

        // Implement the ObtainNewToken method as needed

    }
}
