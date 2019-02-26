using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ladasoft.Koinfu.BLL
{
    public interface ICurrencyPairRepository : IBaseRepository<CurrencyPair>
    {
        Task InsertCurrencyPairsForExchangeAsync(Exchange exchange, IEnumerable<CurrencyPair> currencyPairs);
        Task<IEnumerable<CurrencyPair>> GetCurrencyPairsForExchangeAsync(Exchange exchange);
        Task<CurrencyPair> GetByCurrenciesAsync(Currency baseCurrency, Currency counterCurrency);
    }
}
