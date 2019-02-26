using System;

namespace Ladasoft.Koinfu.BLL
{
    public class CurrencyAlias : IdEntity
    {
        public Exchange Exchange { get; protected set; }
        public Currency Currency { get; protected set; }
        public string Alias { get; protected set; }

        public CurrencyAlias()
        {

        }

        public CurrencyAlias(Exchange exchange, Currency currency, string alias)
        {
            this.Exchange = exchange ?? throw new ArgumentNullException($"{nameof(exchange)}");
            this.Currency = currency ?? throw new ArgumentNullException($"{nameof(currency)}");
            this.Alias = alias ?? throw new ArgumentNullException($"{nameof(alias)}");
        }

        public override string ToString()
            => Alias;
	}
}