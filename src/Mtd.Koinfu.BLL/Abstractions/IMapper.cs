using System;
using System.Collections.Generic;
using System.Text;

namespace Mtd.Koinfu.BLL
{
    public interface IMapper
    {
        TDestination Map<TSource, TDestination>(TSource source);
    }
}
