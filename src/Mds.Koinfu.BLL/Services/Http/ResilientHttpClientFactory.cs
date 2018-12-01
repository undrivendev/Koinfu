using System;
using System.Net.Http;
using Mds.Koinfu.BLL.Services.Logging;
using Polly;

namespace Mds.Koinfu.BLL.Services.Http
{
    public class ResilientHttpClientFactory : IHttpClientFactory
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _decoratedFactory;

        public ResilientHttpClientFactory(ILogger logger, IHttpClientFactory decoratedFactory)
        {
            _logger = logger;
            _decoratedFactory = decoratedFactory;
        }

        public IHttpClient CreateHttpClient()
            => new ResilientHttpClient(CreatePolicies(), _logger, _decoratedFactory.CreateHttpClient());

        // Other code
        private Policy[] CreatePolicies()
            => new Policy[]
            {
        Policy.Handle<HttpRequestException>()
            .WaitAndRetryAsync(
        // number of retries
        3,
        // exponential backoff
        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
        // on retry
        (exception, timeSpan, retryCount, context) =>
        {
            var msg = $@"Retry {retryCount} implemented with Pollys RetryPolicy 
            of {context.PolicyKey} 
            at {context.OperationKey}, 
            due to: {exception}.";
            _logger.Log(new LogEntry(LoggingEventType.Warning, msg));
        })
        };
    }
}
