using System.Data;
using MongoDB.Driver;

namespace AppCore.MongoDB
{
    public interface ITransactionOptionsAdapter
    {
        TransactionOptions GetTransactionOptions(IsolationLevel isolationLevel);
    }
}
