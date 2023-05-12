using Newtonsoft.Json.Linq;
using STGenetics.DataAccess;
using STGenetics.DTOs;
using STGenetics.Models;

namespace STGenetics.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ISQLDataAccess _db;

        public OrderRepository(ISQLDataAccess db)
        {
            this._db = db;
        }
        public async Task<int> AddAsync(Order order)
        {           
            try
            {
                var query = "INSERT INTO [Order] VALUES (@Total); SELECT CAST(SCOPE_IDENTITY() AS INT)"; 
                var id = await _db.GetDataByQuery<int, dynamic>(query, new { order.Total });
                return id.FirstOrDefault();
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
