using Mds.Common.Logging;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mds.Koinfu.BLL
{

    /// <summary>
    /// Gets the ticks from the observable and use them in the repository. 
    /// Manage the status of the tick
    /// 
    /// TODO: change name to something more meaningful
    /// </summary>
    public class TickPersistenceService
    {
        private readonly IObservable<Tick> obs;
        private readonly ITickRepository tickRepository;
        private readonly ILogger logger;

        private enum TickStatus
        {
            Running,
            Stopping,
            Stopped
        }

        private TickStatus status = TickStatus.Running;
        public bool Stopped => status == TickStatus.Stopped;
        public bool Stopping => status == TickStatus.Stopping;

        public TickPersistenceService(
            IObservable<Tick> obs, 
            ITickRepository tickRepository, 
            ILogger logger
            )
        {
            this.obs = obs == null ? throw new ArgumentNullException(nameof(obs)) : obs;
            this.tickRepository = tickRepository == null ? throw new ArgumentNullException(nameof(tickRepository)) : tickRepository;
            this.logger = logger == null ? throw new ArgumentNullException(nameof(logger)) : logger;
        }

        public void Start(CancellationToken token)
        {
            try
            {
                token.Register(() => status = TickStatus.Stopping);

                this.obs
                    .DistinctUntilChanged()
                    .Subscribe(
                    async (tick) =>
                    {
                        await tickRepository.InsertAsync(tick);
                        logger.Log($"Inserted {tick.CurrencyPair} tick on {tick.Exchange}");
                    },
                    error =>
                    {
                        logger.Log(error);
                    },
                    () =>
                    {
                        status = TickStatus.Stopped;
                        logger.Log("TickStatus.Stopped");
                    },
                    token);
            }
            catch (Exception e)
            {
                logger.Log(e);
            }
        }
    }
}
