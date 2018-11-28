using Mtd.Koinfu.BLL.Services.Http;
using Mtd.Koinfu.BLL.Services.Logging;
using Optional;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mtd.Koinfu.BLL.Kraken
{
    /// <summary>
    /// https://www.kraken.com/help/api
    /// </summary>
    public class KrakenTickRestClient : BaseRestClient<KrakenResponse<TickDto>>, ITickRestClient
    {
        private readonly Exchange exchange;
        private readonly CurrencyPair currencyPair;
        private readonly KrakenCurrencyPairConverter currencyPairDtoConverter;

        public KrakenTickRestClient(
            ILogger logger, 
            IHttpClient httpclient, 
            Exchange exchange, 
            CurrencyPair currencyPair, 
            KrakenCurrencyPairConverter currencyPairDtoConverter
            )
         : base(logger, httpclient)
        {
            this.exchange = exchange ?? throw new ArgumentNullException(nameof(exchange));
            this.currencyPair = currencyPair ?? throw new ArgumentNullException(nameof(currencyPair));
            this.currencyPairDtoConverter = currencyPairDtoConverter ?? throw new ArgumentNullException(nameof(currencyPairDtoConverter));
        }

        public async Task<Option<Tick>> GetTickAsync(CancellationToken token)
        {
            var externalCurrencyPair = await currencyPairDtoConverter.ConvertToExchangeRepresentation(currencyPair);

            Option<KrakenResponse<TickDto>> deserializedResponse = (await GetDeserializedDto(token,
                Services.Http.HttpMethod.Post,
                Helper.CombineUrlsAsStrings(this.exchange.RestEndpoint, "/public/Ticker"),
                new { pair = externalCurrencyPair }));

            return deserializedResponse.Map(r =>
            new Tick(
                exchange,
                currencyPair,
                r.Result[externalCurrencyPair].BidArray[0],
                r.Result[externalCurrencyPair].AskArray[0],
                DateTime.UtcNow
                )
            );
        }


    }



}

