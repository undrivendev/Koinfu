using System;
using Dapper.Contrib.Extensions;
using Mtd.Koinfu.BLL;

namespace Mtd.Koinfu.DAL
{
    [Table("fiatexchangerate")]
    public class PsqlFiatExchangeRateDto : BasePsqlDto
    {
        public string currencypair { get; set; }
        public decimal rate { get; set; }
        public DateTime timestamp { get; set; }
    }
}
