using Newtonsoft.Json.Linq;
using STGenetics.DataAccess;
using STGenetics.Models;

namespace STGenetics.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ISQLDataAccess _db;

        public UserRepository(ISQLDataAccess db)
        {
            this._db = db;
        }
        public async Task<User?> GetUserAsync(string userName, string password)
        {
            var query = "SELECT * FROM [User] WHERE UserName = @UserName AND Password = @Password";
            var user = await _db.GetDataByQuery<User, dynamic>(query, new { userName, password });
            return user.FirstOrDefault();
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            var query = "SELECT * FROM [User] WHERE UserId = @UserId";
            var user = await _db.GetDataByQuery<User, dynamic>(query, new { userId });
            return user.FirstOrDefault();
        }
    }
}
