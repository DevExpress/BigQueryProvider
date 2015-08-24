using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;

namespace DevExpress.DataAccess.BigQuery.Native {
    public static class BigQueryTypeConverter {
        static readonly List<Tuple<DbType, BigQueryDbType>> DbTypeToBigQueryDbTypePairs = new List<Tuple<DbType, BigQueryDbType>> {
            new Tuple<DbType, BigQueryDbType>(DbType.String, BigQueryDbType.String),
            new Tuple<DbType, BigQueryDbType>(DbType.Boolean, BigQueryDbType.Boolean),
            new Tuple<DbType, BigQueryDbType>(DbType.DateTime, BigQueryDbType.Timestamp),
            new Tuple<DbType, BigQueryDbType>(DbType.Single, BigQueryDbType.Float),
            new Tuple<DbType, BigQueryDbType>(DbType.Int64, BigQueryDbType.Integer),
            new Tuple<DbType, BigQueryDbType>(DbType.Object, BigQueryDbType.Unknown),
        }; 

        static readonly List<Tuple<Type, DbType>> TypeToDbTypePairs = new List<Tuple<Type, DbType>> {
            new Tuple<Type, DbType>(typeof(long), DbType.Int64),
            new Tuple<Type, DbType>(typeof(float), DbType.Single),
            new Tuple<Type, DbType>(typeof(string), DbType.String),
            new Tuple<Type, DbType>(typeof(char), DbType.String),
            new Tuple<Type, DbType>(typeof(DateTime), DbType.DateTime),
            new Tuple<Type, DbType>(typeof(bool), DbType.Boolean),
            new Tuple<Type, DbType>(typeof(object), DbType.Object),
            new Tuple<Type, DbType>(typeof(Int16), DbType.Int64),
            new Tuple<Type, DbType>(typeof(Int32), DbType.Int64),
            new Tuple<Type, DbType>(typeof(UInt32), DbType.Int64),
            new Tuple<Type, DbType>(typeof(UInt16), DbType.Int64),
        };
 
        static readonly List<Tuple<string, Type>> StringToTypePairs = new List<Tuple<string, Type>> {
            new Tuple<string, Type>("STRING", typeof(string)),
            new Tuple<string, Type>("INTEGER", typeof(Int64)),
            new Tuple<string, Type>("FLOAT", typeof(Single)),
            new Tuple<string, Type>("BOOLEAN", typeof(bool)),
            new Tuple<string, Type>("TIMESTAMP", typeof(DateTime)),
            new Tuple<string, Type>("RECORD", typeof(object))
        };

        static T GetFirstBy<T, T1>(this List<Tuple<T, T1>> list, T1 item2ToSearch, T @default) {
            var tuple = list.FirstOrDefault(li => li.Item2.Equals(item2ToSearch));
            return tuple == null ? @default : tuple.Item1;
        }

        static T1 GetSecondBy<T, T1>(this List<Tuple<T, T1>> list, T item1ToSearch, T1 @default) {
            var tuple = list.FirstOrDefault(li => li.Item1.Equals(item1ToSearch));
            return tuple == null ? @default : tuple.Item2;
        }

        public static BigQueryDbType ToBigQueryDbType(DbType dbType) {
            return GetSecondBy(DbTypeToBigQueryDbTypePairs, dbType, BigQueryDbType.Unknown);
        }

        public static BigQueryDbType ToBigQueryDbType(Type type) {
            return ToBigQueryDbType(ToDbType(type));
        }

        public static BigQueryDbType ToBigQueryDbType(string stringType) {
            return ToBigQueryDbType(ToType(stringType));
        }

        public static DbType ToDbType(BigQueryDbType bqDbType) {
            return GetFirstBy(DbTypeToBigQueryDbTypePairs, bqDbType, DbType.Object);
        }

        public static DbType ToDbType(Type type) {
            return GetSecondBy(TypeToDbTypePairs, type, DbType.Object);
        }

        public static Type ToType(BigQueryDbType bqDbType) {
            return ToType(ToDbType(bqDbType));
        }

        public static Type ToType(string stringType) {
            return GetSecondBy(StringToTypePairs, stringType, null);
        }

        public static Type ToType(DbType dbType) {
            return GetFirstBy(TypeToDbTypePairs, dbType, null);
        }

        public static object GetDefaultValueFor(DbType dbType) {
            var type = ToType(dbType);
            if(type == typeof(DateTime))
                return SqlDateTime.MinValue;
            return type == null ? null : (type.IsValueType ? Activator.CreateInstance(type) : null);
        }
    }
}
