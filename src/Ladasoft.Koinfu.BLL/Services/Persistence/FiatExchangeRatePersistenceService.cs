using Ladasoft.Common.Logging;
using Ladasoft.Koinfu.BLL.OpenExchangeRates;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ladasoft.Koinfu.BLL
{

    /// <summary>
    /// Gets the ticks from the observable and use them in the repository. 
    /// Manage the status of the tick
    /// 
    /// TODO: change name to something more meaningful
    /// </summary>
    public class FiatExchangeRatePersistenceService
    {
        private readonly OpenExchangeRatesObservableFactory obsProvider;
        private readonly IFiatExchangeRateRepository repository;
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

        public FiatExchangeRatePersistenceService(OpenExchangeRatesObservableFactory obsProvider, IFiatExchangeRateRepository repository, ILogger logger)
        {
            this.obsProvider = obsProvider;
            this.repository = repository;
            this.logger = logger;
        }

        public void Start(CancellationToken token)
        {
            try
            {
                token.Register(() => status = TickStatus.Stopping);

                this.obsProvider.GetObservable()
                    .DistinctUntilChanged()
                    .Subscribe(
                    async (fiatRate) =>
                    {
                        await repository.InsertAsync(fiatRate);
                        logger.Log($"{obsProvider.GetType().Name} Tick - {DateTime.Now.ToString("HH:mm:ss.fff")}");
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
