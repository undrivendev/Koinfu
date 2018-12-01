using System;
using System.Collections.Generic;
using System.Text;

namespace Mds.Koinfu.BLL.Bittrex
{
    public class BittrexResponse<T> : BaseDto where T : BaseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Result { get; set; }
    }
}
