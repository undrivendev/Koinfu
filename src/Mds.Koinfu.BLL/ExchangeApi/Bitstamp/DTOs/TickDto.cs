using System;
using System.Collections.Generic;
using System.Text;

namespace Mds.Koinfu.BLL.Bitstamp
{
    public class TickDto
    {
        //        {
        //	"high": "0.11539558",
        //	"last": "0.11437060",
        //	"timestamp": "1531046460",
        //	"bid": "0.11332181",
        //	"vwap": "0.11302433",
        //	"volume": "614.63771646",
        //	"low": "0.10903156",
        //	"ask": "0.11380187",
        //	"open": "0.11413003"
        //}

        public decimal High { get; set; }
        public decimal Last { get; set; }
        public int Timestamp { get; set; }
        public decimal Bid { get; set; }
        public decimal Vwap { get; set; }
        public decimal Volume { get; set; }
        public decimal Low { get; set; }
        public decimal Ask { get; set; }
        public decimal Open { get; set; }
    }
}
