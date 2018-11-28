using System;
using System.Collections.Generic;
using System.Text;

namespace Mtd.Koinfu.BLL.Bitstamp
{
    public class CurrencyPairDto
    {
        //       {
        //	"base_decimals": 8,
        //	"minimum_order": "5.0 USD",
        //	"name": "LTC/USD",
        //	"counter_decimals": 2,
        //	"trading": "Enabled",
        //	"url_symbol": "ltcusd",
        //	"description": "Litecoin / U.S. dollar"
        //},

        public int Base_Decimals { get; set; }
        public string Minimum_Order { get; set; }
        public string Name { get; set; }
        public int Counter_Decimals { get; set; }
        public string Trading { get; set; }
        public string Url_Symbol { get; set; }
        public string Description { get; set; }
    }
}
