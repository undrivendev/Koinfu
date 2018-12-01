using Polly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Optional;
using Mds.Koinfu.BLL.Services.Http;
using Mds.Koinfu.BLL.Services.Logging;

namespace Mds.Koinfu.BLL.Kraken
{
    public class KrakenCurrencyPairRestClient : BaseRestClient<KrakenResponse<CurrencyPairDto>>, ICurrencyPairRestClient
    {
        private readonly Exchange exchange;
        private readonly KrakenCurrencyPairConverter converter;

        public KrakenCurrencyPairRestClient(ILogger logger, IHttpClient httpClient, Exchange exchange, KrakenCurrencyPairConverter converter)
            : base(logger, httpClient)
        {
            this.exchange = exchange ?? throw new ArgumentNullException(nameof(exchange));
            this.converter = converter ?? throw new ArgumentNullException(nameof(exchange));
        }

        public async Task<Option<Tuple<Exchange, IEnumerable<CurrencyPair>>>> GetCurrencyPairsAsync(CancellationToken token)
        {
            Option<KrakenResponse<CurrencyPairDto>> deserializedResponse = (await GetDeserializedDto(token,
               Services.Http.HttpMethod.Get,
               Helper.CombineUrlsAsStrings(this.exchange.RestEndpoint, "/public/AssetPairs")));

            return deserializedResponse.Map(r =>
            new Tuple<Exchange, IEnumerable<CurrencyPair>>(this.exchange,
                                                           Task.WhenAll(r.Result.Values.Select(dto => converter.ConvertFromExchangeRepresentation(dto))).Result //TODO: convert in async
                                                          ));

        }
    }
}
