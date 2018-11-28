using System;
using System.Threading.Tasks;

namespace Mtd.Koinfu.BLL.Kraken
{
    /// <summary>
    /// convert the exchange currency pairs and symbols to internal representations
    /// </summary>
	public class KrakenCurrencyPairConverter
	{
        private readonly ICurrencyAliasRepository _currencyAliasRepository;
        private readonly Exchange _exchange;

        public KrakenCurrencyPairConverter(ICurrencyAliasRepository currencyAliasRepository, Exchange exchange)
        {
            _currencyAliasRepository = currencyAliasRepository;
            _exchange = exchange;
        }

        /// <summary>
        /// 1. check aliases table for the symbol returned by the exhange endpoint
        /// 2. if present use that
        /// 3. if the exchange has reversed currency pairs reverse those also
        /// </summary>
        /// <param name="a">The alpha component.</param>
        public async Task<CurrencyPair> ConvertFromExchangeRepresentation(CurrencyPairDto a)
		{
            string finalBase = RemoveInitialLetterFromCurrencyIfNecessary(a.Base);
            string finalQuote = RemoveInitialLetterFromCurrencyIfNecessary(a.Quote);
            var baseAlias = await _currencyAliasRepository.GetByExchangeAndAlias(_exchange, finalBase);
            var quoteAlias = await _currencyAliasRepository.GetByExchangeAndAlias(_exchange, finalQuote);

            baseAlias.MatchSome(ca => finalBase = ca.Currency.Symbol);
            quoteAlias.MatchSome(ca => finalQuote = ca.Currency.Symbol);

            return this._exchange.ReversedCurrencyPairs ? new CurrencyPair(finalQuote, finalBase) : new CurrencyPair(finalBase, finalQuote);
		}

        private string RemoveInitialLetterFromCurrencyIfNecessary(string currency)
        {
            return currency.Length - 1 >= 3 && (currency.Substring(0, 1) == "X" || currency.Substring(0, 1) == "Z") ? currency.Substring(1) : currency;
        }


        public async Task<string> ConvertToExchangeRepresentation(CurrencyPair a)
        {
            var finalBase = a.BaseCurrency;
            var finalQuote = a.CounterCurrency;
            var baseAlias = await _currencyAliasRepository.GetByExchangeAndCurrency(_exchange, finalBase);
            var quoteAlias = await _currencyAliasRepository.GetByExchangeAndCurrency(_exchange, finalQuote);

            baseAlias.MatchSome(ca => finalBase = new Currency(ca.Alias));
            quoteAlias.MatchSome(ca => finalQuote = new Currency(ca.Alias));

            return (this._exchange.ReversedCurrencyPairs ? new CurrencyPair(finalQuote, finalBase) : new CurrencyPair(finalBase, finalQuote)).ToString("");
        }
    }
}