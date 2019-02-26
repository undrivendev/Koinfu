using System;
using System.Collections.Generic;
using System.Text;

namespace Ladasoft.Koinfu.BLL
{
    public class Order : IdEntity
    {
        public Exchange Exchange { get; protected set; }
        public CurrencyPair CurrencyPair { get; protected set; }
        public Guid ExternalGuid { get; protected set; }
        public decimal Price { get; protected set; }
        public decimal Size { get; protected set; }
        public OrderSide Side { get; protected set; }
        public OrderStatus Status { get; protected set; }
        public OrderType Type { get; protected set; }
        public DateTime Timestamp { get; protected set; }

    }
}
