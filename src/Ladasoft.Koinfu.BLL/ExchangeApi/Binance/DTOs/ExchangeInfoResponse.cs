﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Ladasoft.Koinfu.BLL.Binance
{
    public class ExchangeInfoResponse : BaseDto
    {
        public IEnumerable<CurrencyPairDto> Symbols { get; set; }
    }
}
