using System;
using System.Collections.Generic;
using System.Text;

namespace Mtd.Koinfu.BLL.CoinbasePro
{
    public class CurrencyPairDto
    {
        public string Id { get; set; }
        public string Base_currency { get; set; }
        public string Quote_currency { get; set; }
        public decimal Base_min_size { get; set; }
        public decimal Base_max_size { get; set; }
        public decimal Quote_increment { get; set; }
        public string Display_name { get; set; }
        public string Status { get; set; }
        public string Margin_enabled { get; set; }
        public string Status_message { get; set; }
    }
}
