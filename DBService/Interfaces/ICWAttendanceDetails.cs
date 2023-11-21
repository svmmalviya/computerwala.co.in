using DBService.APIModels;
using DBService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBService.Interfaces
{
    public interface ICWAttendanceDetails
    {
        public Task<ApiResponse> GetAttendanceDetails(int year,int month);
    }
}
