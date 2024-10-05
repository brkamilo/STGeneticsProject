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
       
    }
}
