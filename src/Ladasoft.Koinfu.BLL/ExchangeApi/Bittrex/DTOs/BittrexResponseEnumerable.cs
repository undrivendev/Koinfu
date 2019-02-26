using System;
using System.Collections.Generic;
using System.Text;

namespace Ladasoft.Koinfu.BLL.Bittrex
{
    public class BittrexResponseEnumerable<T> where T : BaseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public IEnumerable<T> Result { get; set; }
    }
}
