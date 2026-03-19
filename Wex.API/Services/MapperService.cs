using AutoMapper;
using System.Globalization;
using Wex.API.Entities;
using Wex.API.Models;

namespace Wex.API.Services
{
    public class MapperService : IMapperService
    {
        private readonly IMapper _mapper;

        public MapperService(ILoggerFactory? loggerFactory = null)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TransactionCreateModel, Transaction>()
                    .ForMember(d => d.Identifier, s => s.MapFrom(src => Guid.NewGuid()))
                    .ForMember(d => d.Date, s => s.MapFrom(src => ToUniversalDateTime(src.Date)))
                    .ForMember(d => d.Id, s => s.Ignore())                    
                    .ForMember(d => d.Card, s => s.Ignore());
                cfg.CreateMap<Transaction, TransactionSavedModel>()
                    .ForMember(d => d.Date, s => s.MapFrom(src => FromUniversalDateTime(src.Date)));
            }, loggerFactory);

            _mapper = config.CreateMapper();
        }

        public TDestination Map<TSource, TDestination>(TSource source)
            where TSource : class
            where TDestination : class
        {
            if (source == null) throw new ArgumentNullException($"{nameof(source)}");

            return _mapper.Map<TSource, TDestination>(source);
        }

        private static DateTime ToUniversalDateTime(string date)
        {
            var dateTime = DateTime.Parse(date, CultureInfo.CurrentCulture);

            // Convert to universal time
            var universalDateTime = dateTime.ToUniversalTime();

            return universalDateTime;
        }

        private static string FromUniversalDateTime(DateTime date)
        {
            // Convert to local time
            DateTime localDateTime = date.ToLocalTime();

            return DateOnly.FromDateTime(localDateTime.Date).ToString(CultureInfo.CurrentCulture);
        }
    }
}
