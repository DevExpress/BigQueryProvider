using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using log4net.Filter;

namespace DevExpress.DataAccess.BigQuery {
    public enum BigQueryDbType {
        String,
        Integer,
        Float,
        Boolean,
        Record,
        Timestamp,
        Unknown,
    }

    internal static class BigQueryTypeConverter {
        static readonly List<Tuple<DbType, BigQueryDbType>> DbTypeToBigQueryDbTypePairs = new List<Tuple<DbType, BigQueryDbType>>() {
            new Tuple<DbType, BigQueryDbType>(DbType.String, BigQueryDbType.String),
            new Tuple<DbType, BigQueryDbType>(DbType.Boolean, BigQueryDbType.Boolean),
            new Tuple<DbType, BigQueryDbType>(DbType.DateTime, BigQueryDbType.Timestamp),
            new Tuple<DbType, BigQueryDbType>(DbType.Single, BigQueryDbType.Float),
            new Tuple<DbType, BigQueryDbType>(DbType.Int64, BigQueryDbType.Integer),
            new Tuple<DbType, BigQueryDbType>(DbType.Object, BigQueryDbType.Unknown),
            //new Tuple<DbType, BigQueryDbType>(, BigQueryDbType.Record),
        }; 

        static readonly List<Tuple<Type, DbType>> TypeToDbTypePairs = new List<Tuple<Type, DbType>>() {
            new Tuple<Type, DbType>(typeof(long), DbType.Int64),
            new Tuple<Type, DbType>(typeof(float), DbType.Single),
            new Tuple<Type, DbType>(typeof(string), DbType.String),
            new Tuple<Type, DbType>(typeof(DateTime), DbType.DateTime),
            new Tuple<Type, DbType>(typeof(bool), DbType.Boolean),
            new Tuple<Type, DbType>(typeof(object), DbType.Object),
            new Tuple<Type, DbType>(typeof(Int16), DbType.Int64),
            new Tuple<Type, DbType>(typeof(Int32), DbType.Int64),
            new Tuple<Type, DbType>(typeof(UInt32), DbType.Int64),
            new Tuple<Type, DbType>(typeof(UInt16), DbType.Int64),
            //place for BigQueryRecord
        };
 
        static readonly List<Tuple<string, Type>> StringToTypePairs = new List<Tuple<string, Type>>() {
            new Tuple<string, Type>("STRING", typeof(string)),
            new Tuple<string, Type>("INTEGER", typeof(Int64)),
            new Tuple<string, Type>("FLOAT", typeof(Single)),
            new Tuple<string, Type>("BOOLEAN", typeof(bool)),
            new Tuple<string, Type>("TIMESTAMP", typeof(DateTime)),
            new Tuple<string, Type>("RECORD", typeof(object))
        }; 

        public static BigQueryDbType ToBigQueryDbType(DbType type) {
            var typesTuple = DbTypeToBigQueryDbTypePairs.FirstOrDefault(i => i.Item1 == type);
            return typesTuple == null ? BigQueryDbType.Unknown : typesTuple.Item2;
        }

        public static BigQueryDbType ToBigQueryDbType(Type type) {
            return ToBigQueryDbType(ToDbType(type));
        }

        public static BigQueryDbType ToBigQueryDbType(string stringType) {
            var type = ToType(stringType);
            return type == null ? BigQueryDbType.Unknown : ToBigQueryDbType(type);
        }

        public static DbType ToDbType(BigQueryDbType type) {
            var typesTuple = DbTypeToBigQueryDbTypePairs.FirstOrDefault(i => i.Item2 == type);
            return typesTuple == null ? DbType.Object : typesTuple.Item1;
        }

        public static DbType ToDbType(Type type) {
            var typesTuple = TypeToDbTypePairs.FirstOrDefault(i => i.Item1 == type);
            return typesTuple == null ? DbType.Object : typesTuple.Item2;
        }

        public static Type ToType(BigQueryDbType type) {
            return ToType(ToDbType(type));
        }

        public static Type ToType(string type) {
            var typesTuple = StringToTypePairs.FirstOrDefault(i => i.Item1 == type);
            return typesTuple == null ? null : typesTuple.Item2;
        }

        public static Type ToType(DbType type) {
            var typesTuple = TypeToDbTypePairs.FirstOrDefault(i => i.Item2 == type);
            return typesTuple == null ? null : typesTuple.Item1;
        }

        public static object GetDefaultValueFor(DbType dbType) {
            var type = ToType(dbType);
            if(type == typeof(DateTime))
                return System.Data.SqlTypes.SqlDateTime.MinValue;
            return type == null ? null : (type.IsValueType ? Activator.CreateInstance(type) : null);
        }
    }
}
