using System;
using System.Collections.Generic;
using System.Text;

namespace Mtd.Koinfu.BLL
{
    public class Tick : IdEntity, IEquatable<Tick>
    {
        //TODO set to protected set and avoid using a ridicolous contructor
        public Exchange Exchange { get; protected set; }
        public CurrencyPair CurrencyPair { get; protected set; }
        public decimal BidPrice { get; protected set; }
        public decimal AskPrice { get; protected set; }
        public DateTime Timestamp { get; protected set; }

        public Tick(
            Exchange exchange, 
            CurrencyPair currencyPair, 
            decimal bidPrice, 
            decimal askPrice, 
            DateTime timeStamp)
        {
            this.Exchange = exchange;
            this.CurrencyPair = currencyPair;
            this.BidPrice = bidPrice;
            this.AskPrice = askPrice;
            this.Timestamp = timeStamp;
        }

        public Tick()
        {

        }

        #region Equals
        public static bool operator ==(Tick l, Tick r) => l?.AskPrice == r?.AskPrice && l?.BidPrice == r?.BidPrice && l?.CurrencyPair == r?.CurrencyPair;
        public static bool operator !=(Tick l, Tick r) => l?.AskPrice != r?.AskPrice || l?.BidPrice != r?.BidPrice || l?.CurrencyPair != r?.CurrencyPair;
        public override int GetHashCode() => Exchange.GetHashCode() ^ CurrencyPair.GetHashCode() ^ BidPrice.GetHashCode() ^ AskPrice.GetHashCode();
        public override bool Equals(object obj) => (obj as Tick) == this;
        public bool Equals(Tick other) => this == other;
        #endregion
    }
}
