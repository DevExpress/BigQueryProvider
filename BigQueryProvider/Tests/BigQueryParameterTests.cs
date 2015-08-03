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
            var connection = new BigQueryConnection();
            using(var dbCommand = connection.CreateCommand()) {
                var param = (BigQueryParameter)dbCommand.CreateParameter();
                param.Value = "test string";
                param.ParameterName = "test_parameter";
                var clone = param.Clone();
                Assert.IsTrue(BigQueryParameterComparer.Equals(clone, param));
            }
        }

        [Test]
        public void BigQueryTypeConverterTest() {
            var intParameter = new BigQueryParameter("testParameter", (long)1);
            Assert.AreEqual(DbType.Int64, intParameter.DbType);
            Assert.AreEqual(BigQueryDbType.Integer, intParameter.BigQueryDbType);
            var stringParameter = new BigQueryParameter("testParameter", "teststring");
            Assert.AreEqual(DbType.String, stringParameter.DbType);
            Assert.AreEqual(BigQueryDbType.String, stringParameter.BigQueryDbType);
            var floatParameter = new BigQueryParameter("testParameter", (float)1);
            Assert.AreEqual(DbType.Single, floatParameter.DbType);
            Assert.AreEqual(BigQueryDbType.Float, floatParameter.BigQueryDbType);
            var boolParameter = new BigQueryParameter("testParameter", true);
            Assert.AreEqual(DbType.Boolean, boolParameter.DbType);
            Assert.AreEqual(BigQueryDbType.Boolean, boolParameter.BigQueryDbType);
            var dateTimeParameter = new BigQueryParameter("testParameter", new DateTime());
            Assert.AreEqual(DbType.DateTime, dateTimeParameter.DbType);
            Assert.AreEqual(BigQueryDbType.Timestamp, dateTimeParameter.BigQueryDbType);
            var doubleParameter = new BigQueryParameter("testParameter", (double)1);
            Assert.AreEqual(DbType.Double, doubleParameter.DbType);
            Assert.AreEqual(BigQueryDbType.Unknown, doubleParameter.BigQueryDbType);
        }
    }
}
#endif