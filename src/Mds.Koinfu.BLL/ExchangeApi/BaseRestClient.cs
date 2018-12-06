using Mds.Common.Base;
using Mds.Common.Http;
using Mds.Common.Logging;
using Mds.Koinfu.BLL.Services;
using Newtonsoft.Json;
using Optional;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Mds.Koinfu.BLL
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


        protected Task<Option<T>> GetDeserializedDto(CancellationToken token, Uri endpointUrl)
            => GetDeserializedDto(token, endpointUrl.ToString());
        

        protected async Task<Option<T>> GetDeserializedDto(CancellationToken token, string endpointUrl)
        {
            
            var headers = BuildRequestHeaders();
            var result = await _httpclient.GetStringAsync(endpointUrl, headers);
            if (result.Success)
            {
                return JsonConvert.DeserializeObject<T>(result.Data).SomeNotNull();
            }
            else
            {
                return Option.None<T>();
            }    
            
        }

        protected Task<Option<T>> PostDto(CancellationToken token, Uri endpointUrl, object requestContent)
         => PostDto(token, endpointUrl.ToString(), requestContent);

        protected async Task<Option<T>> PostDto(CancellationToken token, string endpointUrl, object requestContent)
        {

            var headers = BuildRequestHeaders();
            var result = await _httpclient.PostWithStringResultAsync(endpointUrl, requestContent, headers);

            if (result.Success)
            {
                return JsonConvert.DeserializeObject<T>(result.Data).SomeNotNull();
            }
            else
            {
                return Option.None<T>();
            }
        }




    }
}
