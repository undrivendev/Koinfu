using System;
using System.Collections.Generic;
using System.Text;

namespace Ladasoft.Koinfu.BLL.OpenExchangeRates
{
    public class OpenExchangeRatesResponse
    {
        public string Disclaimer { get; set; }
        public string License { get; set; }
        public long Timestamp { get; set; }
        public string Base { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
