using System.Data;
using Xunit;
using MongoDB.Driver;

namespace AppCore.MongoDB.Test
{
    public class ExtensionsTest
    {
        public static TheoryData<IsolationLevel, TransactionOptions> DataForIsolationToTransactionOptionsTest = new TheoryData<IsolationLevel, TransactionOptions> {
            { IsolationLevel.ReadUncommitted,new TransactionOptions(readConcern:ReadConcern.Local) }
                                                                                                                                     };
        public ExtensionsTest()
        {

        }

        [Theory(DisplayName = "IsolationLevel to TransactionOptions"), MemberData(nameof(DataForIsolationToTransactionOptionsTest))]
        public void IsolationLevel_To_TransactionOptions_Should_Return_Related_MongoDb_Transaction_Options(IsolationLevel isolationLevel,
                                                                                                    TransactionOptions expected)
        {
            TransactionOptions result = isolationLevel.ToMngTransactionOptions();

            Assert.Equal(expected.ReadConcern, result.ReadConcern);
            Assert.Equal(expected.WriteConcern, result.WriteConcern);
            Assert.Equal(expected.ReadPreference, result.ReadPreference);
            Assert.Equal(expected.MaxCommitTime, result.MaxCommitTime);
        }
    }
}
