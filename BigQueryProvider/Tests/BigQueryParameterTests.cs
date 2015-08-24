#if DEBUGTEST
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Xunit;

namespace DevExpress.DataAccess.BigQuery.Tests {
    public class BigQueryParameterTests {
        const string parameterName = "@foo";
        const string stringValue = "Foo";
        const int intValue = 1;
        const float floatValue = 1.1f;
        const int maxStringSize = 2097152;
        const int rangeValue = 400000;

        [Fact]
        public void ConstructorTest() {
            var emptyParam = new BigQueryParameter();
            Assert.Equal(DbType.Object, emptyParam.DbType);
            Assert.Equal(BigQueryDbType.Unknown, emptyParam.BigQueryDbType);
            Assert.Null(emptyParam.Value);
            Assert.Null(emptyParam.ParameterName);

            var paramDbType = new BigQueryParameter(parameterName, DbType.Int64);
            Assert.Equal(parameterName, paramDbType.ParameterName);
            Assert.Equal(DbType.Int64, paramDbType.DbType);
            Assert.Equal(BigQueryDbType.Integer, paramDbType.BigQueryDbType);
            Assert.Equal(default(long), paramDbType.Value);

            var paramBQDbType = new BigQueryParameter(parameterName, BigQueryDbType.Integer);
            Assert.Equal(parameterName, paramBQDbType.ParameterName);
            Assert.Equal(DbType.Int64, paramBQDbType.DbType);
            Assert.Equal(BigQueryDbType.Integer, paramBQDbType.BigQueryDbType);
            Assert.Equal(default(long), paramBQDbType.Value);
        }

        [Fact]
        public void ParameterValueTest() {
            var emptyParam = new BigQueryParameter();
            Assert.Null(emptyParam.Value);

            var paramInitializeValue = new BigQueryParameter(parameterName, DbType.String) {Value = stringValue};
            Assert.Equal(stringValue, paramInitializeValue.Value);

            var intParam = new BigQueryParameter(parameterName, DbType.Int64);
            Assert.Equal(default(long), intParam.Value);

            var stringParam = new BigQueryParameter(parameterName, DbType.String);
            Assert.Null(stringParam.Value);

            var objectParam = new BigQueryParameter(parameterName, DbType.Object);
            Assert.Null(objectParam.Value);
        }

        [Fact]
        public void CloneParameterTest() {
            var param = new BigQueryParameter {
                Value = parameterName,
                ParameterName = stringValue
            };
            var clone = param.Clone();
            Assert.NotNull(clone);
            Assert.NotSame(param, clone);
            bool equal = (param.Value == clone.Value) &&
                (param.ParameterName == clone.ParameterName) &&
                (param.DbType == clone.DbType) &&
                (param.BigQueryDbType == clone.BigQueryDbType) &&
                (param.IsNullable == clone.IsNullable) &&
                (param.SourceColumnNullMapping == clone.SourceColumnNullMapping) &&
                (param.Direction == clone.Direction) &&
                (param.IsNullable == clone.IsNullable) &&
                (param.Size == clone.Size) &&
                (param.SourceColumn == clone.SourceColumn) &&
                (param.SourceVersion == clone.SourceVersion);
            Assert.True(equal);
        }

        [Theory]
        [InlineData(intValue, DbType.Int64, BigQueryDbType.Integer)]
        [InlineData(stringValue, DbType.String, BigQueryDbType.String)]
        [InlineData(null, DbType.Object, BigQueryDbType.Unknown)]
        public void InferDbTypeAndBigQueryDbTypeFromValueTest(object value, DbType exceptedDbType, BigQueryDbType exceptedBQDbType) {
            var param = new BigQueryParameter() {Value = value};
            Assert.Equal(value, param.Value);
            Assert.Equal(exceptedDbType, param.DbType);
            Assert.Equal(exceptedBQDbType, param.BigQueryDbType);
        }

        [Theory]
        [InlineData(DbType.Int64, BigQueryDbType.Integer)]
        [InlineData(DbType.String, BigQueryDbType.String)]
        [InlineData(DbType.Object, BigQueryDbType.Unknown)]
        public void ChangeDbTypeTest(DbType dbType, BigQueryDbType bigQueryDbType) {
            var param = new BigQueryParameter(parameterName, DbType.Single) { Value = intValue };
            Assert.Equal(DbType.Single, param.DbType);
            Assert.Equal(BigQueryDbType.Float, param.BigQueryDbType);
            param.DbType = dbType;
            Assert.Equal(dbType, param.DbType);
            Assert.Equal(bigQueryDbType, param.BigQueryDbType);
        }

        [Theory]
        [InlineData(BigQueryDbType.Integer, DbType.Int64)]
        [InlineData(BigQueryDbType.String, DbType.String)]
        [InlineData(BigQueryDbType.Unknown, DbType.Object)]
        public void ChangeBigQueryDbTypeTest(BigQueryDbType bigQueryDbType, DbType dbType) {
            var param = new BigQueryParameter(parameterName, DbType.Single) { Value = intValue };
            Assert.Equal(DbType.Single, param.DbType);
            Assert.Equal(BigQueryDbType.Float, param.BigQueryDbType);
            param.BigQueryDbType = bigQueryDbType;
            Assert.Equal(bigQueryDbType, param.BigQueryDbType);
            Assert.Equal(dbType, param.DbType);
        }

        [Fact]
        public void DirectionTypeTest() {
            var param = new BigQueryParameter(parameterName, DbType.String);
            Assert.Equal(ParameterDirection.Input, param.Direction);
            Assert.Throws<ArgumentOutOfRangeException>(() => param.Direction = ParameterDirection.Output);
            param.Direction = ParameterDirection.Input;
        }

        [Fact]
        public void ValidationEmptyNameTest() {
            var param = new BigQueryParameter {
                Value = intValue,
                DbType = DbType.Single
            };
            Assert.True(string.IsNullOrEmpty(param.ParameterName));
            Assert.Throws<ArgumentException>(() => param.Validate());
            param.ParameterName = parameterName;
            param.Validate();
        }

        [Fact]
        public void ValidationNullValueTest() {
            var param = new BigQueryParameter() {ParameterName = parameterName};
            Assert.Null(param.Value);
            Assert.Throws<ArgumentException>(() => param.Validate());
            param.Value = intValue;
            param.Validate();
        }

        [Fact]
        public void ValidateUnsupportedBigQueryDbTypeTest() {
            var param = new BigQueryParameter(parameterName, DbType.Byte) {Value = intValue};
            Assert.Equal(intValue, param.Value);
            Assert.Equal(BigQueryDbType.Unknown, param.BigQueryDbType);
            Assert.Throws<NotSupportedException>(() => param.Validate());
            param.BigQueryDbType = BigQueryDbType.Integer;
            param.Validate();
        }

        [Fact]
        public void ValidateValueCanConvertTest() {
            var param = new BigQueryParameter(parameterName, DbType.Int64) { Value = floatValue };
            param.Validate();
        }

        [Fact]
        public void ValidateValueCanNotConvertTest() {
            var param = new BigQueryParameter(parameterName, DbType.DateTime) { Value = floatValue };
            Assert.Throws<ArgumentException>(() => param.Validate());
        }

        [Fact]
        public void ValidateChangeIsnullableTest() {
            var param = new BigQueryParameter(parameterName, DbType.String);
            Assert.False(param.IsNullable);
            param.IsNullable = false;
            Assert.Throws<ArgumentOutOfRangeException>(() => param.IsNullable = true);
        }

        [Fact]
        public void ValidateSizeTest() {
            var param = new BigQueryParameter(parameterName, DbType.String);
            IEnumerable<int> str = Enumerable.Range(0, rangeValue);
            var value = string.Join("", str).Remove(maxStringSize + 1);

            param.Value = value;
            Assert.Equal(maxStringSize + 1, param.Size);
            Assert.Throws<ArgumentException>(() => param.Validate());

            param.Value = value.Remove(maxStringSize);
            param.Validate();

            param.DbType = DbType.Int64;
            param.Value = 1;
            Assert.Equal(0, param.Size);
        }
    }
}
#endif