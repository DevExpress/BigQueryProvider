#if DEBUGTEST
using System;
using System.Collections.Generic;
using System.Data;
using Xunit;

namespace DevExpress.DataAccess.BigQuery.Tests {
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
        public void ParamValueTest() {
            var emptyParam = new BigQueryParameter();
            Assert.Null(emptyParam.Value);

            var paramNotInitializeValue = new BigQueryParameter("test", DbType.Object);
            Assert.Null(paramNotInitializeValue.Value);

            const string stringValue = "stringValue";
            var paramInitializeValue = new BigQueryParameter("test", DbType.String) {Value = stringValue};
            Assert.Equal(paramInitializeValue.Value, stringValue);

            var intParam = new BigQueryParameter("test", DbType.Int64);
            Assert.Equal(default(Int64), intParam.Value);

            var stringParam = new BigQueryParameter("test", DbType.String);
            Assert.Equal(null, stringParam.Value);

            var objectParam = new BigQueryParameter("test", DbType.Object);
            Assert.Equal(null, objectParam.Value);

        }

        [Fact]
        public void CloneParameterTest() {
            var param = new BigQueryParameter {
                Value = "test string",
                ParameterName = "test_parameter"
            };
            var clone = param.Clone();
            Assert.NotNull(clone);
            bool equal = (param.Value == clone.Value) &&
                (param.ParameterName == clone.ParameterName) &&
                (param.DbType == clone.DbType) &&
                (param.BigQueryDbType == clone.BigQueryDbType) &&
                (param.IsNullable == clone.IsNullable) &&
                (param.SourceColumnNullMapping == clone.SourceColumnNullMapping) &&
                (param.Direction == clone.Direction) &&
                (param.Size == clone.Size) &&
                (param.SourceColumn == clone.SourceColumn) &&
                (param.SourceVersion == clone.SourceVersion);
            Assert.True(equal);
        }

        [Theory]
        [InlineData(DbType.Int64, 1)]
        [InlineData(DbType.String, "test")]
        [InlineData(DbType.Object, null)]
        public void InferDbTypeAndBigQueryDbTypeFromValueTest(DbType dbType, object value) {
            var param = new BigQueryParameter() {Value = value};
            Assert.Equal(value, param.Value);
            Assert.Equal(dbType, param.DbType);
        }

        [Fact]
        public void ValidationEmptyNameTest() {
            var param = new BigQueryParameter {
                Value = 1,
                DbType = DbType.Single
            };
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
            Assert.Equal(12, param.Value);
            Assert.Equal(BigQueryDbType.Unknown, param.BigQueryDbType);
            Assert.Throws<NotSupportedException>(() => param.Validate());
            param.DbType = DbType.Int64;
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
            param.DbType = DbType.Object;
            Assert.Equal(BigQueryDbType.Unknown, param.BigQueryDbType);
        }

        [Fact]
        public void ValidateValueCanConvertTest() {
            var param = new BigQueryParameter("test", DbType.Int64) {Value = 123.0F};
            param.Validate();
        }

        [Fact]
        public void ValidateValueCanNotConvertTest() {
            var param = new BigQueryParameter("test", DbType.DateTime) { Value = 123.0F };
            Assert.Throws<ArgumentException>(() => param.Validate());
        }
    }
}
#endif