using Mds.Common.Logging;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mds.Koinfu.BLL.OpenExchangeRates
{
    public class OpenExchangeRatesObservableFactory : IObservableFactory<FiatExchangeRate>
    {
        private readonly OpenExchangeRatesRestClient restClient;
        private readonly int pollingIntervalMilliseconds;
        private readonly ILogger logger;

        public OpenExchangeRatesObservableFactory(OpenExchangeRatesRestClient restClient, int p_pollingIntervalMillisecond, ILogger logger)
        {
            this.restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
            this.pollingIntervalMilliseconds = p_pollingIntervalMillisecond;
            this.logger = logger;
        }

        public IObservable<FiatExchangeRate> GetObservable()
        {
            //the concat is necessary only to make it execute as soon as it's been subscribed to (first run)
            // or i would have to wait all the polling interval to make it execute the first time
            return
                Observable.Concat(
            Observable.Return(1L),
            Observable.Interval(
                TimeSpan.FromMilliseconds(pollingIntervalMilliseconds)
                )).SelectMany(counter => Observable.FromAsync(token => restClient.GetRateAsync(token)))
                .Where(tickOpt => tickOpt.HasValue)
                .Select(tickOpt => tickOpt.ValueOr(new FiatExchangeRate(new CurrencyPair("EUR", "USD"), -1, DateTime.UtcNow)));

        }
    }
}
