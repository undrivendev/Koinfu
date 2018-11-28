using System;
using System.Collections.Generic;
using System.Text;

namespace Mtd.Koinfu.BLL.Bittrex
{
    public class TickDto : BaseDto
    {
        public decimal Bid { get; set; }
        public decimal Ask { get; set; }
        public decimal Last { get; set; }
    }
}
