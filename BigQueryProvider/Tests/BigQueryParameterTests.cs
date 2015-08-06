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
        public void ConstructorTest() {
            var emptyParam = new BigQueryParameter();
            Assert.AreEqual(DbType.Object, emptyParam.DbType);
            Assert.AreEqual(BigQueryDbType.Unknown, emptyParam.BigQueryDbType);
            Assert.AreEqual(null, emptyParam.Value);
            Assert.AreEqual(null, emptyParam.ParameterName);
            var param = new BigQueryParameter("testParam", DbType.Int64);
            Assert.AreEqual("testParam", param.ParameterName);
            Assert.AreEqual(DbType.Int64, param.DbType);
            Assert.AreEqual(BigQueryDbType.Integer, param.BigQueryDbType);
            Assert.AreEqual(default(long), param.Value);
        }
        
        [Test]
        public void CloneParameterTest() {
            var param = new BigQueryParameter {
                Value = "test string",
                ParameterName = "test_parameter"
            };
            Assert.NotNull(param.Value);
            var clone = param.Clone();
            Assert.NotNull(clone);
            Assert.IsTrue(BigQueryParameterComparer.Equals(clone, param));
        }

        [Test]
        public void NotInitializeDbTypeTest() {
            var param = new BigQueryParameter {Value = 1};
            Assert.NotNull(param.Value);
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
            Assert.NotNull(param.Value);
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
            Assert.NotNull(param.Value);
            Assert.AreEqual(BigQueryDbType.Unknown, param.BigQueryDbType);
            param.Validate();
        }

        [Test]
        public void DefaultValueTest() {
            var param = new BigQueryParameter("test", DbType.Int64);
            Assert.AreEqual(default(Int64), param.Value);
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
            Assert.NotNull(param.Value);
            param.Validate();
            param.DbType = DbType.String;
            param.Validate();
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ValidateValueNotConvertTest() {
            var param = new BigQueryParameter("test", DbType.DateTime) { Value = 123.0F };
            Assert.NotNull(param.Value);
            param.Validate();
        }
    }
}
#endif