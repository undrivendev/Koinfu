using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Mtd.Koinfu.BLL
{
    public class OrderMailAlertService
    {
        private readonly IObservable<Order> orderObservable;
        private readonly MailSender mailsender;

        public OrderMailAlertService(IObservable<Order> orderObservable, MailSender mailsender)
        {
            this.orderObservable = orderObservable != null ? orderObservable : throw new ArgumentNullException("orderObservable must not be null");
            this.mailsender = mailsender != null ? mailsender : throw new ArgumentNullException("mailsender must not be null");

        }

        public void StartMailing(CancellationToken token, params string[] mailRecipients)
        {
            this.orderObservable.Subscribe(
                async (order) =>
                {
                    await mailsender.SendMailsAsync($"Order received:\r\nTime: {order.Timestamp}\r\nPrice: {order.Price}\r\n{order.Size}", token, mailRecipients);
                },
            async (order) =>
            {
                await mailsender.SendMailsAsync($"Warning: CoinbasePro websocket completed", token, mailRecipients);
            }
            );
        }
    }
}
