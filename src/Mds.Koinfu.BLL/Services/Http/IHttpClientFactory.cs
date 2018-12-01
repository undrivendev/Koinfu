using System;
namespace Mds.Koinfu.BLL.Services.Http
{
    public interface IHttpClientFactory
    {
        IHttpClient CreateHttpClient();
    }
}
