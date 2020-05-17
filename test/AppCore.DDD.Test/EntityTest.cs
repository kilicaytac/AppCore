using AppCore.DDD.Test.Configuration;
using System;
using Xunit;

namespace AppCore.DDD.Test
{
    public class EntityTest
    {
        [Fact]
        public void AddDomainEvent_Should_Add_Domain_Event_To_Doman_Events_List()
        {
            //Arrange
            User user = new User("aytaç", "kýlýç");

            //Act
            user.AddDomainEvent(new UserCreatedDomainEvent(user));

            //Assert
            Assert.True(user.DomainEvents.Count == 1);
        }

        [Fact]
        public void RemoveDomainEvent_Should_Remove_Domain_Event_From_Domain_Event_List()
        {
            //Arrange
            User user = new User("aytaç", "kýlýç");

            //Act
            var domainEvent = new UserCreatedDomainEvent(user);
            user.AddDomainEvent(domainEvent);
            user.RemoveDomainEvent(domainEvent);

            //Assert
            Assert.True(user.DomainEvents.Count == 0);
        }

        [Fact]
        public void ClearDomainEvents_Should_Remove_All_Domain_Events_From_Domain_Event_List()
        {
            //Arrange
            User user = new User("aytaç", "kýlýç");

            //Act
            var domainEvent = new UserCreatedDomainEvent(user);
            user.AddDomainEvent(domainEvent);
            user.ClearDomainEvents();

            //Assert
            Assert.True(user.DomainEvents.Count == 0);
        }

    }
}
