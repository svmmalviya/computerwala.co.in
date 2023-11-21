using DBService.APIModels;
using DBService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBService.Interfaces
{
    public interface ICWCalender
	{
       public Task<ApiResponse> GetCalender();
       public Task<ApiResponse> GetCalenderYear(int year);
       public Task<ApiResponse> GetCurrentCalender();
       public Task<ApiResponse> GetCalenderByMonthYear(int month,int year);
        public Task<ApiResponse> SaveCalenderDay();
        public Task<ApiResponse> SaveCalender(List<CWCalender> cWCalender);
    }
}
