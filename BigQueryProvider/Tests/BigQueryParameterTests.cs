#if DEBUGTEST
using System.Collections.Generic;
using NUnit.Framework;

namespace DevExpress.DataAccess.BigQuery.Tests {
    public class BigQueryParameterComparer : IEqualityComparer<BigQueryParameter> {
        public static bool Equals(BigQueryParameter x, BigQueryParameter y) {
            return x == y;
        }

        bool IEqualityComparer<BigQueryParameter>.Equals(BigQueryParameter x, BigQueryParameter y) {
            return Equals(x, y);
        }

        int IEqualityComparer<BigQueryParameter>.GetHashCode(BigQueryParameter obj) {
            return obj.GetHashCode();
        }
    }

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
                Assert.IsTrue(BigQueryParameterComparer.Equals(clone, param));
            }
        }
    }
}
#endif