using System.Data;
using MongoDB.Driver;

namespace AppCore.MongoDB
{
    public interface ITransactionOptionsConverter
    {
        TransactionOptions ConvertFromIsolationLevel(IsolationLevel ısolationLevel);
    }
}
