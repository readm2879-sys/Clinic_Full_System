using ClinicDataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SimbleClinic.Controllers
{
    


    [Route("api/[controller]")]
    [ApiController]



    public class AuthController : ControllerBase
    {
        [HttpPost("login")]

        public IActionResult Login([FromBody] LoginRequest request)
        {
            var User  = ClininBusinissLayer.User.GetUserByUserName(request.UserName);

            if (User == null)
                return Unauthorized("Invalid Credintial");


            bool isValidpassword = BCrypt.Net.BCrypt.Verify(request.Password,User.Password);


            if (!isValidpassword)
                return Unauthorized("Invalid Credintial");


            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, User.UserID.ToString()),
                new Claim(ClaimTypes.Email ,User.UserName),
                new Claim(ClaimTypes.Role, User.Role)

            };

            var secretKey = Environment.GetEnvironmentVariable("CLINIC_JWT_SECRET_KEY");

            // التأكد من أن المفتاح ليس فارغاً لتجنب خطأ الـ Null
            if (string.IsNullOrEmpty(secretKey))
            {
                return StatusCode(500, "JWT Secret Key is not configured on the server.");
            }


            var key = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(secretKey));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken(
            issuer: "ClinicApi",
             audience: "ClinicApiUsers",
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds
             );


            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });

        }

    }
}
