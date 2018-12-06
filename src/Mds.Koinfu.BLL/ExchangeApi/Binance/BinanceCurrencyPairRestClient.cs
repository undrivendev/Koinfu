using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mds.Common.Http;
using Mds.Common.Logging;
using Optional;

namespace Mds.Koinfu.BLL.Binance
{
    /// <summary>
    /// https://github.com/binance-exchange/binance-official-api-docs/blob/master/rest-api.md
    /// </summary>
    public class BinanceCurrencyPairRestClient : BaseRestClient<ExchangeInfoResponse>, ICurrencyPairRestClient
    {
        private readonly Exchange exchange;
        private readonly BinanceCurrencyPairConverter converter;

        public BinanceCurrencyPairRestClient(
            ILogger logger,
            IHttpClient httpClient,
            Exchange exchange,
            BinanceCurrencyPairConverter converter)
            : base(logger, httpClient)
        {
            this.exchange = exchange ?? throw new ArgumentNullException(nameof(exchange));
            this.converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        public async Task<Option<Tuple<Exchange, IEnumerable<CurrencyPair>>>> GetCurrencyPairsAsync(CancellationToken token)
        {
            Option<ExchangeInfoResponse> deserialized = (await GetDeserializedDto(token,
              Helper.CombineUrlsAsStrings(exchange.RestEndpoint,
              "/api/v1/exchangeInfo")));

            return deserialized.Map(response =>
            new Tuple<Exchange, IEnumerable<CurrencyPair>>(
                this.exchange,
                 Task.WhenAll(
                response.Symbols
                .Where(s => s.Status == "TRADING")
                .Select(cp => this.converter.ConvertFromExchangeRepresentation(cp))).Result //TODO convert in async
                ));
        }

    }
}
