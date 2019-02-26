using System;
using System.Collections.Generic;
using System.Text;
using System.Reactive.Linq;

namespace Ladasoft.Koinfu.BLL.CoinbasePro
{
    public class CoinbaseProWebsocketTickObservableFactory : IObservableFactory<Tick>
    {
        private readonly IObservableFactory<object> websocketClient;
        private readonly CurrencyPair currencyPair;

        public CoinbaseProWebsocketTickObservableFactory(IObservableFactory<object> websocketClient, CurrencyPair currencyPair)
        {
            this.websocketClient = websocketClient;
            this.currencyPair = currencyPair;
        }

        public IObservable<Tick> GetObservable()
        {
            return websocketClient.GetObservable().Where(a => a is Tick).Cast<Tick>().Where(a => a.CurrencyPair == currencyPair);
        }
    }
}
