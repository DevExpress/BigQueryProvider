#if DEBUGTEST
using System;
using System.Data;
using DevExpress.DataAccess.BigQuery.Native;
using Xunit;

namespace DevExpress.DataAccess.BigQuery.Tests {
    public class BigQueryTypeConverterTests {
        [Theory]
        [InlineData(DbType.String, BigQueryDbType.String, DbType.String)]
        [InlineData(DbType.Boolean, BigQueryDbType.Boolean, DbType.Boolean)]
        [InlineData(DbType.Int64, BigQueryDbType.Integer, DbType.Int64)]
        [InlineData(DbType.Single, BigQueryDbType.Float, DbType.Single)]
        [InlineData(DbType.DateTime, BigQueryDbType.Timestamp, DbType.DateTime)]
        [InlineData(DbType.Double, BigQueryDbType.Unknown, DbType.Object)]
        [InlineData(DbType.Object, BigQueryDbType.Unknown, DbType.Object)]
        public void DbTypeAndBigQueryDbTypeTest(DbType dbType1, BigQueryDbType bigQueryType, DbType dbType2) {
            Assert.Equal(bigQueryType, BigQueryTypeConverter.ToBigQueryDbType(dbType1));
            Assert.Equal(dbType2, BigQueryTypeConverter.ToDbType(bigQueryType));
        }

        [Theory]
        [InlineData(typeof(string), BigQueryDbType.String, typeof(string))]
        [InlineData(typeof(bool), BigQueryDbType.Boolean, typeof(bool))]
        [InlineData(typeof(long), BigQueryDbType.Integer, typeof(long))]
        [InlineData(typeof(float), BigQueryDbType.Float, typeof(float))]
        [InlineData(typeof(DateTime), BigQueryDbType.Timestamp, typeof(DateTime))]
        [InlineData(typeof(double), BigQueryDbType.Unknown, typeof(object))]
        [InlineData(typeof(object), BigQueryDbType.Unknown, typeof(object))]
        public void TypeAndBigQueryDbTypeTest(Type type1, BigQueryDbType bigQueryType, Type type2) {
            Assert.Equal(bigQueryType, BigQueryTypeConverter.ToBigQueryDbType(type1));
            Assert.Equal(type2, BigQueryTypeConverter.ToType(bigQueryType));
        }

        [Theory]
        [InlineData(typeof(string), DbType.String, typeof(string))]
        [InlineData(typeof(char), DbType.String, typeof(string))]
        [InlineData(typeof(bool), DbType.Boolean, typeof(bool))]
        [InlineData(typeof(long), DbType.Int64, typeof(long))]
        [InlineData(typeof(float), DbType.Single, typeof(float))]
        [InlineData(typeof(DateTime), DbType.DateTime, typeof(DateTime))]
        [InlineData(typeof(double), DbType.Object, typeof(object))]
        [InlineData(typeof(object), DbType.Object, typeof(object))]
        public void TypeAndDbTypeTest(Type type1, DbType dbType, Type type2) {
            Assert.Equal(dbType, BigQueryTypeConverter.ToDbType(type1));
            Assert.Equal(type2, BigQueryTypeConverter.ToType(dbType));
        }

        [Theory]
        [InlineData(default(long), DbType.Int64)]
        [InlineData(default(float), DbType.Single)]
        [InlineData(default(bool), DbType.Boolean)]
        [InlineData(default(string), DbType.String)]
        [InlineData(default(object), DbType.Object)]
        public void DefaultValueTest(object defaultValueFromSystem, DbType dbType) {
            Assert.Equal(defaultValueFromSystem, BigQueryTypeConverter.GetDefaultValueFor(dbType));
        }

        [Theory]
        [InlineData("STRING", typeof(string), BigQueryDbType.String)]
        [InlineData("INTEGER", typeof(long), BigQueryDbType.Integer)]
        [InlineData("FLOAT", typeof(float), BigQueryDbType.Float)]
        [InlineData("BOOLEAN", typeof(bool), BigQueryDbType.Boolean)]
        [InlineData("TIMESTAMP", typeof(DateTime), BigQueryDbType.Timestamp)]
        [InlineData("RECORD", typeof(object), BigQueryDbType.Unknown)]
        [InlineData("Foo", null, BigQueryDbType.Unknown)]
        [InlineData("123", null, BigQueryDbType.Unknown)]
        public void StringConvertTest(string stringType, Type systemType, BigQueryDbType bigQueryType) {
            Assert.Equal(systemType, BigQueryTypeConverter.ToType(stringType));
            Assert.Equal(bigQueryType, BigQueryTypeConverter.ToBigQueryDbType(stringType));
        }
    }
}
#endif