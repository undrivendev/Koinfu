using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mds.Koinfu.BLL.Services.Logging;
using Newtonsoft.Json;
using Optional;

namespace Mds.Koinfu.BLL.Services.Http
{
    public class StandardHttpClient : HttpClient, IHttpClient
    {

        private ILogger _logger;

        public StandardHttpClient(ILogger logger)
        {
            _logger = logger;
        }


        private void AssignDictionaryToRequest(HttpRequestMessage request, Dictionary<string, string> inputHeaders)
        {
            foreach (var header in inputHeaders)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        public Task<HttpResponseMessage> GetAsync(string uri, CancellationToken token, Dictionary<string, string> headers = null)
        {
            var requestMessage = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, uri);
            if (headers != null)
            {
                AssignDictionaryToRequest(requestMessage, headers);
            }
            return SendAsync(requestMessage);
        }


        public Task<HttpResponseMessage> PostAsync<T>(string uri, T item, CancellationToken token, Dictionary<string, string> headers = null)
        {
            var requestMessage = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, uri);
            requestMessage.Content = item != null ? new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json") : null;  //CONTENT-TYPE header 
            if (headers != null)
            {
                AssignDictionaryToRequest(requestMessage, headers);
            }
            return SendAsync(requestMessage);
        }


    }
}
