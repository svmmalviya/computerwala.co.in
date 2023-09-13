using DBService.APIModels;
using DBService.Interfaces;
using DBService.Repositories;
using Microsoft.AspNetCore.Mvc;
using computerwala.Models;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace computerwala.Controllers
{
	public class MaintenanceController : Controller
	{
		private readonly ICWSubscription cWSubscription;
		private readonly IHttpContextAccessor _httpContextAccessor;


		private static string Message = "";

		public MaintenanceController(ICWSubscription cWSubscription, IHttpContextAccessor httpContextAccessor)
		{
			this.cWSubscription = cWSubscription;
			this._httpContextAccessor = httpContextAccessor;
		}



		[HttpGet]
		public IActionResult Index()
		{
			ViewBag.Message = Message;
			Message = "";
			return View();
		}

		[HttpPost]
		public IActionResult Index(CWSubscriptions model)
		{
			if (ModelState.IsValid)
			{
				var subscription = new APICWSubscription
				{
					Email = model.Email,
				};

				var response = cWSubscription.Subscribe(subscription);

				if (response != null && response.Success)
				{
					Message = response.Message;
					return RedirectToAction("Index");
				}
			}
			return View(model);
		}

		public void SetVisiter()
		{
			var ipaddress = GetClientIP();
			cWSubscription.Visiter(ipaddress);
		}

		public string GetClientIP()
		{
			
			var context = _httpContextAccessor.HttpContext;
			// Option 1: Get IP from HttpContext.Connection
			var connection = _httpContextAccessor.HttpContext.Connection;

			var remoteIpAddress = context.Connection.RemoteIpAddress;

			// Option 2: Get IP from HttpContext.Request.Headers
			var requestIP = "";

			if (remoteIpAddress != null && remoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
			{
				// It's an IPv4 address
				requestIP= remoteIpAddress.ToString();
			}

			return requestIP;
		}

	}
}
