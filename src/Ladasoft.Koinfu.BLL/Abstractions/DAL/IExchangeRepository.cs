using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ladasoft.Koinfu.BLL
{
    public interface IExchangeRepository : IBaseRepository<Exchange>
    {
        Task<int> GetIdByNameAsync(string name);
    }
}

