using System;
using System.Data;
using NUnit.Framework;

namespace DevExpress.DataAccess.BigQuery.Tests {
    class BigQueryTypeConverterTest {
        [TestCase(DbType.String, BigQueryDbType.String, DbType.String)]
        [TestCase(DbType.Boolean, BigQueryDbType.Boolean, DbType.Boolean)]
        [TestCase(DbType.Int64, BigQueryDbType.Integer, DbType.Int64)]
        [TestCase(DbType.Single, BigQueryDbType.Float, DbType.Single)]
        [TestCase(DbType.DateTime, BigQueryDbType.Timestamp, DbType.DateTime)]
        [TestCase(DbType.Double, BigQueryDbType.Unknown, DbType.Object)]
        [TestCase(DbType.Object, BigQueryDbType.Unknown, DbType.Object)]
        public void DbTypeAndBigQueryDbTypeTest(DbType dbType1, BigQueryDbType bigQueryType, DbType dbType2) {
            Assert.AreEqual(bigQueryType, BigQueryTypeConverter.ToBigQueryDbType(dbType1));
            Assert.AreEqual(dbType2, BigQueryTypeConverter.ToDbType(bigQueryType));
        }

        [TestCase(typeof(string), BigQueryDbType.String, typeof(string))]
        [TestCase(typeof(bool), BigQueryDbType.Boolean, typeof(bool))]
        [TestCase(typeof(Int64), BigQueryDbType.Integer, typeof(Int64))]
        [TestCase(typeof(Single), BigQueryDbType.Float, typeof(Single))]
        [TestCase(typeof(DateTime), BigQueryDbType.Timestamp, typeof(DateTime))]
        [TestCase(typeof(double), BigQueryDbType.Unknown, typeof(object))]
        [TestCase(typeof(object), BigQueryDbType.Unknown, typeof(object))]
        public void TypeAndBigQueryDbTypeTest(Type type1, BigQueryDbType bigQueryType, Type type2) {
            Assert.AreEqual(bigQueryType, BigQueryTypeConverter.ToBigQueryDbType(type1));
            Assert.AreEqual(type2, BigQueryTypeConverter.ToType(bigQueryType));
        }

        [TestCase(typeof(string), DbType.String, typeof(string))]
        [TestCase(typeof(bool), DbType.Boolean, typeof(bool))]
        [TestCase(typeof(Int64), DbType.Int64, typeof(Int64))]
        [TestCase(typeof(Single), DbType.Single, typeof(Single))]
        [TestCase(typeof(DateTime), DbType.DateTime, typeof(DateTime))]
        [TestCase(typeof(double), DbType.Object, typeof(object))]
        [TestCase(typeof(object), DbType.Object, typeof(object))]
        public void TypeAndDbTypeTest(Type type1, DbType dbType, Type type2) {
            Assert.AreEqual(dbType, BigQueryTypeConverter.ToDbType(type1));
            Assert.AreEqual(type2, BigQueryTypeConverter.ToType(dbType));
        }

        [Test]
        public void DefaultValueTest() {
            Assert.AreEqual(default(long), BigQueryTypeConverter.GetDefaultValueFor(DbType.Int64));
            Assert.AreEqual(default(Single), BigQueryTypeConverter.GetDefaultValueFor(DbType.Single));
            Assert.AreEqual(default(bool), BigQueryTypeConverter.GetDefaultValueFor(DbType.Boolean));
            Assert.AreEqual(default(String), BigQueryTypeConverter.GetDefaultValueFor(DbType.String));
            Assert.AreEqual(System.Data.SqlTypes.SqlDateTime.MinValue, BigQueryTypeConverter.GetDefaultValueFor(DbType.DateTime));
            Assert.AreEqual(default(object), BigQueryTypeConverter.GetDefaultValueFor(DbType.Object));
        }

        [TestCase("STRING", typeof(string), BigQueryDbType.String)]
        [TestCase("INTEGER", typeof(long), BigQueryDbType.Integer)]
        [TestCase("FLOAT", typeof(Single), BigQueryDbType.Float)]
        [TestCase("BOOLEAN", typeof(bool), BigQueryDbType.Boolean)]
        [TestCase("TIMESTAMP", typeof(DateTime), BigQueryDbType.Timestamp)]
        [TestCase("RECORD", typeof(object), BigQueryDbType.Unknown)] //until Record is not implemented
        [TestCase("DFSD", null, BigQueryDbType.Unknown)]
        [TestCase("124", null, BigQueryDbType.Unknown)]
        public void StringConvertTest(string stringType, Type systemType, BigQueryDbType bigQueryType) {
            Assert.AreEqual(systemType, BigQueryTypeConverter.ToType(stringType));
            Assert.AreEqual(bigQueryType, BigQueryTypeConverter.ToBigQueryDbType(stringType));
        }
    }
}
