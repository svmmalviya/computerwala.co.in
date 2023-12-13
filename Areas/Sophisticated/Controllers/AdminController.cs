using ComputerWala.DBService.DBService.Interfaces;
using ComputerWala.Model.AdminDashboard;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace computerwala.Areas.Admin.Controllers
{
    [Route("Admin")]
    [Area("Sophisticated")]
    public class AdminController : Controller
    {
        private readonly ICWAdminDashboard _adminDashboard;

        public AdminController(ICWAdminDashboard adminDashboard)
        {
            _adminDashboard = adminDashboard;
        }

        [Route("Dashboard")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            AdminDashboard dashboard = new AdminDashboard();

            try
            {
                dashboard.ListTodayTiffin = await GetTodayTiffin();
            }
            catch (Exception ex)
            {

            }
            return View(dashboard);
        }


        [HttpGet]
        public async Task<IActionResult> TodayTiffin()
        {
            return Json(await GetTodayTiffin());
        }

        private async Task<List<CWTodayTiffin>> GetTodayTiffin() {
            var list = new List<CWTodayTiffin>();
            var response = await _adminDashboard.GetTodayTiffin();
            if (response != null && response.Success)
            {
                list= JsonConvert.DeserializeObject<List<CWTodayTiffin>>(response.Data);
            }
            return list;
        }
    }
}
