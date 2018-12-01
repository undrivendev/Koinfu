using System;
using System.Collections.Generic;
using System.Text;

namespace Mds.Koinfu.BLL.Kraken
{
    public class CurrencyPairDto : BaseDto
    {
        public string Altname { get; set; }
        public string Aclass_base { get; set; }
        public string Base { get; set; }
        public string Aclass_quote { get; set; }
        public string Quote { get; set; }
        public string Lot { get; set; }
        public int Pair_decimals { get; set; }
        public int Lot_decimals { get; set; }
        public int Lot_multiplier { get; set; }
        public int[] Leverage_buy { get; set; }
        public int[] Leverage_sell { get; set; }
        public IEnumerable<string[]>fees { get; set; }
        public IEnumerable<string[]>fees_maker { get; set; }
        public string Fee_volume_currency { get; set; }
        public int Margin_call { get; set; }
        public int Margin_stop { get; set; }
    }
}
