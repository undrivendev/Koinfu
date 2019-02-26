using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using Ladasoft.Common.Logging;

namespace Ladasoft.Koinfu.BLL.CoinbasePro
{
    public class CoinbaseProWebsocketClient : IObservableFactory<object>
    {
        private const string endpoint = @"wss://ws-feed.pro.coinbase.com";
        private const int sendChunkSize = 1024;
        private const int receiveChunkSize = 1024;

        private readonly ClientWebSocket ws;
        private readonly CoinbaseProAuthentication auth;
        private readonly IOrderRepository orderRepository;
        private readonly IEnumerable<CurrencyPair> currencyPairs;
        private readonly Exchange exchange;
        private readonly ILogger logger;
        private IObservable<object> wsObservale;

        public CoinbaseProWebsocketClient(Exchange exchange,
            IEnumerable<CurrencyPair> currencyPairs,
            ILogger logger,
            int defaultPollingIntervalMilliseconds,
            CoinbaseProAuthentication auth,
            IOrderRepository orderRepository)
        {
            this.currencyPairs = currencyPairs ?? throw new ArgumentNullException(nameof(currencyPairs));
            this.exchange = exchange ?? throw new ArgumentNullException(nameof(exchange));
            this.logger = logger;
            this.ws = new ClientWebSocket();
            this.auth = auth;
            this.orderRepository = orderRepository;
        }

        /// <summary>
        /// https://docs.pro.coinbase.com/#protocol-overview
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IObservable<object> GetObservable()
        {
            if (this.wsObservale == null)
            {
                this.wsObservale =
                    Observable.Create(async (IObserver<object> o, CancellationToken token) =>
                    {
                        ws.Options.KeepAliveInterval = TimeSpan.FromSeconds(20);
                        var authHeaders = auth.GetHeaders("/users/self/verify", HttpMethod.Get, "", token);

                        await ws.ConnectAsync(new Uri(endpoint), token);
                        var buffer = new byte[receiveChunkSize];

                        if (ws.State == WebSocketState.Open)
                        {
                            //change to POCO?? maybe not
                            var subscribeDto = new
                            {
                                type = "subscribe",
                                product_ids = this.currencyPairs.Select(cp => cp.ToString()).ToList(),
                                channels = new List<string>() { "ticker", "user" },
                                //auth part
                                signature = authHeaders["CB-ACCESS-SIGN"],
                                key = authHeaders["CB-ACCESS-KEY"],
                                passphrase = authHeaders["CB-ACCESS-PASSPHRASE"],
                                timestamp = authHeaders["CB-ACCESS-TIMESTAMP"],
                            };
                            var subscribeDtoSerialized = JsonConvert.SerializeObject(subscribeDto);
                            try
                            {
                                await SendMessageAsync(subscribeDtoSerialized, token);
                            }
                            catch (Exception e)
                            {
                                logger.Log(e);
                            }
                        }

                        while (ws.State == WebSocketState.Open && !token.IsCancellationRequested)
                        {
                            var stringResult = new StringBuilder();
                            WebSocketReceiveResult result = null;
                            do
                            {
                                try
                                {
                                    result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), token);
                                    if (result.MessageType == WebSocketMessageType.Close)
                                    {
                                        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, token);

                                        //disconnected
                                        logger.Log(new LogEntry(LoggingEventType.Error, "CoinbasePro websocket received signal for disconnection"));
                                    }
                                    else
                                    {
                                        var str = Encoding.UTF8.GetString(buffer, 0, result.Count);
                                        stringResult.Append(str);
                                    }
                                }
                                catch (TaskCanceledException)
                                {
                                    logger.Log(new LogEntry(LoggingEventType.Error, "CoinbasePro websocket threw an exception and got disconnected "));
                                }
                                catch (Exception e)
                                {
                                    logger.Log(e);
                                }

                            } while (result == null || !result.EndOfMessage);

                            //on message received
                            try
                            {
                                if (stringResult != null)
                                {
                                    logger.Log($"Tick received {stringResult.ToString()}");

                                    var dto = JsonConvert.DeserializeObject<WebSocketDto>(stringResult.ToString());
                                    if (dto != null && dto.Type != null)
                                    {
                                        //TICKER
                                        if (dto.Type == "ticker")
                                        {
                                            if (dto.IsValidTicker) //null checking
                                            {
                                                o.OnNext(new Tick(
                                                    exchange,
                                                    new CurrencyPair(
                                                        new Currency(dto.Product_id.Split('-')[0]),
                                                        new Currency(dto.Product_id.Split('-')[1])),
                                                    dto.Best_bid.Value,
                                                    dto.Best_ask.Value,
                                                    DateTime.UtcNow
                                                    )
                                                );
                                            }
                                        }
                                        else
                                        {
                                            logger.Log($"CoinbasePro message received {stringResult.ToString()}");

                                            //ORDERS
                                            /*
                                               * order types to cover:
                                               * 1. market orders executed immediately
                                               * 2. execution on stop losses
                                               * 3. filling of stop losses
                                               * 4. execution of limit orders
                                               * 5. filling of limit orders
                                               * 6. cancellation of pending stop losses
                                               * 7. cancellation of pending limit
                                            */


                                            //Order nextOrder = null;
                                            //if (dto.Type == "done" && dto.Reason == "canceled") //canceled order
                                            //{
                                            //    nextOrder = await this.orderRepository.GetByExternalGuid(dto.Order_id);
                                            //    if (nextOrder != null)
                                            //    {
                                            //        nextOrder.Status = OrderStatus.Removed;
                                            //    }
                                            //}
                                            //else
                                            //{
                                            //    nextOrder = new Order()
                                            //    {
                                            //        Exchange = this.exchange,
                                            //        Status = OrderStatus.Executed,
                                            //        Size = dto.Size,
                                            //        Side = dto.Side == "buy" ? OrderSide.Buy : OrderSide.Sell,
                                            //        CurrencyPair = new CurrencyPair(
                                            //            new Currency(dto.Product_id.Split('-')[0]),
                                            //            new Currency(dto.Product_id.Split('-')[1])),
                                            //        ExternalGuid = dto.Order_id,
                                            //        Timestamp = DateTime.UtcNow
                                            //    };
                                            //    if (dto.Type == "activate" && !String.IsNullOrWhiteSpace(dto.Stop_type)) //stop loss
                                            //    {
                                            //        nextOrder.Price = dto.Stop_price;
                                            //        nextOrder.Type = OrderType.StopLoss;
                                            //    }
                                            //    else if (dto.Type == "received" || dto.Type == "done") //limit
                                            //    {
                                            //        nextOrder.Price = dto.Price;
                                            //        nextOrder.Type = OrderType.Limit;
                                            //    }
                                            //}
                                            //if (nextOrder != null)
                                            //{
                                            //    o.OnNext(nextOrder);
                                            //}
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                logger.Log(e);
                            }
                        }
                        logger.Log(new LogEntry(LoggingEventType.Error, "CoinbasePro websocket completed"));

                        if (ws.State == WebSocketState.Open)
                        {
                            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, token);
                        }
                        ws.Dispose();

                        o.OnCompleted();
                    })
            .Publish()
            .RefCount();
            }
            return this.wsObservale;
        }



        /// <summary>
        /// taken from https://gist.github.com/xamlmonkey/4737291
        /// </summary>
        /// <param name="message"></param>
        /// <param name="token"></param>
        private async Task SendMessageAsync(string message, CancellationToken token)
        {
            if (ws.State != WebSocketState.Open)
            {
                throw new Exception("Connection is not open.");
            }

            var messageBuffer = Encoding.UTF8.GetBytes(message);
            var messagesCount = (int)Math.Ceiling((double)messageBuffer.Length / sendChunkSize);

            for (var i = 0; i < messagesCount; i++)
            {
                var offset = (sendChunkSize * i);
                var count = sendChunkSize;
                var lastMessage = ((i + 1) == messagesCount);

                if ((count * (i + 1)) > messageBuffer.Length)
                {
                    count = messageBuffer.Length - offset;
                }

                await ws.SendAsync(new ArraySegment<byte>(messageBuffer, offset, count), WebSocketMessageType.Text, lastMessage, token);
            }
        }
    }
}
