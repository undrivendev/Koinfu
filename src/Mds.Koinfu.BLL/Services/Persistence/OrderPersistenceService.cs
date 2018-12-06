using Mds.Common.Logging;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading;

namespace Mds.Koinfu.BLL
{
    public class OrderPersistenceService
    {
        private readonly IObservableFactory<Order> obsProvider;
        private readonly IOrderRepository orderRepository;
        private readonly ILogger logger;

        public OrderPersistenceService(IObservableFactory<Order> obsProvider, IOrderRepository orderRepository, ILogger logger)
        {
            this.obsProvider = obsProvider;
            this.orderRepository = orderRepository;
            this.logger = logger;
        }

        public void Start(CancellationToken token)
        {
            try
            {
                this.obsProvider.GetObservable()
                    .DistinctUntilChanged()
                    .Subscribe(
                    async (order) =>
                    {
                        await orderRepository.InsertOrUpdateAsync(order);
                        logger.Log($"{obsProvider.GetType().Name} Order - {DateTime.Now.ToString("HH:mm:ss.fff")}");
                    },
                    error =>
                    {
                        logger.Log(error);
                    },
                    token);
            }
            catch (Exception e)
            {
                logger.Log(e);
            }
        }
    }
}
