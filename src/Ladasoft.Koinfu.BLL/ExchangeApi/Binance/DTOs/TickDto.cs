using System;
using System.Collections.Generic;
using System.Text;

namespace Ladasoft.Koinfu.BLL.Binance
{
    public class TickDto
    {
        public string Symbol { get; set; }
        public decimal BidPrice { get; set; }
        public decimal AskPrice { get; set; }
        public decimal BidQty { get; set; }
        public decimal AskQty { get; set; }
    }
}
