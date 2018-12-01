using Mds.Koinfu.BLL.Services.Http;
using Mds.Koinfu.BLL.Services.Logging;
using Optional;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Mds.Koinfu.BLL.Bittrex
{
    /// <summary>
    /// https://bittrex.com/home/api
    /// </summary>
    public class BittrexTickRestClient : BaseRestClient<BittrexResponse<TickDto>>, ITickRestClient
    {
        private readonly Exchange exchange;
        private readonly CurrencyPair currencyPair;

        public BittrexTickRestClient(ILogger logger, IHttpClient httpClient, Exchange exchange, CurrencyPair currencyPair)
            : base(logger, httpClient)
        {
            this.exchange = exchange ?? throw new ArgumentNullException(nameof(exchange));
            this.currencyPair = currencyPair ?? throw new ArgumentNullException(nameof(currencyPair));
        }

        public async Task<Option<Tick>> GetTickAsync(CancellationToken token)
        {
            Option<BittrexResponse<TickDto>> deserializedResponse = (await GetDeserializedDto(token,
                Services.Http.HttpMethod.Post,  
                Helper.CombineUrlsAsStrings(this.exchange.RestEndpoint, "/api/v1.1/public/getticker"),
                new { market = currencyPair.ToStringReverse("-") })); //ATTENTION: order of base/quote currency is reversed in their stupid api!!!

            return deserializedResponse.Map(r => {
                return new Tick(
                    exchange,
                    currencyPair,
                    r.Result == null ? 0 : r.Result.Bid,
                    r.Result == null ? 0 : r.Result.Ask,
                    DateTime.UtcNow
                );
            });
           
        }

 
    }
}
