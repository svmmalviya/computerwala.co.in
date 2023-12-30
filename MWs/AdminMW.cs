using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace computerwala.MWs
{
   

    public class AdminMW
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MaintenanceMW> _logger;
        private readonly IWebHostEnvironment _environment;

        public AdminMW(RequestDelegate next, IConfiguration configuration, ILogger<MaintenanceMW> logger, IWebHostEnvironment webHost)
        {
            _configuration = configuration;
            this._logger = logger;
            _next = next;
            _environment = webHost;
        }

        public AdminMW()
        {

        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("In Admin MW");

            var sessionLoginVal = Convert.ToInt32(context.Session.GetInt32("IsLoggedIn"));
            var sessionUsertype = Convert.ToInt32(context.Session.GetInt32("UserType"));
            var isLoggin = sessionLoginVal != 0;

            if (context.Request.GetDisplayUrl().ToLower().Contains("/admin"))
            {
                /// if usertype is not admin
                if (isLoggin && sessionUsertype != 2)
                {
                    if (!context.Response.HasStarted)
                    {
                        context.Response.Redirect("/Home", permanent: false);
                    }
                    return;
                }
            }
            await _next(context);
        }

        // Implement the ObtainNewToken method as needed

    }
}
