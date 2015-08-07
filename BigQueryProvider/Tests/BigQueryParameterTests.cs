#if DEBUGTEST
using System;
using System.Collections.Generic;
using System.Data;
using Xunit;

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

    public class BigQueryParameterTests {
        [Fact]
        public void ConstructorTest() {
            var emptyParam = new BigQueryParameter();
            Assert.Equal(DbType.Object, emptyParam.DbType);
            Assert.Equal(BigQueryDbType.Unknown, emptyParam.BigQueryDbType);
            Assert.Equal(null, emptyParam.Value);
            Assert.Equal(null, emptyParam.ParameterName);
            var param = new BigQueryParameter("testParam", DbType.Int64);
            Assert.Equal("testParam", param.ParameterName);
            Assert.Equal(DbType.Int64, param.DbType);
            Assert.Equal(BigQueryDbType.Integer, param.BigQueryDbType);
            Assert.Equal(default(long), param.Value);
        }

        [Fact]
        public void CloneParameterTest() {
            var param = new BigQueryParameter {
                Value = "test string",
                ParameterName = "test_parameter"
            };
            Assert.NotNull(param.Value);
            var clone = param.Clone();
            Assert.NotNull(clone);
            Assert.True(BigQueryParameterComparer.Equals(clone, param));
        }

        [Fact]
        public void NotInitializeDbTypeTest() {
            var param = new BigQueryParameter {Value = 1};
            Assert.NotNull(param.Value);
            Assert.Equal(DbType.Int64, param.DbType);
        }

        [Fact]
        public void NotInitializeBigQueryDbTypeTest() {
            var param = new BigQueryParameter("test", DbType.String);
            Assert.Equal(BigQueryDbType.String, param.BigQueryDbType);
        }

        [Fact]
        public void ValidationEmptyNameTest() {
            var param = new BigQueryParameter {
                Value = 1,
                DbType = DbType.Single
            };
            Assert.NotNull(param.Value);
            Assert.Throws<ArgumentException>(() => param.Validate());
        }

        [Fact]
        public void ValidationNullValueTest() {
            var param = new BigQueryParameter {ParameterName = "nullValueParameter"};
            Assert.Throws<ArgumentException>(() => param.Validate());
        }

        [Fact]
        public void ValidateUnsupportedBigQueryDbType() {
            var param = new BigQueryParameter("test", DbType.Byte) {Value = 12};
            Assert.NotNull(param.Value);
            Assert.Equal(BigQueryDbType.Unknown, param.BigQueryDbType);
            Assert.Throws<NotSupportedException>(() => param.Validate());
        }

        [Fact]
        public void DefaultValueTest() {
            var param = new BigQueryParameter("test", DbType.Int64);
            Assert.Equal(default(Int64), param.Value);
            param.Validate();
        }

        [Fact]
        public void ChangeDbTypeTest() {
            var param = new BigQueryParameter("test", DbType.Single) {Value = 123};
            Assert.Equal(DbType.Single, param.DbType);
            Assert.Equal(BigQueryDbType.Float, param.BigQueryDbType);
            param.DbType = DbType.Int64;
            Assert.Equal(BigQueryDbType.Integer, param.BigQueryDbType);
            param.DbType = DbType.String;
            Assert.Equal(BigQueryDbType.String, param.BigQueryDbType);
        }

        [Fact]
        public void ValidateValueConvertTest() {
            var param = new BigQueryParameter("test", DbType.Int64) {Value = 123.0F};
            Assert.NotNull(param.Value);
            param.Validate();
            param.DbType = DbType.String;
            param.Validate();
        }

        [Fact]
        public void ValidateValueNotConvertTest() {
            var param = new BigQueryParameter("test", DbType.DateTime) { Value = 123.0F };
            Assert.NotNull(param.Value);
            Assert.Throws<ArgumentException>(() => param.Validate());
        }
    }
}
#endif