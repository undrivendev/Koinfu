using System;
using System.Threading.Tasks;

namespace Mds.Koinfu.BLL.Bitstamp
{
    /// <summary>
    /// convert the exchange currency pairs and symbols to internal representations
    /// </summary>
	public class BitstampCurrencyPairConverter
    {
        private readonly ICurrencyAliasRepository _currencyAliasRepository;
        private readonly Exchange _exchange;

        public BitstampCurrencyPairConverter(ICurrencyAliasRepository currencyAliasRepository, Exchange exchange)
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
            var currencies = a.Name.Split('/');
            string finalBase = currencies[0];
            string finalQuote = currencies[1];
            var baseAlias = await _currencyAliasRepository.GetByExchangeAndAlias(_exchange, finalBase);
            var quoteAlias = await _currencyAliasRepository.GetByExchangeAndAlias(_exchange, finalQuote);

            baseAlias.MatchSome(ca => finalBase = ca.Currency.Symbol);
            quoteAlias.MatchSome(ca => finalQuote = ca.Currency.Symbol);

            return this._exchange.ReversedCurrencyPairs ? new CurrencyPair(finalQuote, finalBase) : new CurrencyPair(finalBase, finalQuote);
		}

        public async Task<string> ConvertToExchangeRepresentation(CurrencyPair a)
        {
            var finalBase = a.BaseCurrency;
            var finalQuote = a.CounterCurrency;
            var baseAlias = await _currencyAliasRepository.GetByExchangeAndCurrency(_exchange, finalBase);
            var quoteAlias = await _currencyAliasRepository.GetByExchangeAndCurrency(_exchange, finalQuote);

            baseAlias.MatchSome(ca => finalBase = new Currency(ca.Alias));
            quoteAlias.MatchSome(ca => finalQuote = new Currency(ca.Alias));

            return (this._exchange.ReversedCurrencyPairs ? new CurrencyPair(finalQuote, finalBase) : new CurrencyPair(finalBase, finalQuote)).ToString("").ToLower();
        }
    }
}