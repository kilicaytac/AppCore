using AppCore.Mapper.Mapster.Test.Configuration;
using Xunit;

namespace AppCore.Mapper.Mapster.Test
{
    public class MapsterWrapperTest
    {
        [Fact]
        public void Map_Should_Map_Source_Object_To_Target_Object()
        {
            //Arrange
            Source source = new Source { Id = 1, Value = "Beþiktaþ" };
            MapsterWrapper mapsterWrapper = new MapsterWrapper(null);

            //Act
            Destination target = mapsterWrapper.Map<Source, Destination>(source);

            //Assert
            Assert.Equal(target.Id, source.Id);
            Assert.Equal(target.Value, source.Value);
        }
    }
}
