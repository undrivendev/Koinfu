﻿
using Mds.Common.Http;
using Mds.Common.Logging;
using Optional;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mds.Koinfu.BLL.Bitstamp
{
    /// <summary>
    /// https://www.bitstamp.net/api/
    /// </summary>
    public class BitstampTickRestClient : BaseRestClient<TickDto>, ITickRestClient
    {
        private readonly Exchange exchange;
        private readonly CurrencyPair currencyPair;
        private readonly BitstampCurrencyPairConverter currencyPairDtoConverter;

        public BitstampTickRestClient(
            ILogger logger,
            IHttpClient httpclient,
            Exchange exchange,
            CurrencyPair currencyPair,
            BitstampCurrencyPairConverter currencyPairDtoConverter
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

            Option<TickDto> deserializedResponse = await GetDeserializedDto(token,
                Helper.CombineUrlsAsStrings(this.exchange.RestEndpoint, $"/api/v2/ticker/{externalCurrencyPair}"));

            return deserializedResponse.Map(r =>
            new Tick(
                exchange,
                currencyPair,
                r.Bid,
                r.Ask,
                DateTime.UtcNow
                )
            );
        }


    }
}

