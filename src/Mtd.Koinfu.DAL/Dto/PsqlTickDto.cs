using System;
using Dapper.Contrib.Extensions;
using Mtd.Koinfu.BLL;

namespace Mtd.Koinfu.DAL
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
