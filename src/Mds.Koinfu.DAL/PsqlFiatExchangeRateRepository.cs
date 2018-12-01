using Mds.Koinfu.BLL;
using Npgsql;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mds.Koinfu.DAL
{
    public class PsqlFiatExchangeRateRepository : PsqlBaseRepository<FiatExchangeRate, PsqlFiatExchangeRateDto>, IFiatExchangeRateRepository
    {
        public PsqlFiatExchangeRateRepository(string connString, IMapper mapper)
            : base(connString, mapper)
        {
        }

    }
}
