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

        [Test]
        public void TypeAndBigQueryDbTypeTest() {
            //Type -> BigQueryDbType
            Assert.AreEqual(BigQueryDbType.String, BigQueryTypeConverter.ToBigQueryDbType(typeof(string)));
            Assert.AreEqual(BigQueryDbType.Boolean, BigQueryTypeConverter.ToBigQueryDbType(typeof(bool)));
            Assert.AreEqual(BigQueryDbType.Integer, BigQueryTypeConverter.ToBigQueryDbType(typeof(Int64)));
            Assert.AreEqual(BigQueryDbType.Float, BigQueryTypeConverter.ToBigQueryDbType(typeof(Single)));
            Assert.AreEqual(BigQueryDbType.Timestamp, BigQueryTypeConverter.ToBigQueryDbType(typeof(DateTime)));
            Assert.AreEqual(BigQueryDbType.Unknown, BigQueryTypeConverter.ToBigQueryDbType(typeof(Double)));
            Assert.AreEqual(BigQueryDbType.Unknown, BigQueryTypeConverter.ToBigQueryDbType(typeof(object)));
            //BigQueryDbType -> Type
            Assert.AreEqual(typeof(string), BigQueryTypeConverter.ToType(BigQueryDbType.String));
            Assert.AreEqual(typeof(bool), BigQueryTypeConverter.ToType(BigQueryDbType.Boolean));
            Assert.AreEqual(typeof(Int64), BigQueryTypeConverter.ToType(BigQueryDbType.Integer));
            Assert.AreEqual(typeof(Single), BigQueryTypeConverter.ToType(BigQueryDbType.Float));
            Assert.AreEqual(typeof(DateTime), BigQueryTypeConverter.ToType(BigQueryDbType.Timestamp));
            Assert.AreEqual(typeof(object), BigQueryTypeConverter.ToType(BigQueryDbType.Unknown));
        }

        [Test]
        public void TypeAndDbTypeTest() {
            //Type -> DbType
            Assert.AreEqual(DbType.String, BigQueryTypeConverter.ToDbType(typeof(string)));
            Assert.AreEqual(DbType.Boolean, BigQueryTypeConverter.ToDbType(typeof(bool)));
            Assert.AreEqual(DbType.Int64, BigQueryTypeConverter.ToDbType(typeof(Int64)));
            Assert.AreEqual(DbType.Single, BigQueryTypeConverter.ToDbType(typeof(Single)));
            Assert.AreEqual(DbType.DateTime, BigQueryTypeConverter.ToDbType(typeof(DateTime)));
            Assert.AreEqual(DbType.Object, BigQueryTypeConverter.ToDbType(typeof(object)));
            //DbType -> Type
            Assert.AreEqual(typeof(string), BigQueryTypeConverter.ToType(DbType.String));
            Assert.AreEqual(typeof(bool), BigQueryTypeConverter.ToType(DbType.Boolean));
            Assert.AreEqual(typeof(Int64), BigQueryTypeConverter.ToType(DbType.Int64));
            Assert.AreEqual(typeof(Single), BigQueryTypeConverter.ToType(DbType.Single));
            Assert.AreEqual(typeof(DateTime), BigQueryTypeConverter.ToType(DbType.DateTime));
            Assert.AreEqual(typeof(object), BigQueryTypeConverter.ToType(DbType.Object));

        }
    }
}
