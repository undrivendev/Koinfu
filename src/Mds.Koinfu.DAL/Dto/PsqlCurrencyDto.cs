using System;
using Dapper.Contrib.Extensions;
using Mds.Koinfu.BLL;

namespace Mds.Koinfu.DAL
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
