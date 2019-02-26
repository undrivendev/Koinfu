using Optional;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ladasoft.Koinfu.BLL
{
    public interface ICurrencyPairRestClient
    {
        Task<Option<Tuple<Exchange, IEnumerable<CurrencyPair>>>> GetCurrencyPairsAsync(CancellationToken token);
    }
}
