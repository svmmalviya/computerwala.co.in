using DBService.APIModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBService.Interfaces
{
    public interface IAuthentication
    {
        string GetToken();
        Task<ApiResponse> CheckLogin(string username,string pwd);
    }
}
