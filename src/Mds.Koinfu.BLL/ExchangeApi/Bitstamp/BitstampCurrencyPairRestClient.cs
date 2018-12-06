using Polly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Optional;
using Mds.Koinfu.BLL.Services;
using Mds.Common.Logging;
using Mds.Common.Http;

namespace Mds.Koinfu.BLL.Bitstamp
{
    public class BitstampCurrencyPairRestClient : BaseRestClient<IEnumerable<CurrencyPairDto>>, ICurrencyPairRestClient
    {
        private readonly Exchange exchange;
        private readonly BitstampCurrencyPairConverter converter;

        public BitstampCurrencyPairRestClient(
            ILogger logger, 
            IHttpClient httpClient, 
            Exchange exchange, 
            BitstampCurrencyPairConverter converter
            )
            : base(logger, httpClient)
        {
            this.exchange = exchange ?? throw new ArgumentNullException(nameof(exchange));
            this.converter = converter ?? throw new ArgumentNullException(nameof(exchange));
        }

        public async Task<Option<Tuple<Exchange, IEnumerable<CurrencyPair>>>> GetCurrencyPairsAsync(CancellationToken token)
        {
            Option<IEnumerable<CurrencyPairDto>> deserializedResponse = (await GetDeserializedDto(token,
               Helper.CombineUrlsAsStrings(this.exchange.RestEndpoint, "/api/v2/trading-pairs-info")));

            return deserializedResponse.Map(r =>
            new Tuple<Exchange, IEnumerable<CurrencyPair>>(this.exchange,
                                                           Task.WhenAll(r.Select(dto => converter.ConvertFromExchangeRepresentation(dto))).Result //TODO: convert in async
                                                          ));

        }
    }
}
