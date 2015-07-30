#if DEBUGTEST
using NUnit.Framework;

namespace DevExpress.DataAccess.BigQuery.Tests {
    [TestFixture]
    class BigQueryParameterTests {

        [Test]
        public void CloneParameterTest() {
            var connection = new BigQueryConnection();
            using(var dbCommand = connection.CreateCommand()) {
                var param = (BigQueryParameter)dbCommand.CreateParameter();
                param.Value = "test string";
                param.ParameterName = "test_parameter";
                var clone = param.Clone();
                Assert.IsTrue(clone.IsEqual(param));
            }
        }
    }
}
#endif