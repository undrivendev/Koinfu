using System;
using System.Collections.Generic;
using System.Text;

namespace Ladasoft.Koinfu.BLL
{
    public class Currency : IdEntity, IEquatable<Currency>
    {
        public string Symbol { get; protected set; }

        public Currency()
        {

        }

        public Currency(string symbol)
        {
            if (String.IsNullOrWhiteSpace(symbol))
            { throw new ArgumentException("symbol cannot be null"); }

            this.Symbol = symbol;
        }

        public static implicit operator Currency(string value)
        {
            return new Currency(value);
        }

        public override string ToString()
        {
            return this.Symbol;
        }


        #region Equals
        public static bool operator ==(Currency l, Currency r) => l?.ToString() == r?.ToString();
        public static bool operator !=(Currency l, Currency r) => l?.ToString() != r?.ToString();
        public override int GetHashCode() => this.ToString().GetHashCode();
        public override bool Equals(object obj) => (obj as Currency) == this;
        public bool Equals(Currency other) => other != null && this.ToString() == other.ToString();
        #endregion
    }
}
