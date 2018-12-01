using AutoMapper;
using Mds.Koinfu.BLL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mds.Koinfu.DAL
{
    public class AutomapperWrapper : BLL.IMapper
    {
        public static void InitializeConfiguration()
        {
            Mapper.Initialize(cfg =>
            {
                //from business to dto
                cfg.CreateMap<Tick, PsqlTickDto>()
                .ForMember(d => d.exchangeid, opt => opt.MapFrom(s => s.Exchange.Id))
                .ForMember(d => d.currencypairid, opt => opt.MapFrom(s => s.CurrencyPair.Id)); 

                //from dto to business
                cfg.CreateMap<PsqlTickDto, Tick>();

                //single mapping

                cfg.CreateMap<Currency, PsqlCurrencyDto>().ReverseMap();
                cfg.CreateMap<CurrencyPair, PsqlCurrencyPairDto>().ReverseMap();
                cfg.CreateMap<Exchange, PsqlExchangeDto>().ReverseMap();
                cfg.CreateMap<CurrencyAlias, PsqlCurrencyAliasDto>().ReverseMap();
                cfg.CreateMap<Order, PsqlOrderDto>().ReverseMap();
                cfg.CreateMap<FiatExchangeRate, PsqlFiatExchangeRateDto>().ReverseMap();
            });
        }

        public TDestination Map<TSource, TDestination>(TSource source)
            => Mapper.Map<TSource, TDestination>(source);

    }
}
