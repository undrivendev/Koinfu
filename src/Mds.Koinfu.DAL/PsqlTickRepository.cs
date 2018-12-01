using Dapper;
using Mds.Koinfu.BLL;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mds.Koinfu.DAL
{
    public class PsqlTickRepository : PsqlBaseRepository<Tick, PsqlTickDto>, ITickRepository
    {
        private readonly PsqlCurrencyPairRepository currencypairRepository;

     
        public PsqlTickRepository(string connString, IMapper mapper, PsqlCurrencyPairRepository currencypairRepository)
            :base(connString, mapper)
        {
            this.currencypairRepository = currencypairRepository;
        }


        
    }
}

