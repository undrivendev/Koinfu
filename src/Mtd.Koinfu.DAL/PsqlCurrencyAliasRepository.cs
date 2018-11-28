using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Mtd.Koinfu.BLL;
using Npgsql;
using Optional;

namespace Mtd.Koinfu.DAL
{
    public class PsqlCurrencyAliasRepository : PsqlBaseRepository<CurrencyAlias, PsqlCurrencyAliasDto>, ICurrencyAliasRepository
    {
        public PsqlCurrencyAliasRepository(string connString, IMapper mapper)
            : base(connString, mapper)
        {
        }

        public async Task<Option<CurrencyAlias>> GetByExchangeAndAlias(Exchange exchange, string alias)
        {
            using (var connection = new NpgsqlConnection(connString)) 
            {

                var result = (await connection.QueryAsync<PsqlCurrencyAliasDto, PsqlCurrencyDto, CurrencyAlias>(
                    @" 
SELECT *  
FROM currencyalias  ca
INNER JOIN currency c ON ca.currencyid = c.id
WHERE exchangeid = @exchangeId 
AND alias = @alias;",
                    (currencyAlias, currency) => 
                    {
                        return new CurrencyAlias(exchange, mapper.Map<PsqlCurrencyDto, Currency>(currency), alias);
                    }, 
                    new { exchangeId = exchange.Id, alias }))
                    .SingleOrDefault();

                return result == null ? Option.None<CurrencyAlias>() : Option.Some<CurrencyAlias>(result);
            } 
        }

        public async Task<Option<CurrencyAlias>> GetByExchangeAndCurrency(Exchange exchange, Currency currency)
        {
            using (var connection = new NpgsqlConnection(connString))
            {

                var result = (await connection.QueryAsync<PsqlCurrencyDto, PsqlCurrencyAliasDto, CurrencyAlias>(
                    @" 
SELECT *  
FROM currency  c
INNER JOIN currencyalias ca 
    ON c.id = ca.currencyid 
    AND ca.exchangeid = @exchangeId
WHERE c.id = @currencyId; ",
                    (currencyDto, currencyAliasDto) =>
                    {
                        return new CurrencyAlias(exchange, mapper.Map<PsqlCurrencyDto, Currency>(currencyDto), currencyAliasDto.alias);
                    },
                    new { exchangeId = exchange.Id, currencyId = currency.Id  }))
                    .SingleOrDefault();

                return result == null ? Option.None<CurrencyAlias>() : Option.Some<CurrencyAlias>(result);
            }
        }
    }
}
