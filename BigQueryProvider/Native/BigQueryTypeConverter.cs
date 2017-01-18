/*
   Copyright 2015 Developer Express Inc.

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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;

namespace DevExpress.DataAccess.BigQuery.Native {
    public static class BigQueryTypeConverter {
        static readonly Tuple<DbType, BigQueryDbType>[] DbTypeToBigQueryDbTypePairs = {
            new Tuple<DbType, BigQueryDbType>(DbType.String, BigQueryDbType.String),
            new Tuple<DbType, BigQueryDbType>(DbType.Date, BigQueryDbType.Date),
            new Tuple<DbType, BigQueryDbType>(DbType.Time, BigQueryDbType.Time),
            new Tuple<DbType, BigQueryDbType>(DbType.DateTime, BigQueryDbType.DateTime),
            new Tuple<DbType, BigQueryDbType>(DbType.Boolean, BigQueryDbType.Boolean),
            new Tuple<DbType, BigQueryDbType>(DbType.DateTime, BigQueryDbType.Timestamp),
            new Tuple<DbType, BigQueryDbType>(DbType.Single, BigQueryDbType.Float),
            new Tuple<DbType, BigQueryDbType>(DbType.Int64, BigQueryDbType.Integer),
            new Tuple<DbType, BigQueryDbType>(DbType.Object, BigQueryDbType.Unknown),
        }; 

        static readonly Tuple<Type, DbType>[] TypeToDbTypePairs = {
            new Tuple<Type, DbType>(typeof(long), DbType.Int64),
            new Tuple<Type, DbType>(typeof(float), DbType.Single),
            new Tuple<Type, DbType>(typeof(string), DbType.String),
            new Tuple<Type, DbType>(typeof(char), DbType.String),
            new Tuple<Type, DbType>(typeof(DateTime), DbType.DateTime),
            new Tuple<Type, DbType>(typeof(bool), DbType.Boolean),
            new Tuple<Type, DbType>(typeof(object), DbType.Object),
            new Tuple<Type, DbType>(typeof(short), DbType.Int64),
            new Tuple<Type, DbType>(typeof(int), DbType.Int64),
            new Tuple<Type, DbType>(typeof(uint), DbType.Int64),
            new Tuple<Type, DbType>(typeof(ushort), DbType.Int64),
        };
 
        static readonly Tuple<string, Type>[] StringToTypePairs = {
            new Tuple<string, Type>("STRING", typeof(string)),
            new Tuple<string, Type>("DATE", typeof(DateTime)),
            new Tuple<string, Type>("TIME", typeof(DateTime)),
            new Tuple<string, Type>("DATETIME", typeof(DateTime)),
            new Tuple<string, Type>("INTEGER", typeof(long)),
            new Tuple<string, Type>("FLOAT", typeof(float)),
            new Tuple<string, Type>("BOOLEAN", typeof(bool)),
            new Tuple<string, Type>("TIMESTAMP", typeof(DateTime)),
            new Tuple<string, Type>("RECORD", typeof(object))
        };

        static readonly Tuple<string, BigQueryDbType>[] StringToBigQueryDbTypePairs = {
            new Tuple<string, BigQueryDbType>("STRING", BigQueryDbType.String),
            new Tuple<string, BigQueryDbType>("DATE", BigQueryDbType.Date),
            new Tuple<string, BigQueryDbType>("TIME", BigQueryDbType.Time),
            new Tuple<string, BigQueryDbType>("DATETIME", BigQueryDbType.DateTime),
            new Tuple<string, BigQueryDbType>("INTEGER", BigQueryDbType.Integer),
            new Tuple<string, BigQueryDbType>("FLOAT", BigQueryDbType.Float),
            new Tuple<string, BigQueryDbType>("BOOLEAN", BigQueryDbType.Boolean),
            new Tuple<string, BigQueryDbType>("TIMESTAMP", BigQueryDbType.Timestamp),
            new Tuple<string, BigQueryDbType>("RECORD", BigQueryDbType.Record),
            new Tuple<string, BigQueryDbType>("BYTES", BigQueryDbType.Bytes)
        };

        public static BigQueryDbType ToBigQueryDbType(DbType dbType) {
            return (BigQueryDbType) (DbTypeToBigQueryDbTypePairs.FindPairFor(dbType, GetSecond) ?? BigQueryDbType.Unknown);
        }

        public static BigQueryDbType ToBigQueryDbType(Type type) {
            return ToBigQueryDbType(ToDbType(type));
        }

        public static BigQueryDbType ToBigQueryDbType(string stringType) {
            return (BigQueryDbType)(StringToBigQueryDbTypePairs.FindPairFor(stringType, GetSecond) ?? BigQueryDbType.Unknown);
        }

        public static DbType ToDbType(BigQueryDbType bqDbType) {
            return (DbType) (DbTypeToBigQueryDbTypePairs.FindPairFor(bqDbType, GetFirst) ?? DbType.Object);
        }

        public static DbType ToDbType(Type type) {
            return (DbType) (TypeToDbTypePairs.FindPairFor(type, GetSecond) ?? DbType.Object);
        }

        public static Type ToType(BigQueryDbType bqDbType) {
            return ToType(ToDbType(bqDbType));
        }

        public static Type ToType(string stringType) {
            return (Type) (StringToTypePairs.FindPairFor(stringType, GetSecond));
        }

        public static Type ToType(DbType dbType) {
            return (Type) (TypeToDbTypePairs.FindPairFor(dbType, GetFirst));
        }

        public static object GetDefaultValueFor(DbType dbType) {
            if(dbType == DbType.DateTime)
                return SqlDateTime.MinValue;
            var type = ToType(dbType);
            return type == null ? null : (type.IsValueType ? Activator.CreateInstance(type) : null);
        }

        static object FindPairFor<T1, T2>(this IEnumerable<Tuple<T1, T2>> tuples, object knownItem, Func<Tuple<T1, T2>, object> selector) {
            var tuple = tuples.FirstOrDefault(t => t.Item1.Equals(knownItem) || t.Item2.Equals(knownItem));
            return tuple == null ? null : selector(tuple);
        }

        static object GetFirst<T1, T2>(Tuple<T1, T2> tuple) {
            return tuple.Item1;
        }

        static object GetSecond<T1, T2>(Tuple<T1, T2> tuple) {
            return tuple.Item2;
        }
    }
}
