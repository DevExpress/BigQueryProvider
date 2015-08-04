#if DEBUGTEST
using System;
using System.Collections.Generic;
using System.Data;
using NUnit.Framework;

namespace DevExpress.DataAccess.BigQuery.Tests {
    public class BigQueryParameterComparer : IEqualityComparer<BigQueryParameter> {
        public static bool Equals(BigQueryParameter x, BigQueryParameter y) {
            if(x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;
            return (x.Value == y.Value) && (x.ParameterName == y.ParameterName);
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
            var param = new BigQueryParameter {
                Value = "test string",
                ParameterName = "test_parameter"
            };
            var clone = param.Clone();
            Assert.IsTrue(BigQueryParameterComparer.Equals(clone, param));
        }
    }
}
#endif