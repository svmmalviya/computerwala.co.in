using DBService.APIModels;
using DBService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBService.Interfaces
{
    public interface ICWSubscription : ICWVisiter
    {
        public Task<ApiResponse> Subscribe(APICWSubscription subscription);
        public Task<ApiResponse> Visiter(string ipaddress);
        public Task<ApiResponse> EmailExists(string email);


        public async Task<ApiResponse> SaveVisiter(string ipaddress)
        {
            ApiResponse response = new ApiResponse();

            return response;
        }

        public async Task<ApiResponse> GetVisiterCount()
        {
            ApiResponse response = new ApiResponse();
            
            return response;
        }
    }
}
