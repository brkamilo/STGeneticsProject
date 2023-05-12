using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using STGenetics.DTOs;
using STGenetics.Models;
using STGenetics.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;

namespace STGenetics.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IUserRepository userRepository;
        public UserController(IConfiguration config, IUserRepository userRepos)
        {
            configuration = config;
            userRepository = userRepos;
        }
        [HttpPost]
        [Route("Login")]
        public dynamic Login([FromBody] Object userInfo)
        {
            string name = string.Empty;
            string pass = string.Empty;
            string info = userInfo.ToString() ?? string.Empty;
            if (userInfo != null)
            {
                var data = JsonConvert.DeserializeObject<dynamic>(info);
                if (data != null)
                {
                    name = data.userName.ToString();
                    pass = data.password.ToString();
                }                
            }
           
            var user = userRepository.GetUserAsync(name, pass);
            if (user.Result == null)
            {
                return new
                {
                    success = false,
                    message = "Incorrect credentials",
                    result = ""
                };
            }

            var jwt = configuration.GetSection("Jwt").Get<Jwt>();

            string subject = jwt.Subject ?? string.Empty;
            string userName = user.Result.UserName ?? string.Empty;
            var claims = new[]
            {

                new Claim(JwtRegisteredClaimNames.Sub, subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("UserId", user.Result.UserId.ToString()),
                new Claim("UserName", userName)
            };
            string jwtKey = jwt.Key ?? string.Empty;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var singIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                jwt.Issuer,
                jwt.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: singIn
                );

            return new {
                success = true,
                message = "Login Successed",
                result = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }       
    }
}
