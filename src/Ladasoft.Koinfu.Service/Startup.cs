using Ladasoft.Common.Http;
using Ladasoft.Common.Logging;
using Ladasoft.Koinfu.BLL;
using Ladasoft.Koinfu.BLL.Binance;
using Ladasoft.Koinfu.BLL.Bitstamp;
using Ladasoft.Koinfu.BLL.Bittrex;
using Ladasoft.Koinfu.BLL.CoinbasePro;
using Ladasoft.Koinfu.BLL.Kraken;
using Ladasoft.Koinfu.BLL.OpenExchangeRates;
using Ladasoft.Koinfu.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;

namespace Ladasoft.Koinfu.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            StartApplication();
        }

        public static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Custom method to start the application.
        /// </summary>
        private void StartApplication()
        {
            ILogger logger = new SerilogLoggingProxy(Serilog.Log.Logger);

            var defaultPollIntervalMs = String.IsNullOrWhiteSpace(Configuration["PollIntervalInMs"]) ? 5000 : Int32.Parse(Configuration["PollIntervalInMs"]);
            string mailrecipent = "lucadv91@gmail.com";

            ///HTTP CLIENTS
            var httpClient = new ResilientHttpClientFactory(logger, new StandardHttpClientFactory(logger)).CreateHttpClient();

            //REPOS
            AutomapperWrapper.InitializeConfiguration();
            var mapper = new AutomapperWrapper();
            var connString = Configuration.GetConnectionString("Psql");
            var currencyRepo = new PsqlCurrencyRepository(connString, mapper);
            var currencyPairRepo = new PsqlCurrencyPairRepository(connString, mapper, currencyRepo);
            var exchangeRepo = new PsqlExchangeRepository(connString, mapper);
            var aliasRepo = new PsqlCurrencyAliasRepository(connString, mapper);
            bool firstRun = (currencyPairRepo.GetAllAsync().Result).Count() > 0;

            //EXCHANGES
            var exchanges = exchangeRepo.GetAllAsync().Result;
            var coinbaseproExchange = exchanges.Where(a => a.Name == "coinbasepro").FirstOrDefault();
            var krakenExchange = exchanges.Where(a => a.Name == "kraken").FirstOrDefault();
            var binanceExchange = exchanges.Where(a => a.Name == "binance").FirstOrDefault();
            var bittrexExchange = exchanges.Where(a => a.Name == "bittrex").FirstOrDefault();
            var bitstampExchange = exchanges.Where(a => a.Name == "bitstamp").FirstOrDefault();

            //SERVICES
            IEmailValidator emailValidator = new RegexEmailValidator();

            var mailsender = new MailSender(logger, emailValidator,
               Configuration[AppEnvironmentVariables.Mail.SERVER],
               Int32.Parse(Configuration[AppEnvironmentVariables.Mail.PORT]),
               Configuration[AppEnvironmentVariables.Mail.USERNAME],
               Configuration[AppEnvironmentVariables.Mail.PASSWORD]);


            //AUTH
            var cbAuth = new CoinbaseProAuthentication(
            coinbaseproExchange,
            Configuration[AppEnvironmentVariables.Api.CoinbasePro.KEY],
            Configuration[AppEnvironmentVariables.Api.CoinbasePro.PASSPHRASE],
            Configuration[AppEnvironmentVariables.Api.CoinbasePro.SECRET],
            logger);

            /// CURRENCY PAIRS
            var cbproCurrencyPairObservable = new CurrencyPairRestObservableFactory(
                new CoinbaseProCurrencyPairRestClient(logger, httpClient, coinbaseproExchange), 21600000).GetObservable();
            var krakenCurrencyPairObservable = new CurrencyPairRestObservableFactory(
                new KrakenCurrencyPairRestClient(logger, httpClient, krakenExchange,
                                                 new KrakenCurrencyPairConverter(aliasRepo, krakenExchange)),
                   21600000).GetObservable();
            var bittrexCurrencyPairObservable = new CurrencyPairRestObservableFactory(
                new BittrexCurrencyPairRestClient(logger, httpClient, bittrexExchange),
                  21600000).GetObservable();
            var binanceCurrencyPairObservable = new CurrencyPairRestObservableFactory(
                new BinanceCurrencyPairRestClient(logger, httpClient, binanceExchange,
                                                  new BinanceCurrencyPairConverter(aliasRepo, binanceExchange)),
                  21600000).GetObservable();
            var bitstampCurrencyPairObservable = new CurrencyPairRestObservableFactory(
                new BitstampCurrencyPairRestClient(logger, httpClient, bitstampExchange,
                                                  new BitstampCurrencyPairConverter(aliasRepo, bitstampExchange)),
                  21600000).GetObservable();

            // scheduler.currentthread to prevent the observables to return simultaneuosly making conflicts when inserting values at the same time in the db
            var allPairsObservable = krakenCurrencyPairObservable
                .Merge(bittrexCurrencyPairObservable.Delay(new TimeSpan(0, 0, 10)))
                .Merge(cbproCurrencyPairObservable.Delay(new TimeSpan(0, 0, 20)))
                .Merge(binanceCurrencyPairObservable.Delay(new TimeSpan(0, 0, 30)))
                .Merge(bitstampCurrencyPairObservable.Delay(new TimeSpan(0, 0, 35)));

            var currencyPairPersistenceService = new CurrencyPairPersistenceService(
               allPairsObservable,
               currencyPairRepo,
               currencyRepo,
               logger);

            //CURRENCY PAIR ALERTS
            bool notificationResult;
            Boolean.TryParse(Configuration["EnableCurrencyPairMailNotifications"], out notificationResult);
            if (notificationResult)
            {
                new CurrencyPairMailAlertService(currencyPairPersistenceService.NewCurrencyPairsForExchange.AsObservable(), mailsender)
                .StartMailing(cancellationTokenSource.Token, mailrecipent);
            }

            currencyPairPersistenceService.Start(cancellationTokenSource.Token);

            /// FIAT TICK
            //Do not change the currency pair, that's the only supported for the free plan. Updates every hour
            bool fiatTickerEnabled;
            Boolean.TryParse(Configuration["EnableFiatTicker"], out fiatTickerEnabled);
            if (fiatTickerEnabled)
            {
                new FiatExchangeRatePersistenceService(
                new OpenExchangeRatesObservableFactory(
                        new OpenExchangeRatesRestClient(new CurrencyPair("USD", "EUR"), logger, httpClient, Configuration[AppEnvironmentVariables.Api.OpenExchangeRates.KEY]), 3600000, logger)
                    , new PsqlFiatExchangeRateRepository(connString, mapper)
            , logger).Start(cancellationTokenSource.Token);
            }

            //TICKS
            //TODOL need to make sure this starts after the Currency Pair sync

            //this is the dictionary of the currencies we are tracking directly from the appsettings.json
            Dictionary<Exchange, IEnumerable<CurrencyPair>> trackingDictionary = new Dictionary<Exchange, IEnumerable<CurrencyPair>>();
            var exchangesInConf = Configuration.GetSection("Ticks").GetChildren().ToList();
            foreach (var exchange in exchanges)
            {
                var exchangeInConf = exchangesInConf.FirstOrDefault(a => a.Key == exchange.Name);
                if (exchangeInConf != null)
                {
                    var dbCurrencyPairsForExchange = currencyPairRepo.GetCurrencyPairsForExchangeAsync(exchange).Result;
                    var currencyPairsForExchange = new List<CurrencyPair>();
                    foreach (var configuredCurrencyPair in exchangeInConf.Get<string[]>())
                    {

                        var currencyPair = currencyPairRepo.GetByCurrenciesAsync(
                            new Currency(configuredCurrencyPair.Split('-')[0]),
                            new Currency(configuredCurrencyPair.Split('-')[1]))
                            .Result;

                        if (dbCurrencyPairsForExchange.Contains(currencyPair)) //only track if it's a currency pair prensent on the exchange
                        {
                            currencyPairsForExchange.Add(currencyPair); //syncro
                        }
                        else
                        {
                            logger.Log(new LogEntry(LoggingEventType.Warning, $"Discarding pair {currencyPair} from the config, because it's not present in the db"));
                        }
                    }
                    trackingDictionary.Add(exchange, currencyPairsForExchange);
                }
            }

            TickObservableFactory tickObservableFactory = new TickObservableFactory(
                logger,
                httpClient,
                cbAuth,
                new KrakenCurrencyPairConverter(aliasRepo, krakenExchange),
                new BinanceCurrencyPairConverter(aliasRepo, binanceExchange),
                new BitstampCurrencyPairConverter(aliasRepo, bitstampExchange)
                );

            IList<IObservable<Tick>> allTickObservables = new List<IObservable<Tick>>();
            foreach (var currencyPairsForExchange in trackingDictionary)
            {
                foreach (var currencyPair in currencyPairsForExchange.Value)
                {
                    allTickObservables.Add(tickObservableFactory.Create(currencyPairsForExchange.Key, currencyPair, defaultPollIntervalMs));
                }
            }
            var serviceManager = new TickPersistenceServiceStarter(allTickObservables, logger, new PsqlTickRepository(connString, mapper, currencyPairRepo), defaultPollIntervalMs);
            //start all the ticks
            serviceManager.Start(cancellationTokenSource.Token);

        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
