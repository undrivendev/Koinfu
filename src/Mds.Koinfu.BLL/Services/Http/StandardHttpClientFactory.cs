using System;
using System.Net.Http;
using Mds.Koinfu.BLL.Services.Logging;

namespace Mds.Koinfu.BLL.Services.Http
{
    public class StandardHttpClientFactory : IHttpClientFactory
    {
        private readonly ILogger _logger;

        public StandardHttpClientFactory(ILogger logger)
        {
            _logger = logger;
        }

        public IHttpClient CreateHttpClient()
            => new StandardHttpClient(_logger);
    }
}
