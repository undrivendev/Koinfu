using System;
using Dapper.Contrib.Extensions;
using Mds.Koinfu.BLL;

namespace Mds.Koinfu.DAL
{
    [Table("tick")]
    public class PsqlTickDto : BasePsqlDto
    {
        public int exchangeid { get; set; }
        public int currencypairid { get; set; }
        public decimal bidprice { get; set; }
        public decimal askprice { get; set; }
        public DateTime timestamp { get; set; }
    }
}
