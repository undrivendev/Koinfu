using Dapper;
using Ladasoft.Koinfu.BLL;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ladasoft.Koinfu.DAL
{
    public class PsqlOrderRepository : PsqlBaseRepository<Order, PsqlOrderDto>, IOrderRepository
    {
        private readonly PsqlExchangeRepository exchangeRepository;
        private readonly PsqlCurrencyPairRepository currencyPairRepo;

        public PsqlOrderRepository(string connString, IMapper mapper, PsqlExchangeRepository exchangeRepository, PsqlCurrencyPairRepository currencyPairRepo)
            : base(connString, mapper)
        {
            this.exchangeRepository = exchangeRepository == null ? throw new ArgumentNullException("exchangeRepository cannot be null") : exchangeRepository;
            this.currencyPairRepo = currencyPairRepo == null ? throw new ArgumentNullException("currencyPairRepo cannot be null") : currencyPairRepo;

        }

        public async Task UpdateAsync(int orderId, Order order)
        {
            using (var connection = new NpgsqlConnection(connString))
            {
                await connection.ExecuteAsync(@"
                   UPDATE PUBLIC.""order""
                    SET  exchangeid = @exchangeId
	                    ,currencypairid = @currencyPairId
                        ,externalguid = @externalGuid
	                    ,price = @price
	                    ,""size"" = @size
                        ,side = @side
	                    ,status = @status
                        ,type = @type
	                    ,""timestamp"" = @timestamp
                    WHERE id = @orderId;
                ", new
                {
                    exchangeId = await exchangeRepository.GetIdByNameAsync(order.Exchange.Name),
                    currencyPairId = await currencyPairRepo.GetIdAsync(order.CurrencyPair),
                    externalGuid = order.ExternalGuid,
                    price = order.Price,
                    size = order.Size,
                    side = order.Side.ToString(),
                    status = order.Status.ToString(),
                    type = order.Type.ToString(),
                    timestamp = order.Timestamp,
                    orderId = orderId
                });
            }
        }


        public async Task InsertOrUpdateAsync(Order order)
        {
            var orderId = await GetIdByExternalGuid(order.ExternalGuid);
            if (orderId == 0) //if orderId == 0 it means that is not present on the db
            {
                await InsertAsync(order);
            }
            else
            {
                await UpdateAsync(orderId, order);
            }
        }

        public async Task<int> GetIdByExternalGuid(Guid externalGuid)
        {
            using (var connection = new NpgsqlConnection(connString))
            {
                return await connection.QuerySingleOrDefaultAsync<int>(@"
                   SELECT id FROM PUBLIC.""order""
                    WHERE externalguid = @externalGuid;
                ", new
                {
                     externalGuid
                });
            }
        }

        public async Task<Order> GetByExternalGuid(Guid externalGuid)
        {
            using (var connection = new NpgsqlConnection(connString))
            {
                return await connection.QuerySingleOrDefaultAsync<Order>(@"
                   SELECT * FROM PUBLIC.""order""
                    WHERE externalguid = @externalGuid;
                ", new
                {
                     externalGuid
                });
            }
        }

    }
}
