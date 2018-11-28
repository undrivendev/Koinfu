using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mtd.Koinfu.BLL.Services.Logging;
using Polly;

namespace Mtd.Koinfu.BLL.Services.Http
{
    public class ResilientHttpClient : IHttpClient
    {
        private Policy _policy;
        private readonly IHttpClient _innerclient;
        private ILogger _logger;

        public ResilientHttpClient(Policy[] policies,
            ILogger logger,
                                   IHttpClient innerclient)
        {
            _logger = logger;
            // Add Policies to be applied
            _policy =  WrapOrSingleAsync(policies);
            _innerclient = innerclient;
        }

        private Policy WrapOrSingleAsync(Policy[] policies)
        {
            switch (policies.Length)
            {
                case 0:
                    throw new ArgumentException(/* some error message that no policies were supplied */);
                case 1:
                    return policies[0];
                default:
                    return Policy.WrapAsync(policies);
            }
        }

        // Executes the action applying all
        // the policies defined in the wrapper
        private async Task<T> HttpInvoker<T>(Func<Task<T>> action)
            => await _policy.ExecuteAsync(() => action());


        public async Task<HttpResponseMessage> GetAsync(string uri, CancellationToken token, Dictionary<string, string> headers = null)
            => await HttpInvoker(async ()
                                 => await _innerclient.GetAsync(uri, token, headers));

        public async Task<HttpResponseMessage> PostAsync<T>(string uri, T item, CancellationToken token, Dictionary<string, string> headers = null)
        => await HttpInvoker(async ()
                             => await _innerclient.PostAsync<T>(uri, item, token, headers));
    }
}
