using System;
using System.Net.Http;
using Mtd.Koinfu.BLL.Services.Logging;

namespace Mtd.Koinfu.BLL.Services.Http
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
