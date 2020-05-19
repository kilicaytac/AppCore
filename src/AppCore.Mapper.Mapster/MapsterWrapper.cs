using MapsterCore = Mapster;
using MapsterMapper;

namespace AppCore.Mapper.Mapster
{
    public class MapsterWrapper : IMapper
    {
        private readonly MapsterMapper.IMapper _mapper;
        public MapsterWrapper(MapsterCore.TypeAdapterConfig config)
        {
            _mapper = new MapsterMapper.Mapper();
        }
        public TDestination Map<TSource, TDestination>(TSource source)
            where TSource : class
            where TDestination : class
        {
            return _mapper.Map<TSource, TDestination>(source);
        }
    }
}
