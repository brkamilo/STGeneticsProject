using STGenetics.Repository;
using System.Security.Claims;

namespace STGenetics.Models
{
    public class Jwt
    {
        public string? Key { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public string? Subject { get; set; }

        //private readonly IConfiguration configuration;
        //private readonly IUserRepository userRepository;
        //public Jwt(IConfiguration config, IUserRepository userRepos)
        //{
        //    configuration = config;
        //    userRepository = userRepos;
        //}

        //public dynamic ValidateToken(ClaimsIdentity identity)
        //{
        //    try
        //    {
        //        if (identity.Claims.Count() == 0)
        //        {
        //            return new
        //            {
        //                success = false,
        //                message = "Invalid Token",
        //                result = ""
        //            };
        //        }

        //        var id = identity.Claims.FirstOrDefault(x => x.Type == "id").Value;

        //        var user = userRepository.GetUserByIdAsync(int.Parse(id));
        //        return new
        //        {
        //            success = true,
        //            message = "Valid Token",
        //            result = user
        //        };

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
    }
}
