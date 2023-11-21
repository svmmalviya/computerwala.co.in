using Azure;
using computerwala.Base;
using computerwala.DBService.APIModels;
using computerwala.DBService.Models;
using computerwala.Models;
using DBService.APIModels;
using DBService.AppContext;
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
using System.ComponentModel;
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
        private readonly AppDBContext dBContext;
        private static string Message = "";

        public HomeController(ILogger<HomeController> logger, IMemoryCache cache, IAuthentication authentication, ICWSubscription cWSubscription,
            ICWCalender calender, ICWEvent cWEvent, AppDBContext dBContext)
        {
            _logger = logger;
            _cache = cache;
            _authentication = authentication;
            this.cWSubscription = cWSubscription;
            this.cWCalender = calender;
            this.cWEvent = cWEvent;
            dBContext = dBContext;
        }

        [HttpGet]
        public async Task<IActionResult> HappyBirthDayBabu()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> LoginView()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Preferences()
        {
            var preference = new CWTiffinsPreferences();
            var response = await cWEvent.GetTiffinPreferences();
            ViewBag.message = Message;

            if (response.Success)
            {
                preference = JsonConvert.DeserializeObject<CWTiffinsPreferences>(response.Data);
            }

            Message = string.Empty;
            return View(preference);
        }


        [HttpPost]
        public async Task<IActionResult> Preferences(CWTiffinsPreferences preferences)
        {
            var response = await cWEvent.SaveTiffinPreferences(preferences);
            if (response.Success)
            {
                var updatedOrSave = JsonConvert.DeserializeObject<bool>(response.Data);
                Message = updatedOrSave == true ? "Preferences Updated Succesfully." : "Failed To Update Preferences.";
            }
            return RedirectToAction("Preferences");
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

        [HttpGet]
        public async Task<CWCurrentMonth> GetMonthMealDetails()
        {
            var response = await cWCalender.GetCurrentCalender();
            var calender = new CWCurrentMonth();
            CurrentMonthAttendanceDetails currentMonth = new CurrentMonthAttendanceDetails();

            try
            {

                if (response.Success)
                    calender = JsonConvert.DeserializeObject<CWCurrentMonth>(response.Data);

                response = await cWEvent.GetAttendanceDetails(calender.Year, calender.Month.Month);

                if (response.Success)
                {
                    var configurationwithattendance = JsonConvert.DeserializeObject<CWTiffinAttendanceWithConfiguration>(response.Data);
                    foreach (var item in configurationwithattendance.Attendances)
                    {
                        if (item.AttendanceTime.ToLower() == "morning")
                        {
                            if (item.Type.ToLower() == "h")
                                currentMonth.CurrentMonthMorningHalf++;
                            if (item.Type.ToLower() == "f")
                                currentMonth.CurrentMonthMorningFull++;
                        }

                        if (item.AttendanceTime.ToLower() == "evening")
                        {
                            if (item.Type.ToLower() == "h")
                                currentMonth.CurrentMonthEveningHalf++;
                            if (item.Type.ToLower() == "f")
                                currentMonth.CurrentMonthEveningFull++;
                        }

                    }

                    currentMonth.CurrentMonthAmt = (currentMonth.CurrentMonthMorningHalf * configurationwithattendance.Configuration.HalfMealAmount) +
                        (currentMonth.CurrentMonthMorningFull * configurationwithattendance.Configuration.FullMealAmount) +
                        (currentMonth.CurrentMonthEveningHalf * configurationwithattendance.Configuration.HalfMealAmount) +
                        (currentMonth.CurrentMonthEveningFull * configurationwithattendance.Configuration.FullMealAmount);

                }
                calender.AttendanceDetails = currentMonth;

            }
            catch (Exception e)
            {
                throw;
            }

            return calender;
        }

        public string GetCurrentUrl()
        {
            var currentUrl = $"{this.Request.Scheme}://{this.Request.Host}";
            return currentUrl;
        }

        [HttpGet]
        public async Task<IActionResult> CWCalender()
        {
            var calender = await GetMonthMealDetails();
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
                    var configurationwithattendance = JsonConvert.DeserializeObject<CWTiffinAttendanceWithConfiguration>(response.Data);

                    CurrentMonthAttendanceDetails currentMonth = new CurrentMonthAttendanceDetails();


                    foreach (var item in configurationwithattendance.Attendances)
                    {
                        if (item.AttendanceTime.ToLower() == "morning")
                        {
                            if (item.Type.ToLower() == "h")
                                currentMonth.CurrentMonthMorningHalf++;
                            if (item.Type.ToLower() == "f")
                                currentMonth.CurrentMonthMorningFull++;
                        }

                        if (item.AttendanceTime.ToLower() == "evening")
                        {
                            if (item.Type.ToLower() == "h")
                                currentMonth.CurrentMonthEveningHalf++;
                            if (item.Type.ToLower() == "f")
                                currentMonth.CurrentMonthEveningFull++;
                        }

                    }

                    currentMonth.CurrentMonthAmt = (currentMonth.CurrentMonthMorningHalf * configurationwithattendance.Configuration.HalfMealAmount) +
                        (currentMonth.CurrentMonthMorningFull * configurationwithattendance.Configuration.FullMealAmount) +
                        (currentMonth.CurrentMonthEveningHalf * configurationwithattendance.Configuration.HalfMealAmount) +
                        (currentMonth.CurrentMonthEveningFull * configurationwithattendance.Configuration.FullMealAmount);

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
                AttendanceDate = Convert.ToDateTime(Convert.ToDateTime(attendanceTime.date).ToString("yyyy-MM-dd")),
                AttendanceTime = attendanceTime.time,
                CreatedOn = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")),
                HasAttended = true,
                Type = attendanceTime.type

            };
            var response = await cWEvent.SaveEvent(cWAttendance);

            var resp = JsonConvert.DeserializeObject<bool>(response.Data);

            return Json(resp);
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