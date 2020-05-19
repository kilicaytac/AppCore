using AppCore.Mapper.AutoMapper.Test.Configuration;
using Xunit;
using AutoMapperCore = AutoMapper;

namespace AppCore.Mapper.AutoMapper.Test
{
    public class AutoMapperWrapperTest
    {
        [Fact]
        public void Map_Should_Map_Source_Object_To_Target_Object()
        {
            //Arrange
            Source source = new Source { Id = 1, Value = "Beþiktaþ" };

            var mappingConfig = new AutoMapperCore.MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            AutoMapperWrapper autoMapperWrapper = new AutoMapperWrapper(mappingConfig);

            //Act
            Destination target = autoMapperWrapper.Map<Source, Destination>(source);

            //Assert
            Assert.Equal(target.Id, source.Id);
            Assert.Equal(target.Value, source.Value);
        }
    }
}
