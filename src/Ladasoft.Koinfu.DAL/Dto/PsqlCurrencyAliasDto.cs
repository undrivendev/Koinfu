using System;
using Dapper.Contrib.Extensions;
using Ladasoft.Koinfu.BLL;

namespace Ladasoft.Koinfu.DAL
{
    [Table("currencyalias")]
    public class PsqlCurrencyAliasDto : BasePsqlDto
    {
        public PsqlCurrencyAliasDto()
        {
            
        }
       
        public string alias { get; set; }
        public int currencyid { get; set; }
        public int exchangeid { get; set; }
    }
}
