using System;
using System.Collections.Generic;
using System.Text;


/// <summary>
/// {"type":"ticker","sequence":2850313318,"product_id":"BTC-EUR","price":"5277.97000000","open_24h":"5782.24000000","volume_24h":"7342.8538261","low_24h":"5277.97000000","high_24h":"5789.25000000","volume_30d":"81702.7020531","best_bid":"5277.97","best_ask":"5277.98","side":"sell","time":"2017-11-12T20:40:51.342000Z","trade_id":5291296,"last_size":"0.06430000"}
/// </summary>
namespace Ladasoft.Koinfu.BLL.CoinbasePro
{
    class WebSocketDto : BaseDto
    {
        public bool IsValidTicker => Best_bid != null && Best_ask != null && Best_bid != 0 && Best_ask != 0 && Product_id != null;
        //public bool IsValidOrder => ;

        //GENERAL
        public string Type { get; set; }
        public string Side { get; set; }
        public decimal? Price { get; set; }
        public string Product_id { get; set; }
        public string Sequence { get; set; }
        public DateTime? Time { get; set; }
        //TICK
        public decimal? Open_24h { get; set; }
        public decimal? Volume_24h { get; set; }
        public decimal? Low_24h { get; set; }
        public decimal? High_24h { get; set; }
        public decimal? Volume_30d { get; set; }
        public decimal? Best_bid { get; set; } // Server responds seems to answer with null too
        public decimal? Best_ask { get; set; } // Server responds seems to answer with null too
        public string Trade_id { get; set; }
        public decimal Last_size { get; set; }
        //ORDER:type = received
        public string Order_type { get; set; }
        public decimal? Size { get; set; }
        public Guid? Client_oid { get; set; }
        //ORDER:type = open
        public decimal? Remaining_size { get; set; } // also present on done type
        public string User_id { get; set; }
        public Guid? Profile_id { get; set; }
        public Guid? Order_id { get; set; }
        //ORDER:type = done
        public string Reason { get; set; }
        //ORDER:type = activate
        public string Stop_type { get; set; }
        public decimal? Stop_price { get; set; }
    }
}
