using Azure;
using computerwala.Base;
using computerwala.Models;
using DBService.APIModels;
using DBService.Interfaces;
using DBService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
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
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private IMemoryCache _cache;
        private IAuthentication _authentication;
        private readonly ICWSubscription cWSubscription;
        private readonly ICWCalender cWCalender;
        private readonly ICWEvent cWEvent;
        public HomeController(ILogger<HomeController> logger, IMemoryCache cache, IAuthentication authentication, ICWSubscription cWSubscription,
            ICWCalender calender, ICWEvent cWEvent)
        {
            _logger = logger;
            _cache = cache;
            _authentication = authentication;
            this.cWSubscription = cWSubscription;
            this.cWCalender = calender;
            this.cWEvent = cWEvent;
        }

        [HttpGet]
        public async Task<IActionResult> HappyBirthDayBabu()
        {
            return View();
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


        [HttpGet]
        public void SaveVisitor()
        {
            var Ipaddress = GetClientIpAddress();
            cWSubscription.SaveVisiter(Ipaddress);
        }


        [HttpGet]
        public async Task<int> GetVisiter()
        {
            var count = 0;

            var resp = await cWSubscription.GetVisiterCount();

            count = JsonConvert.DeserializeObject<int>(resp.Data);

            return count;
        }



        #region Localization
        [HttpGet]
        public IActionResult ChangeLanguage(string culture)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions() { Expires = DateTimeOffset.UtcNow.AddYears(1) });
            var previewsPage = Request.Headers["Referer"].ToString();

            return Redirect(previewsPage);
        }
        #endregion

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUser(string email)
        {
            var user = await this.cWSubscription.EmailExists(email);

            if (user.Success && !JsonConvert.DeserializeObject<bool>(user.Data))
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
            var response = await cWCalender.GetCurrentCalender();
            var attendance = new List<CWAttendance>();
            CurrentMonthAttendanceDetails currentMonth = new CurrentMonthAttendanceDetails();
            var calender = new CWCurrentMonth();

            if (response.Success)
                calender = JsonConvert.DeserializeObject<CWCurrentMonth>(response.Data);

            response = await cWEvent.GetAttendanceDetails(calender.Year, calender.Month.Month);

            if (response.Success)
                attendance = JsonConvert.DeserializeObject<List<CWAttendance>>(response.Data);


            foreach (var item in attendance)
            {
                if (item.AttendanceTime.ToLower() == "morning")
                    currentMonth.CurrentMonthFirstCnt++;

                if (item.AttendanceTime.ToLower() == "evening")
                    currentMonth.CurrentMonthSecondCnt++;

            }


            currentMonth.CurrentMonthAmt = (currentMonth.CurrentMonthFirstCnt * 50) + (currentMonth.CurrentMonthFirstCnt * 70);

            calender.AttendanceDetails = currentMonth;

            return View("Privacy", calender);
        }

        private async Task<CWCurrentMonth> fillAttendanceDetails(CWCurrentMonth calender)
        {
            var response = new ApiResponse();
            try
            {
                response = await cWEvent.GetAttendanceDetails(calender.Year, calender.Month.Month);

                if (response.Success)
                {
                    var attendance = JsonConvert.DeserializeObject<List<CWAttendance>>(response.Data);

                    CurrentMonthAttendanceDetails currentMonth = new CurrentMonthAttendanceDetails();

                    foreach (var item in attendance)
                    {
                        if (item.AttendanceTime.ToLower() == "morning")
                            currentMonth.CurrentMonthFirstCnt++;

                        if (item.AttendanceTime.ToLower() == "evening")
                            currentMonth.CurrentMonthSecondCnt++;

                    }


                    currentMonth.CurrentMonthAmt = (currentMonth.CurrentMonthFirstCnt * 50) + (currentMonth.CurrentMonthSecondCnt * 70);

                    calender.AttendanceDetails = currentMonth;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return calender;
        }

        [HttpPost]
        public async Task<IActionResult> ChangeCalender(CWCurrentMonth cWCurrent)
        {
            var response = await cWCalender.GetCalenderByMonthYear(cWCurrent.CurrentMonth, cWCurrent.Year);

            var calender = JsonConvert.DeserializeObject<CWCurrentMonth>(response.Data);

            calender = await fillAttendanceDetails(calender);

            return View("Privacy", calender);
        }

        [HttpPost]
        public async Task<IActionResult> NextMonth(CWCurrentMonth cWCurrent)
        {
            cWCurrent.CurrentMonth = cWCurrent.CurrentMonth + 1;

            var response = await cWCalender.GetCalenderByMonthYear(cWCurrent.CurrentMonth, cWCurrent.Year);

            var calender = JsonConvert.DeserializeObject<CWCurrentMonth>(response.Data);
            calender = await fillAttendanceDetails(calender);

            return View("Privacy", calender);
        }

        [HttpPost]
        public async Task<IActionResult> PreviousMonth(CWCurrentMonth cWCurrent)
        {
            cWCurrent.CurrentMonth = cWCurrent.CurrentMonth - 1;

            var response = await cWCalender.GetCalenderByMonthYear(cWCurrent.CurrentMonth, cWCurrent.Year);

            var calender = JsonConvert.DeserializeObject<CWCurrentMonth>(response.Data);
            calender = await fillAttendanceDetails(calender);
            return View("Privacy", calender);
        }


        [HttpPost]
        public async Task<IActionResult> DateClick(AttendanceTime attendanceTime)
        {

            CWAttendance cWAttendance = new CWAttendance
            {
                Active = false,
                AttendanceDate = Convert.ToDateTime(attendanceTime.date),
                AttendanceTime = attendanceTime.time,
                CreatedOn = DateTime.Now.Date,
                HasAttended = true

            };
            var response = await cWEvent.SaveEvent(cWAttendance);

            var calender = JsonConvert.DeserializeObject<ApiResponse>(response.Data);

            return Json(calender);
        }

        [HttpPost]
        public async Task<IActionResult> EventExists(AttendanceTime attendanceTime)
        {
            var calender = new List<CWAttendance>();
            try
            {


                CWAttendance cWAttendance = new CWAttendance
                {
                    Active = false,
                    AttendanceDate = Convert.ToDateTime(attendanceTime.date).Date,
                    AttendanceTime = attendanceTime.time,
                    CreatedOn = DateTime.Now.Date,
                    HasAttended = true

                };
                var response = await cWEvent.EventExists(cWAttendance);

                calender = JsonConvert.DeserializeObject<List<CWAttendance>>(response.Data);
            }
            catch (Exception e)
            {

            }

            return Json(calender);
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