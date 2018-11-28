using Dapper;
using Dapper.Contrib.Extensions;
using Mtd.Koinfu.BLL;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mtd.Koinfu.DAL
{
    public class PsqlCurrencyPairRepository : PsqlBaseRepository<CurrencyPair, PsqlCurrencyPairDto>, ICurrencyPairRepository
    {
        private readonly ICurrencyRepository currencyRepository;

        public PsqlCurrencyPairRepository(string connString, IMapper mapper, ICurrencyRepository currencyRepository)
            : base(connString, mapper)
        {
            this.currencyRepository = currencyRepository;
        }

        public async Task<CurrencyPair> GetByCurrenciesAsync(Currency baseCurrency, Currency counterCurrency)
        {
            using (var connection = new NpgsqlConnection(connString))
            {
                return (await connection.QueryAsync<PsqlCurrencyPairDto, PsqlCurrencyDto, PsqlCurrencyDto, CurrencyPair>($@"
                    SELECT *
                    FROM currencypair AS cp
                    LEFT OUTER JOIN currency AS c1 ON cp.basecurrencyid = c1.id
                    LEFT OUTER JOIN currency AS c2 ON cp.countercurrencyid = c2.id
                    WHERE c1.symbol = @baseCurrency
                    AND c2.symbol = @counterCurrency
                    ",
                    (pair, baseCurr, counterCurr) =>
                    {
                        return new CurrencyPair(
                            pair.id,
                            mapper.Map<PsqlCurrencyDto, Currency>(baseCurr)
                            , mapper.Map<PsqlCurrencyDto, Currency>(counterCurr)
                            , pair.timestamp
                            );
                    },
                    new
                    {
                        baseCurrency = baseCurrency.ToString(),
                        counterCurrency = counterCurrency.ToString()
                    }
                    ))
                    .FirstOrDefault();

            }
        }

        public async Task<int> GetIdAsync(CurrencyPair currencyPair)
            => (await this.GetByCurrenciesAsync(currencyPair.BaseCurrency, currencyPair.CounterCurrency)).Id;

        private async Task InsertLinkToExchange(int exchangeId, int currencyPairId)
        {
            using (var connection = new NpgsqlConnection(connString))
            {
                await connection.ExecuteAsync(@"
                    INSERT INTO public.exchange_currencypair(
	                exchangeid, currencypairid)
                    VALUES(@exchangeid, @currencypairid);
                ", new
                {
                    exchangeid = exchangeId,
                    currencypairid = currencyPairId
                });
            }
        }

        public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsForExchangeAsync(Exchange ex)
        {
            using (var connection = new NpgsqlConnection(connString))
            {
                return (await connection.QueryAsync<PsqlCurrencyPairDto, PsqlCurrencyDto, PsqlCurrencyDto, CurrencyPair>(
                    $@"
                        SELECT cp.*, c1.*, c2.*
                        FROM exchange AS e
                        INNER JOIN exchange_currencypair AS l ON e.id = l.exchangeid
                        INNER JOIN currencypair cp ON l.currencypairid = cp.id
                        INNER JOIN currency c1 ON cp.basecurrencyid = c1.id
                        INNER JOIN currency c2 ON cp.countercurrencyid = c2.id
                        WHERE e.id = @exchangeId
                        ",
                    (pair, baseCurr, counterCurr) =>
                    {
                        return new CurrencyPair(
                            pair.id,
                            mapper.Map<PsqlCurrencyDto, Currency>(baseCurr),
                            mapper.Map<PsqlCurrencyDto, Currency>(counterCurr),
                            pair.timestamp
                            );
                    },
                    new { exchangeId = ex.Id }
                    ))
                    .ToList();
            }
        }



        public async Task InsertCurrencyPairsForExchangeAsync(Exchange exchange, IEnumerable<CurrencyPair> currencyPairs)
        {
            foreach (var currentCurrencyPair in currencyPairs)
            {
                var id = await GetIdAsync(currentCurrencyPair);
                if (id != 0) //if currency pair has been saved correctly
                {
                    await InsertLinkToExchange((int)exchange.Id, id);
                }
                else
                {
                    throw new NpgsqlException("Unable to save the currency pair on the db");
                }

            }
        }


        public override async Task InsertManyAsync(IEnumerable<CurrencyPair> items)
        {
            List<PsqlCurrencyPairDto> dtoList = new List<PsqlCurrencyPairDto>();
            foreach (var item in items)
            {
                var currentDto = mapper.Map<CurrencyPair, PsqlCurrencyPairDto>(item);
                currentDto.basecurrencyid = (await currencyRepository.GetIdAsync(item.BaseCurrency)).ValueOr(0);
                currentDto.countercurrencyid = (await currencyRepository.GetIdAsync(item.CounterCurrency)).ValueOr(0);
                dtoList.Add(currentDto);
                using (var connection = new NpgsqlConnection(connString))
                {
                    await connection.InsertAsync(currentDto);
                }
            }

        }


        public override async Task<IEnumerable<CurrencyPair>> GetAllAsync()
        {
            using (var connection = new NpgsqlConnection(connString))
            {
                return (await connection.QueryAsync<PsqlCurrencyPairDto, PsqlCurrencyDto, PsqlCurrencyDto, CurrencyPair>(
                    $@"
                        SELECT cp.*, c1.*, c2.*
                        FROM currencypair cp
                        INNER JOIN currency c1 ON cp.basecurrencyid = c1.id
                        INNER JOIN currency c2 ON cp.countercurrencyid = c2.id;
                        ",
                    (pair, baseCurr, counterCurr) =>
                    {
                        return new CurrencyPair(
                            pair.id,
                            mapper.Map<PsqlCurrencyDto, Currency>(baseCurr),
                            mapper.Map<PsqlCurrencyDto, Currency>(counterCurr),
                            pair.timestamp
                            );
                    }))
                    .ToList();
            }
        }



    }
}

