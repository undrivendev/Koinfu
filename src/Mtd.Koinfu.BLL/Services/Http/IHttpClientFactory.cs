using System;
namespace Mtd.Koinfu.BLL.Services.Http
{
    public interface IHttpClientFactory
    {
        IHttpClient CreateHttpClient();
    }
}
