using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
            //new Tuple<DbType, BigQueryDbType>(, BigQueryDbType.Record),
        }; 

        static readonly List<Tuple<Type, DbType>> TypeToDbTypePairs = new List<Tuple<Type, DbType>>() {
            new Tuple<Type, DbType>(typeof(long), DbType.Int64),
            new Tuple<Type, DbType>(typeof(float), DbType.Single),
            new Tuple<Type, DbType>(typeof(string), DbType.String),
            new Tuple<Type, DbType>(typeof(DateTime), DbType.DateTime),
            new Tuple<Type, DbType>(typeof(bool), DbType.Boolean),
            //place for BigQueryRecord
        }; 

        public static BigQueryDbType ToBigQueryDbType(DbType type) {
            var typesTuple = DbTypeToBigQueryDbTypePairs.FirstOrDefault(i => i.Item1 == type);
            return typesTuple == null ? BigQueryDbType.Unknown : typesTuple.Item2;
        }

        public static BigQueryDbType ToBigQueryDbType(Type type) {
            return ToBigQueryDbType(ToDbType(type));
        }

        public static BigQueryDbType ToBigQueryDbType(string type) {
            switch(type) {
                case "STRING":
                   return BigQueryDbType.String;
                case "INTEGER":
                   return BigQueryDbType.Integer;
                case "FLOAT":
                   return BigQueryDbType.Float;
                case "BOOLEAN":
                   return BigQueryDbType.Boolean;
                case "TIMESTAMP":
                   return BigQueryDbType.Timestamp;
                case "RECORD":
                   return BigQueryDbType.Record;
            }
            return BigQueryDbType.Unknown;
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
            switch(type) {
                case "STRING":
                    return typeof(string);
                case "INTEGER":
                    return typeof(Int64);
                case "FLOAT":
                    return typeof(Single);
                case "BOOLEAN":
                    return typeof(bool);
                case "TIMESTAMP":
                    return typeof(DateTimeOffset);
                case "RECORD":
                    return typeof(object);
            }
            return null;    
        }

        public static Type ToType(DbType type) {
            var typesTuple = TypeToDbTypePairs.FirstOrDefault(i => i.Item2 == type);
            return typesTuple == null ? null : typesTuple.Item1;
        }

        public static object GetDefaultValueFor(DbType dbType) {
            var type = ToType(dbType);
            return type == null ? null : Activator.CreateInstance(type);
        }
    }
}
