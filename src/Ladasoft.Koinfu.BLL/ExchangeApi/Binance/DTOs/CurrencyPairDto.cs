using System;
using System.Collections.Generic;
using System.Text;

namespace Ladasoft.Koinfu.BLL.Binance
{
    public class CurrencyPairDto : BaseDto
    {
        public string Symbol { get; set; }
        public string Status { get; set; }
        public string BaseAsset { get; set; }
        public string QuoteAsset { get; set; }
    }
   
}
