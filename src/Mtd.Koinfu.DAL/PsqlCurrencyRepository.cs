using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Mtd.Koinfu.BLL;
using Npgsql;
using Optional;

namespace Mtd.Koinfu.DAL
{
    public class PsqlCurrencyRepository : PsqlBaseRepository<Currency, PsqlCurrencyDto>, ICurrencyRepository
    {
        public PsqlCurrencyRepository(string connString, IMapper mapper)
            : base(connString, mapper)
        {
        }

        public async Task<Option<int>> GetIdAsync(Currency currency)
        {
            using (var connection = new NpgsqlConnection(connString))
            {

                var result =  await connection.QuerySingleOrDefaultAsync<int>(
                    @" 
SELECT id  
FROM currency  
WHERE symbol = @symbol
",
                new { symbol = currency.Symbol });

                return result == 0 ? Option.None<int>() : Option.Some(result);

            }
        }
    }
}
