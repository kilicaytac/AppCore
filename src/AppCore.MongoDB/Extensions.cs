using System.Data;
using MongoDB.Driver;

namespace AppCore.MongoDB
{
    public static class Extensions
    {
        public static TransactionOptions ToMngTransactionOptions(this IsolationLevel isolationLevel)
        {
            TransactionOptions transactionOptions = null;

            switch (isolationLevel)
            {
                case IsolationLevel.ReadUncommitted:
                    transactionOptions = new TransactionOptions(readConcern: ReadConcern.Local);
                    break;
                case IsolationLevel.Snapshot:
                    transactionOptions = new TransactionOptions(readConcern: ReadConcern.Snapshot);
                    break;
                case IsolationLevel.RepeatableRead:
                case IsolationLevel.Chaos:
                case IsolationLevel.Serializable:
                case IsolationLevel.Unspecified:
                case IsolationLevel.ReadCommitted:
                    transactionOptions = new TransactionOptions();
                    break;
                default:
                    break;
            }

            return transactionOptions;
        }
    }
}
