using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Ladasoft.Koinfu.BLL.Kraken
{
    public class CurrencyPairRestObservableFactory : IObservableFactory<Tuple<Exchange, IEnumerable<CurrencyPair>>>
    {
        private readonly int pollingIntervalMilliseconds;
        private readonly ICurrencyPairRestClient restClient;

        public CurrencyPairRestObservableFactory(ICurrencyPairRestClient restClient, int pollingIntervalMilliseconds)
        {
            this.restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
            this.pollingIntervalMilliseconds = pollingIntervalMilliseconds;
        }

        public IObservable<Tuple<Exchange, IEnumerable<CurrencyPair>>> GetObservable()
        {
            //the concat is necessary only to make it execute as soon as it's been subscribed to (first run)
            // or i would have to wait all the polling interval to make it execute the first time
            return Observable.Concat<long>(Observable.Return(1L),
             Observable.Interval(TimeSpan.FromMilliseconds(pollingIntervalMilliseconds)))
                 .SelectMany(counter => Observable.FromAsync(token => this.restClient.GetCurrencyPairsAsync(token)))
                 .Where(tickOpt => tickOpt.HasValue)
                 .Select(tickOpt => tickOpt.ValueOr(
                     new Tuple<Exchange, IEnumerable<CurrencyPair>>(
                         new Exchange(),
                         new List<CurrencyPair>()
                         )));
        }
    }
}
