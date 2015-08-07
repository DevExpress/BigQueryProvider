#if DEBUGTEST
using System;
using System.Data;
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
        [InlineData(typeof(Int64), BigQueryDbType.Integer, typeof(Int64))]
        [InlineData(typeof(Single), BigQueryDbType.Float, typeof(Single))]
        [InlineData(typeof(DateTime), BigQueryDbType.Timestamp, typeof(DateTime))]
        [InlineData(typeof(double), BigQueryDbType.Unknown, typeof(object))]
        [InlineData(typeof(object), BigQueryDbType.Unknown, typeof(object))]
        public void TypeAndBigQueryDbTypeTest(Type type1, BigQueryDbType bigQueryType, Type type2) {
            Assert.Equal(bigQueryType, BigQueryTypeConverter.ToBigQueryDbType(type1));
            Assert.Equal(type2, BigQueryTypeConverter.ToType(bigQueryType));
        }

        [Theory]
        [InlineData(typeof(string), DbType.String, typeof(string))]
        [InlineData(typeof(bool), DbType.Boolean, typeof(bool))]
        [InlineData(typeof(Int64), DbType.Int64, typeof(Int64))]
        [InlineData(typeof(Single), DbType.Single, typeof(Single))]
        [InlineData(typeof(DateTime), DbType.DateTime, typeof(DateTime))]
        [InlineData(typeof(double), DbType.Object, typeof(object))]
        [InlineData(typeof(object), DbType.Object, typeof(object))]
        public void TypeAndDbTypeTest(Type type1, DbType dbType, Type type2) {
            Assert.Equal(dbType, BigQueryTypeConverter.ToDbType(type1));
            Assert.Equal(type2, BigQueryTypeConverter.ToType(dbType));
        }

        [Fact]
        public void DefaultValueTest() {
            Assert.Equal(default(long), BigQueryTypeConverter.GetDefaultValueFor(DbType.Int64));
            Assert.Equal(default(Single), BigQueryTypeConverter.GetDefaultValueFor(DbType.Single));
            Assert.Equal(default(bool), BigQueryTypeConverter.GetDefaultValueFor(DbType.Boolean));
            Assert.Equal(default(String), BigQueryTypeConverter.GetDefaultValueFor(DbType.String));
            Assert.Equal(System.Data.SqlTypes.SqlDateTime.MinValue, BigQueryTypeConverter.GetDefaultValueFor(DbType.DateTime));
            Assert.Equal(default(object), BigQueryTypeConverter.GetDefaultValueFor(DbType.Object));
        }

        [Theory]
        [InlineData("STRING", typeof(string), BigQueryDbType.String)]
        [InlineData("INTEGER", typeof(long), BigQueryDbType.Integer)]
        [InlineData("FLOAT", typeof(Single), BigQueryDbType.Float)]
        [InlineData("BOOLEAN", typeof(bool), BigQueryDbType.Boolean)]
        [InlineData("TIMESTAMP", typeof(DateTime), BigQueryDbType.Timestamp)]
        [InlineData("RECORD", typeof(object), BigQueryDbType.Unknown)] //until Record is not implemented
        [InlineData("DFSD", null, BigQueryDbType.Unknown)]
        [InlineData("124", null, BigQueryDbType.Unknown)]
        public void StringConvertTest(string stringType, Type systemType, BigQueryDbType bigQueryType) {
            Assert.Equal(systemType, BigQueryTypeConverter.ToType(stringType));
            Assert.Equal(bigQueryType, BigQueryTypeConverter.ToBigQueryDbType(stringType));
        }
    }
}
#endif