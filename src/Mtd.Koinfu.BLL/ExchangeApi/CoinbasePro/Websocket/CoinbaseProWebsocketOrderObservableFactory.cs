using System;
using System.Collections.Generic;
using System.Text;
using System.Reactive.Linq;

namespace Mtd.Koinfu.BLL.CoinbasePro
{
    public class CoinbaseProWebsocketOrderObservableFactory : IObservableFactory<Order>
    {
        private readonly IObservableFactory<object> websocketClient;

        public CoinbaseProWebsocketOrderObservableFactory(IObservableFactory<object> websocketClient)
        {
            this.websocketClient = websocketClient;
        }

        public IObservable<Order> GetObservable()
        {
           return websocketClient.GetObservable().Where(a => a is Order).Cast<Order>();
        }
    }
}
