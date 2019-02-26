using System;
using System.Threading.Tasks;

namespace Ladasoft.Koinfu.BLL
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task InsertOrUpdateAsync(Order order);
        Task<Order> GetByExternalGuid(Guid externalGuid);
    }
}
