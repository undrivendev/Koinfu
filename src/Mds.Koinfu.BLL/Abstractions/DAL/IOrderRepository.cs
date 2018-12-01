using System;
using System.Threading.Tasks;

namespace Mds.Koinfu.BLL
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task InsertOrUpdateAsync(Order order);
        Task<Order> GetByExternalGuid(Guid externalGuid);
    }
}
