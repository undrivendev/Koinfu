using System;
using System.Linq;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive.Subjects;
using Mds.Common.Logging;

namespace Mds.Koinfu.BLL
{
    /// <summary>
    /// Gets the ticks from the observable and use them in the repository. 
    /// Manage the status of the tick
    /// </summary>
    public class CurrencyPairPersistenceService
    {
        //i need this observable to notify everyone about new currency pair inserted in the db
        public ISubject<Tuple<Exchange, IList<CurrencyPair>>> NewCurrencyPairsForExchange { get; protected set; }

        private readonly IObservable<Tuple<Exchange, IEnumerable<CurrencyPair>>> obsProvider;
        private readonly ICurrencyPairRepository currencyPairRepository;
        private readonly ICurrencyRepository currencyRepository;
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

        public CurrencyPairPersistenceService(IObservable<Tuple<Exchange, IEnumerable<CurrencyPair>>> obsProvider, ICurrencyPairRepository currencyPairRepository, ICurrencyRepository currencyRepository, ILogger logger)
        {
            this.obsProvider = obsProvider;
            this.currencyPairRepository = currencyPairRepository;
            this.currencyRepository = currencyRepository;
            this.logger = logger;

            this.NewCurrencyPairsForExchange = new Subject<Tuple<Exchange, IList<CurrencyPair>>>();
        }

        public void Start(CancellationToken token)
        {
            try
            {
                token.Register(() => status = TickStatus.Stopping);

                this.obsProvider
                    .DistinctUntilChanged()
                    .Subscribe(
                        async currencyPairsForExchange =>
                        {
                            await InsertCurrencyPairsForExchangeAsync(currencyPairsForExchange.Item1, currencyPairsForExchange.Item2.Distinct());
                        }
                    ,
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

        private async Task InsertCurrencyPairsForExchangeAsync(Exchange exchange, IEnumerable<CurrencyPair> endpointCurrencyPairs)
        {
            var allCurrencyPairs = await currencyPairRepository.GetAllAsync();
            var allCurrencies = await currencyRepository.GetAllAsync();

            List<CurrencyPair> currencyPairsToAdd = new List<CurrencyPair>();
            List<Currency> currenciesToAdd = new List<Currency>();

            var currencyPairsToAddToExchange = new List<CurrencyPair>();
            var alreadyPersistedCurrencyPairsForExchange = await currencyPairRepository.GetCurrencyPairsForExchangeAsync(exchange);
            foreach (var currentCurrencyPair in endpointCurrencyPairs)
            {
                currentCurrencyPair.Timestamp = DateTime.Now;

                if (!currenciesToAdd.Contains(currentCurrencyPair.BaseCurrency) && !allCurrencies.Contains(currentCurrencyPair.BaseCurrency))
                {
                    currenciesToAdd.Add(currentCurrencyPair.BaseCurrency);
                }

                if (!currenciesToAdd.Contains(currentCurrencyPair.CounterCurrency) && !allCurrencies.Contains(currentCurrencyPair.CounterCurrency))
                {
                    currenciesToAdd.Add(currentCurrencyPair.CounterCurrency);
                }

                if (!currencyPairsToAdd.Contains(currentCurrencyPair) && !allCurrencyPairs.Contains(currentCurrencyPair))
                {
                    currencyPairsToAdd.Add(currentCurrencyPair);
                }

                if (!currencyPairsToAddToExchange.Contains(currentCurrencyPair) && !alreadyPersistedCurrencyPairsForExchange.Contains(currentCurrencyPair))
                {
                    currencyPairsToAddToExchange.Add(currentCurrencyPair);
                }
            }

            //first let's add all the new currencies
            if (currenciesToAdd.Count > 0)
            {
                await currencyRepository.InsertManyAsync(currenciesToAdd);
            }

            //second let's add all the new currency pairs
            if (currencyPairsToAdd.Count > 0)
            {
                await currencyPairRepository.InsertManyAsync(currencyPairsToAdd);
                this.NewCurrencyPairsForExchange.OnNext(new Tuple<Exchange, IList<CurrencyPair>>(exchange, currencyPairsToAdd));
            }
            //third let's add all the currency pairs that were not associated with the exchange before
            if (currencyPairsToAddToExchange.Count > 0)
            {
                await currencyPairRepository.InsertCurrencyPairsForExchangeAsync(exchange, currencyPairsToAddToExchange);
            }
        }

    }
}
