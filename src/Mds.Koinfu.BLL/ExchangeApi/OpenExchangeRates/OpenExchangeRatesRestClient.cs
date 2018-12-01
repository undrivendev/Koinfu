using Mds.Koinfu.BLL.Services.Http;
using Mds.Koinfu.BLL.Services.Logging;
using Optional;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mds.Koinfu.BLL.OpenExchangeRates
{
    /// <summary>
    /// 
    /// </summary>
    public class OpenExchangeRatesRestClient : BaseRestClient<OpenExchangeRatesResponse>
    {
        private readonly string endpoint = "";

        private readonly CurrencyPair currencyPair;
        private readonly string apiSecret;


        public OpenExchangeRatesRestClient(CurrencyPair currencyPair, ILogger logger, IHttpClient httpClient, string apiSecret)
            : base(logger, httpClient)
        {
            if (String.IsNullOrWhiteSpace(apiSecret))
            {
                throw new ArgumentException("invalid apiSecret");
            }
            this.currencyPair = currencyPair ?? throw new ArgumentNullException(nameof(currencyPair));
            this.apiSecret = apiSecret;
            this.endpoint = $@"https://openexchangerates.org/api/latest.json?app_id={this.apiSecret}&base={currencyPair.BaseCurrency.Symbol}";
        }

        public async Task<Option<FiatExchangeRate>> GetRateAsync(CancellationToken token)
        {
            Option<OpenExchangeRatesResponse> deserializedResponse = await GetDeserializedDto(token, Services.Http.HttpMethod.Get, this.endpoint);
            return deserializedResponse.Map(a =>
                new FiatExchangeRate(this.currencyPair,
                a.Rates[this.currencyPair.CounterCurrency.Symbol],
                DateTimeOffset.FromUnixTimeSeconds(a.Timestamp).UtcDateTime)
                );
        }
    }
}
