using System;

namespace Mds.Koinfu.BLL
{
    public class FiatExchangeRate : IdEntity
    {
        public CurrencyPair CurrencyPair { get; protected set; }
        public decimal Rate { get; protected set; }
        public DateTime Timestamp { get; protected set; }

        public FiatExchangeRate()
        {

        }

        public FiatExchangeRate(CurrencyPair currencyPair, decimal rate, DateTime timestamp)
        {
            if (rate == 0) {
                throw new ArgumentException($"{nameof(rate)} cannot be null");
            }

            this.CurrencyPair = currencyPair ?? throw new ArgumentNullException($"parameter {nameof(currencyPair)} cannot be null");
            this.Timestamp = timestamp; 
            this.Rate = rate;
        }
    }
}
