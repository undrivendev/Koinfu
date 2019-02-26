using System;
using Dapper.Contrib.Extensions;
using Ladasoft.Koinfu.BLL;

namespace Ladasoft.Koinfu.DAL
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
