using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ladasoft.Koinfu.BLL.Kraken
{
    /// <summary>
    /// https://www.kraken.com/help/api
    /// <pair_name> = pair name
    ///a = ask array(<price>, <whole lot volume>, <lot volume>),
    ///b = bid array(<price>, <whole lot volume>, <lot volume>),
    ///c = last trade closed array(<price>, <lot volume>),
    ///v = volume array(<today>, <last 24 hours>),
    ///p = volume weighted average price array(<today>, <last 24 hours>),
    ///t = number of trades array(<today>, <last 24 hours>),
    ///l = low array(<today>, <last 24 hours>),
    ///h = high array(<today>, <last 24 hours>),
    ///o = today's opening price
    /// </summary>
    public class TickDto : BaseDto
    {
        [JsonProperty(PropertyName = "a")]
        public decimal[] AskArray { get; set; }
        [JsonProperty(PropertyName = "b")]
        public decimal[] BidArray { get; set; }
        [JsonProperty(PropertyName = "c")]
        public decimal[] LastTradeClosedArray { get; set; }
        [JsonProperty(PropertyName = "v")]
        public decimal[] VolumeArray { get; set; }
        [JsonProperty(PropertyName = "p")]
        public decimal[] VolumeWeightedAveragePriceArray { get; set; }
        [JsonProperty(PropertyName = "t")]
        public decimal[] NumberOfTradesArray { get; set; }
        [JsonProperty(PropertyName = "l")]
        public decimal[] LowArray { get; set; }
        [JsonProperty(PropertyName = "h")]
        public decimal[] HighArray { get; set; }
        [JsonProperty(PropertyName = "o")]
        public decimal OpeningPrice { get; set; }
    }
}
