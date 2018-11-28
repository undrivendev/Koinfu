using Mtd.Koinfu.BLL.Services.Http;
using Mtd.Koinfu.BLL.Services.Logging;
using Newtonsoft.Json;
using Optional;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mtd.Koinfu.BLL
{
    /// <summary>
    /// Base class to centralize all the rest calls to the server
    /// </summary>
    /// <typeparam name="T1">type that the respose from the server has to be deserialized to</typeparam>
    public abstract class BaseRestClient<T> 
        where T : class
    {
        protected readonly ILogger logger;
        private readonly IHttpClient _httpclient;

        public BaseRestClient(ILogger logger, IHttpClient httpclient)
        {
            this.logger = logger;
            _httpclient = httpclient;
        }


        /// <summary>
        /// Builds the request headers.
        /// </summary>
        /// <returns>The request headers.</returns>
        private Dictionary<string, string> BuildRequestHeaders()
        {
            var headers = new Dictionary<string, string>() { { @"Accept", @"application/json" } };
            foreach (var h in AddToDefaultHeaders())
            {
                headers.Add(h.Key, h.Value);
            }
            return headers;
        }

        /// <summary>
        /// Extensibility point for derived class to customize the headers. Do an interface with composition?? maybe
        /// </summary>
        /// <returns>The to default headers.</returns>
        protected virtual Dictionary<string, string> AddToDefaultHeaders()
            => new Dictionary<string, string>() { };
        

        protected async Task<Option<T>> GetDeserializedDto(CancellationToken token, Services.Http.HttpMethod method, Uri endpointUrl, object requestContent = null)
        {
            return await GetDeserializedDto(token, method, endpointUrl.ToString(), requestContent);
        }

        protected async Task<Option<T>> GetDeserializedDto(CancellationToken token, Services.Http.HttpMethod method, string endpointUrl, object requestContent = null)
        {
            HttpResponseMessage response = null;
            var headers = BuildRequestHeaders();
            switch (method)
            {
                case Services.Http.HttpMethod.Get:
                    response = await _httpclient.GetAsync(endpointUrl, token, headers);
                    break;
                case Services.Http.HttpMethod.Post:
                    response = await _httpclient.PostAsync(endpointUrl, requestContent, token, headers);
                    break;
                default:
                    break;
            }

            if (!response.IsSuccessStatusCode || response == null)
            {
                logger.Log(new LogEntry(LoggingEventType.Error, $"Error on tick responsev from {endpointUrl}, response={response}. \r\n"));
                logger.Log(new LogEntry(LoggingEventType.Debug, $"Response content: {await response.Content.ReadAsStringAsync()}"));
                return Option.None<T>(); 
            }
            var message = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(message).SomeNotNull();
        }

    }
}
