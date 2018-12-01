using Polly;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Optional;
using Mds.Koinfu.BLL.Services.Http;
using Mds.Koinfu.BLL.Services.Logging;

namespace Mds.Koinfu.BLL.CoinbasePro
{
    public class CoinbaseProCurrencyPairRestClient : BaseRestClient<IEnumerable<CurrencyPairDto>>, ICurrencyPairRestClient
    {
        private readonly Exchange exchange;

        public CoinbaseProCurrencyPairRestClient(ILogger logger, IHttpClient httpclient, Exchange exchange)
            : base(logger, httpclient)
        {
            this.exchange = exchange ?? throw new ArgumentNullException(nameof(exchange));
        }

        protected override Dictionary<string, string> AddToDefaultHeaders()
            => new Dictionary<string, string>() { { @"User-Agent", @"Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)" } };



        public async Task<Option<Tuple<Exchange, IEnumerable<CurrencyPair>>>> GetCurrencyPairsAsync(CancellationToken token)
        {
            Option<IEnumerable<CurrencyPairDto>> deserializedTick = (await GetDeserializedDto(token,
               Services.Http.HttpMethod.Get,
               Helper.CombineUrlsAsStrings(exchange.RestEndpoint,
               "/products")));

            return deserializedTick.Map(enumerable =>
            new Tuple<Exchange, IEnumerable<CurrencyPair>>(
                this.exchange,
                enumerable.Select(cp => new CurrencyPair(
                    new Currency(cp.Base_currency),
                    new Currency(cp.Quote_currency)
                    ))));
        }
    }
}
