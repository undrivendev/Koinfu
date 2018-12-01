using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Optional;

namespace Mds.Koinfu.BLL.Services.Http
{
    public interface IHttpClient
    {
        Task<HttpResponseMessage> GetAsync(string uri,
                                            CancellationToken token,
                                            Dictionary<string, string> headers = null);

        Task<HttpResponseMessage> PostAsync<T>(string uri,
                                               T item,
                                               CancellationToken token,
                                               Dictionary<string, string> headers = null);
    }
}
