﻿using Ladasoft.Common.Http;
using Ladasoft.Common.Logging;
using Ladasoft.Koinfu.BLL.Binance;
using Ladasoft.Koinfu.BLL.Bitstamp;
using Ladasoft.Koinfu.BLL.Bittrex;
using Ladasoft.Koinfu.BLL.CoinbasePro;
using Ladasoft.Koinfu.BLL.Kraken;
using System;

namespace Ladasoft.Koinfu.BLL
{
    public class TickObservableFactory
    {
        private readonly ILogger _logger;
        private readonly KrakenCurrencyPairConverter krakenCurrencyPairConverter;
        private readonly BinanceCurrencyPairConverter binanceCurrencyPairConverter;
        private readonly BitstampCurrencyPairConverter bitstampCurrencyPairConverter;
        private readonly CoinbaseProAuthentication auth;
        private readonly IHttpClient _httpClient;

        public TickObservableFactory(
            ILogger logger,
            IHttpClient httpClient,
            CoinbaseProAuthentication auth,
            KrakenCurrencyPairConverter krakenCurrencyPairConverter,
            BinanceCurrencyPairConverter binanceCurrencyPairConverter,
            BitstampCurrencyPairConverter bitstampCurrencyPairConverter
            )
        {
            this._logger = logger;
            this.krakenCurrencyPairConverter = krakenCurrencyPairConverter;
            this.binanceCurrencyPairConverter = binanceCurrencyPairConverter;
            this.bitstampCurrencyPairConverter = bitstampCurrencyPairConverter;
            this.auth = auth;
            _httpClient = httpClient;
        }

        public IObservable<Tick> Create(
            Exchange exchange,
            CurrencyPair currencyPair,
            int defaultPollIntervalInMs
            )
        {
            int pollIntervalMs = exchange.PollIntervalMs != 0 ? exchange.PollIntervalMs : defaultPollIntervalInMs;
            switch (exchange.Name)
            {
                case "coinbasepro":
                    return new TickRestClientObservableFactory(new CoinbaseProTickRestClient(_logger, _httpClient, exchange, currencyPair), pollIntervalMs).GetObservable();
                case "kraken":
                    return new TickRestClientObservableFactory(new KrakenTickRestClient(_logger, _httpClient, exchange, currencyPair, krakenCurrencyPairConverter), pollIntervalMs).GetObservable();
                case "bittrex":
                    return new TickRestClientObservableFactory(new BittrexTickRestClient(_logger, _httpClient, exchange, currencyPair), pollIntervalMs).GetObservable();
                case "binance":
                    return new TickRestClientObservableFactory(new BinanceTickRestClient(_logger, _httpClient, exchange, currencyPair, binanceCurrencyPairConverter), pollIntervalMs).GetObservable();
                case "bitstamp":
                    return new TickRestClientObservableFactory(new BitstampTickRestClient(_logger, _httpClient, exchange, currencyPair, bitstampCurrencyPairConverter), pollIntervalMs).GetObservable();
                default:
                    return null;
            }
        }
    }
}
