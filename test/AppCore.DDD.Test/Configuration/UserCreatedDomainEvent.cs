using System;
using System.Collections.Generic;
using System.Text;

namespace AppCore.DDD.Test.Configuration
{
    public class UserCreatedDomainEvent : IDomainEvent
    {
        private readonly User _user;
        public User User { get { return _user; } }
        public UserCreatedDomainEvent(User user)
        {
            _user = user;
        }
    }
}
