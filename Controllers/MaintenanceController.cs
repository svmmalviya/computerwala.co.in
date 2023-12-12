using DBService.APIModels;
using DBService.Interfaces;
using DBService.Repositories;
using Microsoft.AspNetCore.Mvc;
using computerwala.Models;
using Microsoft.Extensions.FileProviders;
using System.Text;
using System.IO;
using computerwala.Base;

namespace computerwala.Controllers
{
	public class MaintenanceController : BaseController
	{
		private readonly ICWSubscription cWSubscription;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IWebHostEnvironment _environment;

		private static string Message = "";

		public MaintenanceController(ICWSubscription cWSubscription, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment environment)
		{
			this.cWSubscription = cWSubscription;
			this._httpContextAccessor = httpContextAccessor;
			this._environment = environment;
		}



		[HttpGet]
		public IActionResult Index()
		{
			ViewBag.Message = Message;
			Message = "";
			return View();
		}

		[HttpGet]
		public IActionResult GetTextFile(string name = "")
		{
			// Specify the relative path to the text file within the wwwroot folder

			string relativePath = "logs\\" + "log" + DateTime.Now.ToString("yyyyMMdd") + ".txt"; // Adjust the path as needed
			
			if (string.IsNullOrWhiteSpace(name))
				relativePath = Path.Combine(_environment.ContentRootPath, relativePath);
			else
				relativePath = Path.Combine(_environment.ContentRootPath, "logs\\" + name); // Adjust the path as needed

			// Get the file provider for the wwwroot folder
			IFileProvider fileProvider = _environment.WebRootFileProvider;

			// Combine the base path with the relative path to get the full file path
			string filePath = Path.Combine("logs/" + "log" + DateTime.Now.ToString("yyyyMMdd"));
			byte[] buffer = new byte[10000000]; // Create a buffer to read bytes into
			int bytesRead;
			// Check if the file exists
			if (System.IO.File.Exists(relativePath))
			{
				// Read the text content of the file

					
						string textContent = System.IO.File.ReadAllText(relativePath);


						// Convert the text content to bytes with the desired encoding (e.g., UTF-8)
						buffer = Encoding.UTF8.GetBytes(textContent);

						// Set the content type to "text/plain" for a plain text file
						string contentType = "text/plain";

						// Set the file download name (optional)
						string fileName = "log" + DateTime.Now.ToString("yyyyMMdd") + ".txt";

						// Create a FileContentResult to return the text file
						
					
					var result = new FileContentResult(buffer, "text/plain")
					{
						FileDownloadName = name == "" ? DateTime.Now.ToString("yyyyMMdd") + ".txt" : name // Optional: specify the file name for download
					};
					return result;
			}
			else
			{
				// Handle the case where the file does not exist
				return RedirectToAction("Index");
			}
		}

		[HttpPost]
		public async Task<IActionResult> PostIndex(CWViewSubscriptions model)
		{
			if (ModelState.IsValid)
			{
				var subscription = new APICWSubscription
				{
					Email = model.Email,
				};

				var response = await cWSubscription.Subscribe(subscription);

				if (response != null && response.Success)
				{
					Message = response.Message;
					return RedirectToAction("Index");
				}
			}
			return View("Index", model);
		}

		public void SetVisiter()
		{
			var ipaddress = GetClientIP();
			if (!string.IsNullOrEmpty(ipaddress))
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
				requestIP = remoteIpAddress.ToString();
			}

			return requestIP;
		}

	}
}
