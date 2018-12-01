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
    public class PsqlExchangeRepository : PsqlBaseRepository<Exchange, PsqlExchangeDto>, IExchangeRepository
    {
        public PsqlExchangeRepository(string connString, IMapper mapper)
            : base(connString, mapper)
        {
        }

        public async Task<int> GetIdByNameAsync(string name) 
        { 
            using (var connection = new NpgsqlConnection(connString)) 
            { 
                return (await connection.QuerySingleOrDefaultAsync<int>(@" 
                                SELECT id  
                                FROM exchange  
                                WHERE name = @exchangeName", 
                                new { exchangeName = name })); 
            } 
        } 


    }
}

