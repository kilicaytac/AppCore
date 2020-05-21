using AppCore.Mapper.Mapster.Test.Configuration;
using Xunit;

namespace AppCore.Mapper.Mapster.Test
{
    public class MapsterWrapperTest
    {
        [Fact]
        public void Map_Should_Map_Source_Object_To_Destination_Object()
        {
            //Arrange
            Source source = new Source { Id = 1, Value = "Beþiktaþ" };
            MapsterWrapper mapsterWrapper = new MapsterWrapper(null);

            //Act
            Destination destination = mapsterWrapper.Map<Source, Destination>(source);

            //Assert
            Assert.Equal(destination.Id, source.Id);
            Assert.Equal(destination.Value, source.Value);
        }
    }
}
