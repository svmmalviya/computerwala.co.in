using Azure;
using computerwala.DBService.Models;
using Dapper;
using DBService.APIModels;
using DBService.AppContext;
using DBService.CWConstants;
using DBService.Models;
using DBService.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBService.Interfaces
{
    public interface ICWEvent : ICWAttendanceDetails
    {
        public Task<ApiResponse> SaveEvent(CWAttendance cWAttendance);
        public Task<ApiResponse> GetTiffinPreferences();
        public Task<ApiResponse> SaveTiffinPreferences(CWTiffinsPreferences cWTiffinsPreferences);
        public Task<ApiResponse> EventExists(CWAttendance cWAttendance);


        public async Task<ApiResponse> GetAttendanceDetails(int year, int month)
        {

            var response = new DBService.APIModels.ApiResponse();
            await Task.Delay(1000); // Delay for 1 second.
            return response;
        }
    }
}
