using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace computerwala.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private ILogger<AuthenticationController> _logger { get; set; }
        public AuthenticationController(ILogger<AuthenticationController> logger)
        {
            this._logger = logger;
        }

        [HttpGet]
        [Route("GetToken")]
        public ActionResult GetToken()
        {
            _logger.LogInformation("Token processing");
            
            Claim[] claims = {
    new Claim(JwtRegisteredClaimNames.Gender, "Male"),
    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
};

            //var seckey = GenerateJwtSecretKey();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("sdfdslkfjdskljfldsjklfjsldkjflksdjlfjlsdjfljsdlkfjlksdjfljslkdjfldskj"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                //issuer: "your-issuer",
                //audience: "your-audience",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10), // Set token expiration
                signingCredentials: creds
            );
            var validtoken = new JwtSecurityTokenHandler().WriteToken(token);

            _logger.LogInformation($"Generated Token : {validtoken}");

            return Ok(validtoken);
        }
    }
}
