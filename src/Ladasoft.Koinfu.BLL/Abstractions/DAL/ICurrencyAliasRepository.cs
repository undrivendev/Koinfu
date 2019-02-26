using System;
using System.Threading.Tasks;
using Optional;

namespace Ladasoft.Koinfu.BLL
{
    public interface ICurrencyAliasRepository : IBaseRepository<CurrencyAlias>
    {
        Task<Option<CurrencyAlias>> GetByExchangeAndAlias(Exchange exchange, string alias);
        Task<Option<CurrencyAlias>> GetByExchangeAndCurrency(Exchange exchange, Currency currency);
    }
}
