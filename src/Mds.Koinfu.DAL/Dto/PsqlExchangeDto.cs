using System;
using Dapper.Contrib.Extensions;
using Mds.Koinfu.BLL;

namespace Mds.Koinfu.DAL
{
    [Table("exchange")]
    public class PsqlExchangeDto : BasePsqlDto
    {
        public string name { get; set; }
        public string restendpoint { get; set; }
        public string websocketendpoint { get; set; }
        public int pollintervalms { get; set; }
        public bool reversedcurrencypairs { get; set; }
    }
}
