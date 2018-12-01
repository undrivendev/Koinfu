using Mds.Koinfu.BLL.Services.Logging;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mds.Koinfu.BLL.CoinbasePro
{
    /// <summary>
    /// https://docs.pro.coinbase.com/#generating-an-api-key
    /// </summary>
    public class CoinbaseProAuthentication
    {
        private readonly Policy callWithRetry = Policy.Handle<HttpRequestException>().WaitAndRetryAsync(new[]
          {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1)
            });
        private readonly Exchange exchange;
        private readonly string apiKey;
        private readonly string passphrase;
        private readonly string apiSecret;

        private readonly ILogger logger;


        //we need to align to the time of the server or our request will be discarded.
        //I don't want to call the server every time i do a call to incur in latency or other problems.
        //We'll do one request at initialization time and save the datetime of the server relative to ours.
        //
        // Note to Luca : Slippage may occur, need to re-align once in a while
        //
        private TimeSpan serverTimeDifference;


        public CoinbaseProAuthentication(Exchange exchange, string apiKey, string passphrase, string apiSecret, ILogger logger)
        {
            if (String.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentException("invalid apiKey");
            }
            if (String.IsNullOrWhiteSpace(passphrase))
            {
                throw new ArgumentException("invalid passphrase");
            }
            if (String.IsNullOrWhiteSpace(apiSecret))
            {
                throw new ArgumentException("invalid apiSecret");
            }

            this.exchange = exchange ?? throw new ArgumentNullException(nameof(exchange));
            this.apiKey = apiKey;
            this.passphrase = passphrase;
            this.apiSecret = apiSecret;
            this.logger = logger;
        }

        public void SetHeaders(HttpClient client, string relativePath, HttpMethod httpMethod, string body, CancellationToken token = default(CancellationToken))
        {
            foreach (var header in this.GetHeaders(relativePath, httpMethod, body, token))
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        public Dictionary<string, string> GetHeaders(string relativePath, HttpMethod httpMethod, string body, CancellationToken token = default(CancellationToken))
        {
            if (this.serverTimeDifference == default(TimeSpan))
            {
                this.serverTimeDifference = DateTime.UtcNow - GetTimeFromServerAsync(token).Result.Iso;
            }
            var timestamp = GetTimestamp();
            var returnCollection = new Dictionary<string,string>();
            returnCollection.Add("CB-ACCESS-KEY", this.apiKey);
            returnCollection.Add("CB-ACCESS-SIGN", GetAuthSignature(relativePath, httpMethod, timestamp, body));
            returnCollection.Add("CB-ACCESS-TIMESTAMP", timestamp.ToString());
            returnCollection.Add("CB-ACCESS-PASSPHRASE", this.passphrase);

            var finalString = "";
            foreach (var key in returnCollection.Keys)
            {
                finalString += $"{key}: {returnCollection[key]} \r\n";
            }
            
            return returnCollection;
        }

        private string GetAuthSignature(string relativeUri, HttpMethod httpMethod, long timestamp, string body)
        {
            var what = timestamp.ToString() + httpMethod.Method.ToUpper() + relativeUri + body;
            var key = Convert.FromBase64String(this.apiSecret);
            return Convert.ToBase64String(SignWithHmac(Encoding.UTF8.GetBytes(what), key));
        }

        private byte[] SignWithHmac(byte[] dataToSign, byte[] keyBody)
        {
            using (var hmacAlgorithm = new HMACSHA256(keyBody))
            {
                hmacAlgorithm.ComputeHash(dataToSign);
                return hmacAlgorithm.Hash;
            }
        }        

        /// <summary>
        /// timestamp must be in epoch UNIX format
        /// </summary>
        /// <returns></returns>
        private long GetTimestamp()
        {
            var serverTimeStamp = DateTime.UtcNow - this.serverTimeDifference;
            return ((DateTimeOffset)serverTimeStamp).ToUnixTimeSeconds();
        }

        private async Task<TimeResponse> GetTimeFromServerAsync(CancellationToken token = default(CancellationToken))
        {
            var timeUri = new Uri(new Uri(exchange.RestEndpoint), "/time");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
            var request = new HttpRequestMessage(HttpMethod.Get, timeUri);

            HttpResponseMessage response = null;
            try
            {
                response = await callWithRetry.ExecuteAsync(async (t) => response = await client.SendAsync(request, t), token);
            }
            catch (HttpRequestException e)
            {
                logger.Log(e);
            }

            var message = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TimeResponse>(message);
        }
    }
}
