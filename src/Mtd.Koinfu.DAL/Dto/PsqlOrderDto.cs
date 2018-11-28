using System;
using Dapper.Contrib.Extensions;
using Mtd.Koinfu.BLL;

namespace Mtd.Koinfu.DAL
{
    [Table("order")]
    public class PsqlOrderDto : BasePsqlDto
    {
        public int exchangeid { get; set; }
        public int currencypairid { get; set; }
        public Guid externalguid { get; set; }
        public decimal price { get; set; }
        public decimal size { get; set; }
        public OrderSide side { get; set; }
        public OrderStatus status { get; set; }
        public OrderType type { get; set; }
        public DateTime timestamp { get; set; }
    }
}
