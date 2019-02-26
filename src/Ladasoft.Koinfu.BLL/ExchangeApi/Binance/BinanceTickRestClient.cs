
using Ladasoft.Common.Http;
using Ladasoft.Common.Logging;
using Optional;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ladasoft.Koinfu.BLL.Binance
{
    /// <summary>
    /// https://github.com/binance-exchange/binance-official-api-docs/blob/master/rest-api.md
    /// </summary>
    public class BinanceTickRestClient : BaseRestClient<TickDto>, ITickRestClient
    {
        private readonly Exchange exchange;
        private readonly CurrencyPair currencyPair;
        private readonly BinanceCurrencyPairConverter converter;

        public BinanceTickRestClient(ILogger logger,
            IHttpClient httpClient,
            Exchange exchange,
            CurrencyPair currencyPair,
            BinanceCurrencyPairConverter converter)
            : base(logger, httpClient)
        {
            this.exchange = exchange ?? throw new ArgumentNullException(nameof(exchange));
            this.currencyPair = currencyPair ?? throw new ArgumentNullException(nameof(currencyPair));
            this.converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        public async Task<Option<Tick>> GetTickAsync(CancellationToken token)
        {
            var externalCurrencyPair = await converter.ConvertToExchangeRepresentation(currencyPair);

            Option<TickDto> deserializedResponse = (await GetDeserializedDto(token,
                Helper.CombineUrlsAsStrings(this.exchange.RestEndpoint, $"/api/v3/ticker/bookTicker?symbol={externalCurrencyPair}")));


            return deserializedResponse.Map(r =>
           new Tick(
               exchange,
               currencyPair,
               r.BidPrice,
              r.AskPrice,
               DateTime.UtcNow
               )
            );
        }
    }
}
