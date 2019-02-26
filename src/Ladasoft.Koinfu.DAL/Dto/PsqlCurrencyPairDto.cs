using System;
using Dapper.Contrib.Extensions;
using Ladasoft.Koinfu.BLL;

namespace Ladasoft.Koinfu.DAL
{
    [Table("currencypair")]
    public class PsqlCurrencyPairDto : BasePsqlDto
    {
        public int basecurrencyid { get; set; }
        public int countercurrencyid { get; set; }
        public DateTime timestamp { get; set; }

    }
}
