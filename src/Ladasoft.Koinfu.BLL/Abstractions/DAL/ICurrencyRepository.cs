using Optional;
using System;
using System.Threading.Tasks;

namespace Ladasoft.Koinfu.BLL
{
    public interface ICurrencyRepository : IBaseRepository<Currency>
    {
        Task<Option<int>> GetIdAsync(Currency currency);
    }
}
