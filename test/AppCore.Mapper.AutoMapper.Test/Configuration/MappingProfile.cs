using AutoMapper;

namespace AppCore.Mapper.AutoMapper.Test.Configuration
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Source, Destination>();
        }
    }
}
