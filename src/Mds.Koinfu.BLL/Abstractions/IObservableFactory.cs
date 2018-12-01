using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mds.Koinfu.BLL
{
    public interface IObservableFactory<T>
    {
        IObservable<T> GetObservable();
    }
}
