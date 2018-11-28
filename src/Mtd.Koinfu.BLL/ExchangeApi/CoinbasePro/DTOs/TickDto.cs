using System;
using System.Collections.Generic;
using System.Text;

namespace Mtd.Koinfu.BLL.CoinbasePro
{
    public class TickDto : BaseDto
    {
        public int trade_id { get; set; }
        public decimal price { get; set; }
        public decimal size { get; set; }
        public decimal bid { get; set; }
        public decimal ask { get; set; }
        public decimal volume { get; set; }
        public DateTime time { get; set; }
    }
}
