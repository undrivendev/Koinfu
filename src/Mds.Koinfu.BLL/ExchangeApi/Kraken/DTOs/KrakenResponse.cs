using System;
using System.Collections.Generic;
using System.Text;

namespace Mds.Koinfu.BLL.Kraken
{
    public class KrakenResponse<T> : BaseDto where T : BaseDto
    {
        //the string is a currency pair. Can i force that by using a poco? need to tinker with the serialization properties maybe
        public Dictionary<string, T> Result { get; set; }
        public IEnumerable<string> Error { get; set; }
    }
}
