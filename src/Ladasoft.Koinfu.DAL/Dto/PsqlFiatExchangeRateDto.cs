using System;
using Dapper.Contrib.Extensions;
using Ladasoft.Koinfu.BLL;

namespace Ladasoft.Koinfu.DAL
{
    [Table("fiatexchangerate")]
    public class PsqlFiatExchangeRateDto : BasePsqlDto
    {
        public string currencypair { get; set; }
        public decimal rate { get; set; }
        public DateTime timestamp { get; set; }
    }
}
