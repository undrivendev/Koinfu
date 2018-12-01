using System;
using System.Collections.Generic;
using System.Text;

namespace Mds.Koinfu.BLL
{
    public class CurrencyPair : IdEntity, IEquatable<CurrencyPair>
    {
        private const string DEFAULT_SEPARATOR = "-";

        public Currency BaseCurrency { get; protected set; }
        public Currency CounterCurrency { get; protected set; }

        protected DateTime _timestamp;
        public DateTime Timestamp 
        { 
            get { return _timestamp; }
            set {
                if (value == default(DateTime))
                {
                    throw new ArgumentException("Invalid Timestamp value");
                }
                _timestamp = value;
            } 
        }

        public CurrencyPair()
        {

        }

        public CurrencyPair(Currency baseCurrency, Currency counterCurrency)
        {
            this.BaseCurrency = baseCurrency ?? throw new ArgumentNullException($"{nameof(baseCurrency)} cannot be null");
            this.CounterCurrency = counterCurrency ?? throw new ArgumentNullException($"{nameof(counterCurrency)} cannot be null");
        }

        public CurrencyPair(Currency baseCurrency, Currency counterCurrency, DateTime timeStamp)
            : this(baseCurrency, counterCurrency)
        {
            this._timestamp = timeStamp == default(DateTime) ? throw new ArgumentNullException($"a value must be provided for parameter {nameof(timeStamp)}") : timeStamp;
        }

        public CurrencyPair(int id, Currency baseCurrency, Currency counterCurrency, DateTime timeStamp)
            : this(baseCurrency, counterCurrency, timeStamp)
        {
            this.Id = id;
        }



        public override string ToString()
         => ToString(DEFAULT_SEPARATOR);
        

        public string ToString(string separator)
            => $"{(BaseCurrency != null ? BaseCurrency.Symbol : String.Empty)}{separator}{(CounterCurrency != null ? CounterCurrency.Symbol : String.Empty)}";
        

        /// <summary>
        /// needed for some stupid exchange that reverses the order of base/quote currency
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        public string ToStringReverse(string separator) 
        {
            return $"{CounterCurrency.Symbol}{separator}{BaseCurrency.Symbol}";
        }

        #region Equals
        public static bool operator ==(CurrencyPair l, CurrencyPair r) => l?.ToString() == r?.ToString();
        public static bool operator !=(CurrencyPair l, CurrencyPair r) => l?.ToString() != r?.ToString();
        public override int GetHashCode() => this.ToString().GetHashCode();
        public override bool Equals(object obj) => (obj as CurrencyPair) == this;
        public bool Equals(CurrencyPair other) => other != null && this.ToString() == other.ToString();
        #endregion

        //convert to a class
        public static implicit operator CurrencyPair(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return null;

            var pair = value.Split(new string[] { DEFAULT_SEPARATOR }, 2, StringSplitOptions.None);
            if (pair.Length != 2)
            { throw new ArgumentException("value format is invalid"); }

            return new CurrencyPair(pair[0], pair[1]);
        }
    }
}
