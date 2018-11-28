using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Mtd.Koinfu.BLL
{
    public class CurrencyPairMailAlertService
    {
        private readonly IObservable<Tuple<Exchange, IList<CurrencyPair>>> currencyPairObservable;
        private readonly MailSender mailsender;

        public CurrencyPairMailAlertService(IObservable<Tuple<Exchange, IList<CurrencyPair>>> currencyPairObservable, MailSender mailsender)
        {
            this.currencyPairObservable = currencyPairObservable ?? throw new ArgumentNullException(nameof(currencyPairObservable));
            this.mailsender = mailsender;
        }

        public void StartMailing(CancellationToken token, params string[] mailRecipients)
        {
            this.currencyPairObservable.Subscribe(
                async (currencyPairsForExchange) =>
                {
                    var builder = new StringBuilder($"New currency pairs added for exchange: {currencyPairsForExchange.Item1}\r\n");
                    foreach (var currencyPair in currencyPairsForExchange.Item2)
                    {
                        builder.Append($"{currencyPair}\r\n");
                    }
                    await mailsender.SendMailsAsync(builder.ToString(), token, mailRecipients);
                }
            );
        }

    }
}
