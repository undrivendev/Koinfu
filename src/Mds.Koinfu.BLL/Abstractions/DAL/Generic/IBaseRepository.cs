using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mds.Koinfu.BLL
{
    public interface IBaseRepository<T> where T : IdEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetAsync(int id);
        Task InsertAsync(T item);
        Task InsertManyAsync(IEnumerable<T> items);
        Task UpdateAsync(T item);
        Task DeleteAsync(T item);

    }
}
