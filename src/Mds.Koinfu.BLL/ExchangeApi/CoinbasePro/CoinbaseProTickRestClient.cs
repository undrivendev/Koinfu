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
    public class CoinbaseProTickRestClient : BaseRestClient<TickDto>, ITickRestClient
    {
        private readonly Exchange exchange;
        private readonly CurrencyPair currencyPair;

        public CoinbaseProTickRestClient(ILogger logger, IHttpClient httpclient, Exchange exchange, CurrencyPair currencyPair)
          : base(logger, httpclient)
        {
            this.exchange = exchange ?? throw new ArgumentNullException(nameof(exchange));
            this.currencyPair = currencyPair ?? throw new ArgumentNullException(nameof(currencyPair));
        }

        protected override Dictionary<string, string> AddToDefaultHeaders()
            => new Dictionary<string, string>() { { @"User-Agent", @"Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)" } };


        public async Task<Option<Tick>> GetTickAsync(CancellationToken token)
        {
            Option<TickDto> deserializedTick = (await GetDeserializedDto(token,
                Services.Http.HttpMethod.Get,
                Helper.CombineUrlsAsStrings(exchange.RestEndpoint,
                $"/products/{currencyPair}/ticker")));

            return deserializedTick.Map(t =>
                                        new Tick(
                                            exchange,
                                            currencyPair,
                                            t.bid,
                                            t.ask,
                                            DateTime.UtcNow
                                           )
            );
        }
    }
}
