using Azure;
using computerwala.Models;
using Dapper;
using DBService.APIModels;
using DBService.AppContext;
using DBService.CWConstants;
using DBService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DBService.Repositories
{
    public class CWAuthentication : IAuthentication
    {
        private ApiResponse response;
        private ILogger<CWAuthentication> _logger { get; set; }
        private readonly AppDBContext dbContext;
        private DapperContext _dapperContext { get; set; }


        public CWAuthentication(ILogger<CWAuthentication> logger, DapperContext dapperContext, AppDBContext appDBContext)
        {
            this._logger = logger;
            _dapperContext = dapperContext;
            dbContext = appDBContext;
        }


        public string GetToken()
        {

            Claim[] claims = {
    new Claim(JwtRegisteredClaimNames.Gender, "Male"),
    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
};
            //var seckey = GenerateJwtSecretKey();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("website_computerwala.co.in.indianWebsite"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Set server time zone to India Standard Time (Asia/Kolkata)
            string indiaTimeZoneId = "Asia/Kolkata";
            TimeZoneInfo indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById(indiaTimeZoneId);
            //TimeZoneInfo.Local = indiaTimeZone;

            // Get the current time in the India time zone
            DateTime currentTimeInIndia = TimeZoneInfo.ConvertTime(DateTime.UtcNow, indiaTimeZone);

            var token = new JwtSecurityToken(
                //issuer: "your-issuer",
                //audience: "your-audience",
                claims: claims,
                expires: currentTimeInIndia.AddMinutes(10), // Set token expiration
                signingCredentials: creds
            );
            var validtoken = new JwtSecurityTokenHandler().WriteToken(token);



            return validtoken;
        }

        public async Task<ApiResponse> CheckLogin(string username, string pwd)
        {
            response = new ApiResponse();

            try
            {
                _logger.LogInformation("in subscribe method");

                var query = "select Count(*) from cwlogins where username=@username and password=@pwd";
                using (var connection = _dapperContext.CreateConnection())
                {
                    var count = await connection.QuerySingleOrDefaultAsync<int>(query, new { username, pwd });

                    if (count == 1)
                    {
                        query = "select * from cwlogins where username=@username and password=@pwd";
                        CWUserData user = await connection.QuerySingleOrDefaultAsync<CWUserData>(query, new { username, pwd });

                        response.Success = true;
                        if (user != null)
                        {
                            response.Data = JsonConvert.SerializeObject(user);
                            response.Message = String.Empty;
                        }
                        else
                            response.Data = JsonConvert.SerializeObject(true);
                    }
                    else
                    {
                        response.Success = false;
                        response.Data = JsonConvert.SerializeObject(false);
                        response.Message = String.Empty;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.Message);
                response.Success = false;
                response.Message = e.Message;
                response.Data = null;
            }

            return response;
        }
    }
}