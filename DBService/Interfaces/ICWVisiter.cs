using Azure;
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
    public interface ICWVisiter 
    {
        public Task<ApiResponse> SaveVisiter(string ipaddress);
        public Task<ApiResponse> GetVisiterCount();

    }
}
