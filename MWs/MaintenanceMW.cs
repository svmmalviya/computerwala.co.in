using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using RestSharp;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http.Extensions;
using DBService.Interfaces;
using System.Runtime.Caching;
using MemoryCache = System.Runtime.Caching.MemoryCache;
using Newtonsoft.Json.Linq;

namespace computerwala.MWs
{

	public class MaintenanceMW
	{
		private readonly RequestDelegate _next;
		private readonly IMemoryCache _cache;
		private readonly IConfiguration _configuration;
		private readonly ILogger<MaintenanceMW> _logger;

		public MaintenanceMW(RequestDelegate next, IConfiguration configuration, ILogger<MaintenanceMW> logger)
		{
			_configuration = configuration;
			this._logger = logger;
			_next = next;
		}

		public MaintenanceMW()
		{

		}

		public async Task InvokeAsync(HttpContext context)
		{
			_logger.LogInformation("In Maintenance MW");
			var maintenanceEnabled = _configuration["Maintenance"];
			_logger.LogInformation("In Maintenance enabled: "+maintenanceEnabled);
			if ( maintenanceEnabled.ToLower() == "enable")
			{
				_logger.LogInformation("Redirecting to maintenance page");
				//context.Response.Redirect("/Maintenance/Index", true);
				context.Request.Path="/Maintenance/Index";
				return;
			}

			await _next(context);
		}

		// Implement the ObtainNewToken method as needed

	}


}
