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

        [Test]
        public void NotInitializeDbTypeTest() {
            var param = new BigQueryParameter {Value = 1};
            Assert.AreEqual(DbType.Int64, param.DbType);
        }

        [Test]
        public void NotInitializeBigQueryDbTypeTest() {
            var param = new BigQueryParameter("test", DbType.String);
            Assert.AreEqual(BigQueryDbType.String, param.BigQueryDbType);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ValidationEmptyNameTest() {
            var param = new BigQueryParameter {
                Value = 1,
                DbType = DbType.Single
            };
            param.Validate();
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ValidationNullValueTest() {
            var param = new BigQueryParameter {ParameterName = "nullValueParameter"};
            param.Validate();
        }

        [Test, ExpectedException(typeof(NotSupportedException))]
        public void ValidateUnsupportedBigQueryDbType() {
            var param = new BigQueryParameter("test", DbType.Byte) {Value = 12};
            Assert.AreEqual(BigQueryDbType.Unknown, param.BigQueryDbType);
            param.Validate();
        }

        [Test]
        public void DefaultValueTest() {
            var param = new BigQueryParameter("test", DbType.Int64);
            Assert.AreEqual(0, param.Value);
            param.Validate();
        }

        [Test]
        public void ChangeDbTypeTest() {
            var param = new BigQueryParameter("test", DbType.Single) {Value = 123};
            Assert.AreEqual(DbType.Single, param.DbType);
            Assert.AreEqual(BigQueryDbType.Float, param.BigQueryDbType);
            param.DbType = DbType.Int64;
            Assert.AreEqual(BigQueryDbType.Integer, param.BigQueryDbType);
            param.DbType = DbType.String;
            Assert.AreEqual(BigQueryDbType.String, param.BigQueryDbType);
        }

        [Test]
        public void ValidateValueConvertTest() {
            var param = new BigQueryParameter("test", DbType.Int64) {Value = 123.0F};
            param.Validate();
            param.DbType = DbType.String;
            param.Validate();
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ValidateValueNotConvertTest() {
            var param = new BigQueryParameter("test", DbType.DateTime) { Value = 123.0F };
            param.Validate();
        }

    }
}
#endif