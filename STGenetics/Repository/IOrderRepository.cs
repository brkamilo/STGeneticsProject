using STGenetics.DTOs;
using STGenetics.Models;

namespace STGenetics.Repository
{
    public interface IOrderRepository
    {
        Task<int> AddAsync(Order order);
    }
}
