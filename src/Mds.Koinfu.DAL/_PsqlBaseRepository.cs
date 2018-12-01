using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Mds.Koinfu.BLL;
using Npgsql;
using System.Linq;

namespace Mds.Koinfu.DAL
{
    public class PsqlBaseRepository<T, TDto> : IBaseRepository<T>
        where T : IdEntity, new()
        where TDto : BasePsqlDto, new()
    {
        protected readonly string connString;
        protected readonly IMapper mapper;

        public PsqlBaseRepository(string connString, IMapper mapper)
        {
            if (String.IsNullOrWhiteSpace(connString))
                throw new ArgumentException("connection string cannot be null or empty");
            
            this.connString = connString;
            this.mapper = mapper;
        }

        public virtual async Task DeleteAsync(T item)
        {
            using (var connection = new NpgsqlConnection(connString))
            {
                await connection.DeleteAsync(mapper.Map<T, TDto>(item));
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            using (var connection = new NpgsqlConnection(connString))
            {
                return (await connection.GetAllAsync<TDto>()).Select(a => mapper.Map<TDto, T>(a)).ToList();
            }
        }

        public virtual async Task<T> GetAsync(int id)
        {
            using (var connection = new NpgsqlConnection(connString))
            {
                return mapper.Map<TDto, T>(await connection.GetAsync<TDto>(id));
            }
        }

        public virtual async Task InsertAsync(T item)
        {
            using (var connection = new NpgsqlConnection(connString))
            {
                await connection.InsertAsync(mapper.Map<T, TDto>(item));
            }
        }

        public virtual async Task InsertManyAsync(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                await InsertAsync(item);
            }
        }

        public virtual async Task UpdateAsync(T item)
        {
            using (var connection = new NpgsqlConnection(connString))
            {
                await connection.UpdateAsync(mapper.Map<T,TDto>(item));
            }
        }
    }
}
