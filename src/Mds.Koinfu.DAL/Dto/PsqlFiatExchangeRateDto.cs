using System;
using Dapper.Contrib.Extensions;
using Mds.Koinfu.BLL;

namespace Mds.Koinfu.DAL
{
    [Table("fiatexchangerate")]
    public class PsqlFiatExchangeRateDto : BasePsqlDto
    {
        public string currencypair { get; set; }
        public decimal rate { get; set; }
        public DateTime timestamp { get; set; }
    }
}
