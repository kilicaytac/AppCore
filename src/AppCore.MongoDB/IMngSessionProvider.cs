using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppCore.MongoDB
{
    public interface IMngSessionProvider
    {
        IClientSessionHandle GetSession();
    }
}
