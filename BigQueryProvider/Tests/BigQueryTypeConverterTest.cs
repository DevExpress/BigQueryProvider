using System;
using System.Data;
using NUnit.Framework;

namespace DevExpress.DataAccess.BigQuery.Tests {
    class BigQueryTypeConverterTest {
        //Need in BigQueryDbType.Record tests

        //type1 convert to -> type2 convert to -> type3
        [TestCase(DbType.String, BigQueryDbType.String, DbType.String)]
        [TestCase(DbType.Boolean, BigQueryDbType.Boolean, DbType.Boolean)]
        [TestCase(DbType.Int64, BigQueryDbType.Integer, DbType.Int64)]
        [TestCase(DbType.Single, BigQueryDbType.Float, DbType.Single)]
        [TestCase(DbType.DateTime, BigQueryDbType.Timestamp, DbType.DateTime)]
        [TestCase(DbType.Double, BigQueryDbType.Unknown, DbType.Object)]
        [TestCase(DbType.Object, BigQueryDbType.Unknown, DbType.Object)]
        public void DbTypeAndBigQueryDbTypeTest(DbType d1, BigQueryDbType b, DbType d2) {
            //DbType -> BigQueryDbType
            Assert.AreEqual(b, BigQueryTypeConverter.ToBigQueryDbType(d1));
            //BigQueryDbType -> DbType
            Assert.AreEqual(d2, BigQueryTypeConverter.ToDbType(b));
        }

        [TestCase(typeof(string), BigQueryDbType.String, typeof(string))]
        [TestCase(typeof(bool), BigQueryDbType.Boolean, typeof(bool))]
        [TestCase(typeof(Int64), BigQueryDbType.Integer, typeof(Int64))]
        [TestCase(typeof(Single), BigQueryDbType.Float, typeof(Single))]
        [TestCase(typeof(DateTime), BigQueryDbType.Timestamp, typeof(DateTime))]
        [TestCase(typeof(double), BigQueryDbType.Unknown, typeof(object))]
        [TestCase(typeof(object), BigQueryDbType.Unknown, typeof(object))]
        public void TypeAndBigQueryDbTypeTest(Type t1, BigQueryDbType b, Type t2) {
            //Type -> BigQueryDbType
            Assert.AreEqual(b, BigQueryTypeConverter.ToBigQueryDbType(t1));
            //BigQueryDbType -> Type
            Assert.AreEqual(t2, BigQueryTypeConverter.ToType(b));
        }

        [TestCase(typeof(string), DbType.String, typeof(string))]
        [TestCase(typeof(bool), DbType.Boolean, typeof(bool))]
        [TestCase(typeof(Int64), DbType.Int64, typeof(Int64))]
        [TestCase(typeof(Single), DbType.Single, typeof(Single))]
        [TestCase(typeof(DateTime), DbType.DateTime, typeof(DateTime))]
        [TestCase(typeof(double), DbType.Object, typeof(object))]
        [TestCase(typeof(object), DbType.Object, typeof(object))]
        public void TypeAndDbTypeTest(Type t1, DbType d, Type t2) {
            //Type -> DbType
            Assert.AreEqual(d, BigQueryTypeConverter.ToDbType(t1));
            //DbType -> Type
            Assert.AreEqual(t2, BigQueryTypeConverter.ToType(d));
        }

        [Test]
        public void DefaultValueTest() {
            Assert.AreEqual(0, BigQueryTypeConverter.GetDefaultValueFor(DbType.Int64));
            Assert.AreEqual(0, BigQueryTypeConverter.GetDefaultValueFor(DbType.Single));
            Assert.AreEqual(false, BigQueryTypeConverter.GetDefaultValueFor(DbType.Boolean));
            Assert.AreEqual(null, BigQueryTypeConverter.GetDefaultValueFor(DbType.String));
            Assert.AreEqual(default(DateTime), BigQueryTypeConverter.GetDefaultValueFor(DbType.DateTime));
            Assert.AreEqual(null, BigQueryTypeConverter.GetDefaultValueFor(DbType.Object));
        }
    }
}
