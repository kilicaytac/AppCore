using System;
using System.Collections.Generic;
using System.Text;

namespace AppCore.DDD.Test.Configuration
{
    public class User : Entity<int>
    {
        private readonly string _firstName;
        private readonly string _lastName;
        public User(string firstName, string lastName)
        {
            _firstName = firstName;
            _lastName = lastName;
        }

        new public void AddDomainEvent(IDomainEvent domainEvent)
        {
            base.AddDomainEvent(domainEvent);
        }

        new public void RemoveDomainEvent(IDomainEvent domainEvent)
        {
            base.RemoveDomainEvent(domainEvent);
        }

        new public void ClearDomainEvents()
        {
            base.ClearDomainEvents();
        }
    }
}
