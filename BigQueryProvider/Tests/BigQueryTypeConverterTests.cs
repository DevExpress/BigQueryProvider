/*
   Copyright 2015-2018 Developer Express Inc.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

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
        [InlineData(DbType.DateTime, BigQueryDbType.DateTime, DbType.DateTime)]
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
        [InlineData(typeof(DateTime), BigQueryDbType.DateTime, typeof(DateTime))]
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
        [InlineData("RECORD", typeof(object), BigQueryDbType.Record)]
        [InlineData("Foo", null, BigQueryDbType.Unknown)]
        [InlineData("123", null, BigQueryDbType.Unknown)]
        public void StringConvertTest(string stringType, Type systemType, BigQueryDbType bigQueryType) {
            Assert.Equal(systemType, BigQueryTypeConverter.ToType(stringType));
            Assert.Equal(bigQueryType, BigQueryTypeConverter.ToBigQueryDbType(stringType));
        }

        [Theory]
        [InlineData("DATE", typeof(DateTime), BigQueryDbType.Date, DbType.Date)]
        [InlineData("TIME", typeof(DateTime), BigQueryDbType.Time, DbType.Time)]
        [InlineData("DATETIME", typeof(DateTime), BigQueryDbType.DateTime, DbType.DateTime)]
        [InlineData("TIMESTAMP", typeof(DateTime), BigQueryDbType.Timestamp, DbType.DateTime)]
        public void TimesTest(string bigQueryTypeName, Type systemType, BigQueryDbType bigQueryDbType, DbType dbType) {
            Assert.Equal(systemType, BigQueryTypeConverter.ToType(bigQueryTypeName));
            Assert.Equal(bigQueryDbType, BigQueryTypeConverter.ToBigQueryDbType(bigQueryTypeName));
            Assert.Equal(dbType, BigQueryTypeConverter.ToDbType(bigQueryDbType));
        }
    }
}
#endif