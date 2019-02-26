using Ladasoft.Common.Logging;
using Ladasoft.Koinfu.BLL;
using Ladasoft.Koinfu.BLL.Bittrex;
using Ladasoft.Koinfu.BLL.CoinbasePro;
using Ladasoft.Koinfu.BLL.Kraken;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ladasoft.Koinfu.BLL
{
    public class TickPersistenceServiceStarter
    {
        private readonly ILogger logger;
        private readonly ITickRepository tickRepo;
        private IList<TickPersistenceService> tickServices = new List<TickPersistenceService>();

        // the IConfiguration 
        public TickPersistenceServiceStarter(IList<IObservable<Tick>> tickObservables, ILogger logger,
            ITickRepository tRepository,
            int defaultPollInterval)
        {
            this.logger = logger;
            this.tickRepo = tRepository;

            foreach (var observable in tickObservables)
            {
                tickServices.Add(new TickPersistenceService(observable, tickRepo, logger));
            }
        }

        public void Start(CancellationToken token)
        {
            foreach (var ticker in tickServices)
                ticker.Start(token);
        }

        // TODO: Do something better than that shite
        public bool Stopped => tickServices.All(o => o.Stopped);
        public bool Stopping => tickServices.All(o => o.Stopping);
    }
}
