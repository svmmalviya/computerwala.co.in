using DBService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DBService.Repositories
{
    public class Authentication : IAuthentication
    {
        private ILogger<Authentication> _logger { get; set; }

        public Authentication(ILogger<Authentication> logger)
        {
            this._logger = logger;
        }

        public Authentication()
        {

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
    }
}