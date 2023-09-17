using computerwala.Models;
using DBService.APIModels;
using DBService.Interfaces;
using DBService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;

namespace computerwala.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IMemoryCache _cache;
        private IAuthentication _authentication;
		private readonly ICWSubscription cWSubscription;
		private readonly ICWCalender cWCalender;
        private readonly ICWEvent cWEvent;
        public HomeController(ILogger<HomeController> logger, IMemoryCache cache,IAuthentication authentication,ICWSubscription cWSubscription,
            ICWCalender calender,ICWEvent cWEvent)
        {
            _logger = logger;
            _cache = cache;
            _authentication = authentication;
            this.cWSubscription = cWSubscription;
            this.cWCalender = calender;
            this.cWEvent= cWEvent;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            //var token = GetCachedToken();
            //token = await GetToken();

            //RestClient client = new RestClient("https://localhost:7089");
            //RestRequest restRequest = new RestRequest("Authentication/GetWeatherForecast", Method.Get);
            //restRequest.AddHeader("Content-Type", "application/json");

            //restRequest.AddHeader("Authorization", "Bearer " + token);

            //var resp = client.Execute(restRequest);
            //var content = resp.Content;

            return View();
        }

		[AcceptVerbs("Get", "Post")]
		[AllowAnonymous]
		public async Task<IActionResult> IsEmailInUser(string email)
		{
            var user = await this.cWSubscription.EmailExists(email);

			if (user.Success&&!JsonConvert.DeserializeObject<bool>(user.Data))
			{
				return Json(user.Message);
			}
			else
			{
				return Json(true);
			}
		}
		public string GetCurrentUrl()
        {
            var currentUrl = $"{this.Request.Scheme}://{this.Request.Host}";
            return currentUrl;
        }

        public async Task<IActionResult> CWCalender()
        {
            var response = await cWCalender.GetCalender();

            var calender = JsonConvert.DeserializeObject<CWCalender>(response.Data);

            return View("Privacy",calender);
        }

        [HttpPost]
        public async Task<IActionResult> DateClick(AttendanceTime attendanceTime)
        {

            CWAttendance cWAttendance = new CWAttendance {
                Active = false,
                AttendanceDate=DateTime.Now.Date,
                AttendanceTime=attendanceTime.time,
                CreatedOn= DateTime.Now.Date,
                HasAttended=true
            
            };
            var response = await cWEvent.SaveEvent(cWAttendance);

            var calender = JsonConvert.DeserializeObject<ApiResponse>(response.Data);

            return Json("test");
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("Error")]
        [AllowAnonymous]
        [HttpGet]

        public IActionResult Error()
        {
            ViewBag.statuscode = HttpContext.Response.StatusCode;
            //ViewBag.address = (HttpContext.Features.Get<IServerVariablesFeature>()["Remote_Address"]);
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}