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
            return (x.Value == y.Value) &&
                   (x.ParameterName == y.ParameterName) &&
                   (x.DbType == y.DbType) &&
                   (x.BigQueryDbType == y.BigQueryDbType) &&
                   (x.IsNullable == y.IsNullable) &&
                   (x.SourceColumnNullMapping == y.SourceColumnNullMapping) &&
                   (x.Direction == y.Direction) &&
                   (x.Size == y.Size) &&
                   (x.SourceColumn == y.SourceColumn) &&
                   (x.SourceVersion == y.SourceVersion);
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
            const string paramName = "testParam";
            var param = new BigQueryParameter(paramName, DbType.Int64);
            Assert.Equal(paramName, param.ParameterName);
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
        public void InferDbTypeAndBigQueryDbTypeFromValueTest() {
            var intValueParam = new BigQueryParameter {Value = 1};
            Assert.NotNull(intValueParam.Value);
            Assert.Equal(DbType.Int64, intValueParam.DbType);
            Assert.Equal(BigQueryDbType.Integer, intValueParam.BigQueryDbType);

            var stringValueParam = new BigQueryParameter { Value = "test" };
            Assert.NotNull(stringValueParam.Value);
            Assert.Equal(DbType.String, stringValueParam.DbType);
            Assert.Equal(BigQueryDbType.String, stringValueParam.BigQueryDbType);

            var nullValueParam = new BigQueryParameter { Value = null };
            Assert.Null(nullValueParam.Value);
            Assert.Equal(DbType.Object, nullValueParam.DbType);
            Assert.Equal(BigQueryDbType.Unknown, nullValueParam.BigQueryDbType);

            var notInitializeValueParam = new BigQueryParameter();
            Assert.Null(notInitializeValueParam.Value);
            Assert.Equal(DbType.Object, notInitializeValueParam.DbType);
            Assert.Equal(BigQueryDbType.Unknown, notInitializeValueParam.BigQueryDbType);
        }

        [Fact]
        public void ValidationEmptyNameTest() {
            var param = new BigQueryParameter {
                Value = 1,
                DbType = DbType.Single
            };
            Assert.NotNull(param.Value);
            Assert.Throws<ArgumentException>(() => param.Validate());
            param.ParameterName = "test";
            param.Validate();
        }

        [Fact]
        public void ValidationNullValueTest() {
            var param = new BigQueryParameter {ParameterName = "nullValueParameter"};
            Assert.Throws<ArgumentException>(() => param.Validate());
            param.Value = 13;
            param.Validate();
        }

        [Fact]
        public void ValidateUnsupportedBigQueryDbType() {
            var param = new BigQueryParameter("test", DbType.Byte) {Value = 12};
            Assert.NotNull(param.Value);
            Assert.Equal(BigQueryDbType.Unknown, param.BigQueryDbType);
            Assert.Throws<NotSupportedException>(() => param.Validate());
            param.DbType = DbType.Int64;
            param.Validate();
        }

        [Fact]
        public void DefaultValueTest() {
            var intParam = new BigQueryParameter("test", DbType.Int64);
            Assert.Equal(default(Int64), intParam.Value);

            var stringParam = new BigQueryParameter("test", DbType.String);
            Assert.Equal(null, stringParam.Value);

            var objectParam = new BigQueryParameter("test", DbType.Object);
            Assert.Equal(null, objectParam.Value);
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
            param.DbType = DbType.Object;
            Assert.Equal(BigQueryDbType.Unknown, param.BigQueryDbType);
        }

        [Fact]
        public void ValidateValueConvertTest() {
            var param = new BigQueryParameter("test", DbType.Int64) {Value = 123.0F};
            Assert.NotNull(param.Value);
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