using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mds.Koinfu.BLL
{
    public class TickRestClientObservableFactory : IObservableFactory<Tick>
    {
        private readonly ITickRestClient restClient;
        private readonly int pollingIntervalMilliseconds;

        public TickRestClientObservableFactory(ITickRestClient tickClient, int pollingIntervalMilliseconds)
        {
            this.restClient = tickClient ?? throw new ArgumentNullException(nameof(tickClient));
            this.pollingIntervalMilliseconds = pollingIntervalMilliseconds;
        }

        public IObservable<Tick> GetObservable()
        {
            return Observable.Interval(TimeSpan.FromMilliseconds(pollingIntervalMilliseconds))
                .SelectMany(counter => Observable.FromAsync(token => restClient.GetTickAsync(token)))
                .Where(tickOpt => tickOpt.HasValue)
                .Select(tickOpt => tickOpt.ValueOr(default(Tick)));
        }
    }
}
