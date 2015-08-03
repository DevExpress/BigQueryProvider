using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private static readonly Dictionary<BigQueryDbType, DbType> BigQueryDbTypeToDbType = new Dictionary<BigQueryDbType, DbType>() {
            {BigQueryDbType.Boolean, DbType.Boolean},
            {BigQueryDbType.Float, DbType.Single},
            {BigQueryDbType.Integer, DbType.Int64},
            //{BigQueryDbType.Record,},
            {BigQueryDbType.String, DbType.String},
            {BigQueryDbType.Timestamp, DbType.DateTime},
        };
        private static readonly Dictionary<DbType, BigQueryDbType> DbTypeToBigQueryDbType = new Dictionary<DbType, BigQueryDbType>() {
            {DbType.Boolean, BigQueryDbType.Boolean},
            {DbType.Date, BigQueryDbType.Timestamp},
            {DbType.DateTime, BigQueryDbType.Timestamp},
            {DbType.Single, BigQueryDbType.Float},
            {DbType.Int64, BigQueryDbType.Integer},
            {DbType.String, BigQueryDbType.String},
            {DbType.DateTime2, BigQueryDbType.Timestamp},
            {DbType.DateTimeOffset, BigQueryDbType.Timestamp},
        };

        private static readonly List<Tuple<BigQueryDbType, Type>> BigQueryDbTypeToTypePairs = new List<Tuple<BigQueryDbType, Type>>() {
            new Tuple<BigQueryDbType, Type>(BigQueryDbType.Boolean, typeof(bool)),
            new Tuple<BigQueryDbType, Type>(BigQueryDbType.Float, typeof(float)),
            new Tuple<BigQueryDbType, Type>(BigQueryDbType.Integer, typeof(long)),
            //new Tuple<BigQueryDbType, Type>(BigQueryDbType.Record, ),
            new Tuple<BigQueryDbType, Type>(BigQueryDbType.String, typeof(string)),
            new Tuple<BigQueryDbType, Type>(BigQueryDbType.Timestamp, typeof(DateTime))
        };
        private static readonly Dictionary<Type, DbType> TypeToDbType = new Dictionary<Type, DbType>() {
            {typeof(byte), DbType.Byte},
            {typeof(sbyte), DbType.SByte},
            {typeof(short), DbType.Int16},
            {typeof(ushort), DbType.UInt16},
            {typeof(int), DbType.Int32},
            {typeof(uint), DbType.UInt32},
            {typeof(long), DbType.Int64},
            {typeof(ulong), DbType.UInt64},
            {typeof(float), DbType.Single},
            {typeof(double), DbType.Double},
            {typeof(decimal), DbType.Decimal},
            {typeof(bool), DbType.Boolean},
            {typeof(string), DbType.String},
            {typeof(char), DbType.StringFixedLength},
            {typeof(Guid), DbType.Guid},
            {typeof(DateTime), DbType.DateTime},
            {typeof(DateTimeOffset), DbType.DateTimeOffset},
            {typeof(byte[]), DbType.Binary},
            {typeof(byte?), DbType.Byte},
            {typeof(sbyte?), DbType.SByte},
            {typeof(short?), DbType.Int16},
            {typeof(ushort?), DbType.UInt16},
            {typeof(int?), DbType.Int32},
            {typeof(uint?), DbType.UInt32},
            {typeof(long?), DbType.Int64},
            {typeof(ulong?), DbType.UInt64},
            {typeof(float?), DbType.Single},
            {typeof(double?), DbType.Double},
            {typeof(decimal?), DbType.Decimal},
            {typeof(bool?), DbType.Boolean},
            {typeof(char?), DbType.StringFixedLength},
            {typeof(Guid?), DbType.Guid},
            {typeof(DateTime?), DbType.DateTime},
            {typeof(DateTimeOffset?), DbType.DateTimeOffset},
        }; 

        public static BigQueryDbType ToBigQueryDbType(DbType type) {
            BigQueryDbType bigQueryDbTypeType;
            if(DbTypeToBigQueryDbType.TryGetValue(type, out bigQueryDbTypeType))
                return bigQueryDbTypeType;
            throw new NotSupportedException("Can't convert " + type + "to BigQueryDbType.");
        }

        public static BigQueryDbType ToBigQueryDbType(Type type) {
            var typesTuple = BigQueryDbTypeToTypePairs.FirstOrDefault(i => i.Item2 == type);
            return typesTuple == null ? BigQueryDbType.Unknown : typesTuple.Item1;
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
            DbType dbType;
            if(BigQueryDbTypeToDbType.TryGetValue(type, out dbType))
                return dbType;
            throw new NotSupportedException("Can't convert " + type + "to DbType.");
        }

        public static DbType ToDbType(Type type) {
            DbType dbType;
            if(TypeToDbType.TryGetValue(type, out dbType))
                return dbType;
            throw new NotSupportedException("Can't convert " + type + "to DbType.");
        }

        public static Type ToType(BigQueryDbType type) {
            var typesTuple = BigQueryDbTypeToTypePairs.FirstOrDefault(i => i.Item1 == type);
            return typesTuple == null ? null : typesTuple.Item2;
        }
    }
}
