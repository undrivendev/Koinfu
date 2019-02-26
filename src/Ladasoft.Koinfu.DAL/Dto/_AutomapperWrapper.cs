using AutoMapper;
using Ladasoft.Koinfu.BLL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ladasoft.Koinfu.DAL
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
                cfg.CreateMap<Currency, PsqlCurrencyDto>();
                cfg.CreateMap<CurrencyPair, PsqlCurrencyPairDto>();
                cfg.CreateMap<Exchange, PsqlExchangeDto>();
                cfg.CreateMap<CurrencyAlias, PsqlCurrencyAliasDto>();
                cfg.CreateMap<Order, PsqlOrderDto>();
                cfg.CreateMap<FiatExchangeRate, PsqlFiatExchangeRateDto>();

                //reverse
                cfg.CreateMap<PsqlCurrencyDto, Currency>();
                cfg.CreateMap<PsqlCurrencyPairDto, CurrencyPair>();
                cfg.CreateMap<PsqlExchangeDto, Exchange>();
                cfg.CreateMap<PsqlCurrencyAliasDto, CurrencyAlias>();
                cfg.CreateMap<PsqlOrderDto, Order>();
                cfg.CreateMap<PsqlFiatExchangeRateDto, FiatExchangeRate>();


            });
            Mapper.AssertConfigurationIsValid();
        }

        public TDestination Map<TSource, TDestination>(TSource source)
            => Mapper.Map<TSource, TDestination>(source);

    }
}
