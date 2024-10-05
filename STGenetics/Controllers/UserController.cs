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
            if (!UserValid(userInfo, out string userName, out string userId))
            {
                return new
                {
                    success = false,
                    message = "Incorrect credentials",
                    result = ""
                };
            }

            JwtSecurityToken token = JwtAuthentication(userName, userId);

            return new
            {
                success = true,
                message = "Login Successed",
                result = new JwtSecurityTokenHandler().WriteToken(token)
            };

        }


        private bool UserValid(Object userInfo, out string userName, out string userId)
        {
            userName = string.Empty;
            userId = string.Empty;

            if (userInfo == null)
                return false;

            string info = userInfo.ToString() ?? string.Empty;
            try
            {
                var data = JsonConvert.DeserializeObject<dynamic>(info);
                if (data == null)
                    return false;
                string name = data.userName.ToString();
                string pass = data.password.ToString();

                var user = userRepository.GetUserAsync(name, pass);
                if (user.Result == null)
                    return false;

                userName = user.Result.UserName ?? string.Empty;
                userId = user.Result.UserId.ToString();

                return true;

            }
            catch (Exception) { return false; }

        }

        private JwtSecurityToken JwtAuthentication(string userName, string userId)
        {
            var jwt = configuration.GetSection("Jwt").Get<Jwt>();

            string subject = jwt.Subject ?? string.Empty;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("UserId", userId),
                new Claim("UserName", userName)
            };

            string jwtKey = jwt.Key ?? string.Empty;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var singIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                jwt.Issuer,
                jwt.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: singIn
                );

            return token;

        }
    }
}
