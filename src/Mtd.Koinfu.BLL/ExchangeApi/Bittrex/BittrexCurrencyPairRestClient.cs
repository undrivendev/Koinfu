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
using Mtd.Koinfu.BLL.Services.Http;
using Mtd.Koinfu.BLL.Services.Logging;

namespace Mtd.Koinfu.BLL.Bittrex
{
    public class BittrexCurrencyPairRestClient : BaseRestClient<BittrexResponseEnumerable<CurrencyPairDto>>, ICurrencyPairRestClient
    {
        private readonly Exchange exchange;

        public BittrexCurrencyPairRestClient(ILogger logger, IHttpClient httpClient, Exchange exchange)
            : base(logger, httpClient)
        {
            this.exchange = exchange ?? throw new ArgumentNullException(nameof(exchange));
        }

        public async Task<Option<Tuple<Exchange, IEnumerable<CurrencyPair>>>> GetCurrencyPairsAsync(CancellationToken token)
        {
            Option<BittrexResponseEnumerable<CurrencyPairDto>> deserializedResponse = (await GetDeserializedDto(token,
             Services.Http.HttpMethod.Get,
             Helper.CombineUrlsAsStrings(this.exchange.RestEndpoint, "/api/v1.1/public/getmarkets")));

            return deserializedResponse.Map(o => 
            new Tuple<Exchange, IEnumerable<CurrencyPair>> (
                this.exchange, 
                o.Result.Where(a => a.IsActive).Select(r => 
                new CurrencyPair(
                    new Currency(r.MarketCurrency),   //ATTENTION: order of base/quote currency is reversed in their stupid api!!!
                    new Currency(r.BaseCurrency)
                    ))));

        }
    }
}
