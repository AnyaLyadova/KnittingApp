using KnittingApp.Services;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KnittingApp.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : Controller
    {
        private readonly IUserService userService;
        private readonly IConfiguration _configuration;
        public UserController(IUserService userService, IConfiguration configuration)
        {
            this.userService = userService;
            _configuration = configuration;
        }

        [HttpPost ("register")]
        public async Task<IActionResult> Register(string username, string password)
        {
            await userService.CreateUser(username, password);
            return Ok();
        }

        [HttpGet ("login")]
        public async Task<IActionResult> Login(string username, string password)
        {

            try
            { 
                var user = await userService.GetUserByPassword(username, password);
                if (user == null)
                    return Unauthorized(); // если пользователь не найден, отправляем статусный код 401
                var secretKey = _configuration["Jwt:SecretKey"];
                var issuer = _configuration["Jwt:Issuer"];
                var audience = _configuration["Jwt:Audience"];
                var expirationMinutes = Convert.ToInt32(_configuration["Jwt:AccessTokenExpirationMinutes"]);

                // Создаем ключ
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

                var jwt = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                return Ok(encodedJwt); //возвращаем токен
            }
            catch (ArgumentException)
            {
                return Unauthorized(); // если пользователь не найден, отправляем статусный код 401
            }           
           
        }
        
    }
}
