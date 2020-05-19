using AutoMapperCore = AutoMapper;

namespace AppCore.Mapper.AutoMapper
{
    public class AutoMapperWrapper : IMapper
    {
        private readonly AutoMapperCore.IMapper _mapper;
        public AutoMapperWrapper(AutoMapperCore.MapperConfiguration configuration)
        {
            _mapper = configuration.CreateMapper();
        }
       
        public TDestination Map<TSource, TDestination>(TSource source)
            where TSource : class
            where TDestination : class
        {
            return _mapper.Map<TSource, TDestination>(source);
        }
    }
}
