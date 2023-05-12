using STGenetics.Models;

namespace STGenetics.Repository
{
    public interface IUserRepository
    {
        Task<User?> GetUserAsync(string userName, string password);
        Task<User?> GetUserByIdAsync(int userId);
    }
}
