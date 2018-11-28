using System;
using Dapper.Contrib.Extensions;
using Mtd.Koinfu.BLL;

namespace Mtd.Koinfu.DAL
{
    [Table("currency")]
    public class PsqlCurrencyDto : BasePsqlDto
    {
        public PsqlCurrencyDto()
        {

        }

        public string symbol { get; set; }
    }
}
