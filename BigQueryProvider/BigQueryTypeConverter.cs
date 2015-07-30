using System;
using System.Collections.Generic;
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
            //{BigQueryDbType.Float, DbType.Double},
            {BigQueryDbType.Integer, DbType.Int32},
            //{BigQueryDbType.Record,},
            {BigQueryDbType.String, DbType.String},
            {BigQueryDbType.Timestamp, DbType.DateTime},
        };
        private static readonly Dictionary<DbType, BigQueryDbType> DbTypeToBigQueryDbType = new Dictionary<DbType, BigQueryDbType>() {
            {DbType.Boolean, BigQueryDbType.Boolean},
            {DbType.Date, BigQueryDbType.Timestamp},
            {DbType.DateTime, BigQueryDbType.Timestamp},
            //{DbType.Double, BigQueryDbType.Float},
            {DbType.Int32, BigQueryDbType.Integer},
            {DbType.String, BigQueryDbType.String},
            {DbType.DateTime2, BigQueryDbType.Timestamp},
            {DbType.DateTimeOffset, BigQueryDbType.Timestamp},
        };
        private static readonly Dictionary<Type, BigQueryDbType> TypeToBigQueryDbType = new Dictionary<Type, BigQueryDbType>() {
            {typeof(bool), BigQueryDbType.Boolean},
            {typeof(float), BigQueryDbType.Float},
            {typeof(double), BigQueryDbType.Float},
            {typeof(int), BigQueryDbType.Integer},
            //{, BigQueryDbType.Record},
            {typeof(string), BigQueryDbType.String},
            {typeof(DateTime), BigQueryDbType.Timestamp},
        };
        private static readonly Dictionary<BigQueryDbType, Type> BigQueryDbTypeToType = new Dictionary<BigQueryDbType, Type>() {
            {BigQueryDbType.Boolean, typeof(bool)},
            {BigQueryDbType.Float, typeof(float)},
            {BigQueryDbType.Float, typeof(double)},
            {BigQueryDbType.Integer, typeof(int)},
            //{, BigQueryDbType.Record},
            {BigQueryDbType.String, typeof(string)},
            {BigQueryDbType.Timestamp, typeof(DateTime)},
        };

        public static BigQueryDbType ToBigQueryDbType(DbType type) {
            BigQueryDbType bigQueryDbTypeType;
            if(DbTypeToBigQueryDbType.TryGetValue(type, out bigQueryDbTypeType))
                return bigQueryDbTypeType;
            throw new NotSupportedException("Can't convert " + type + "for BigQueryDbType.");
        }

        public static BigQueryDbType ToBigQueryDbType(Type type) {
            BigQueryDbType bigQueryDbTypeType;
            if(TypeToBigQueryDbType.TryGetValue(type, out bigQueryDbTypeType))
                return bigQueryDbTypeType;
            throw new NotSupportedException("Can't convert " + type + "for BigQueryDbType.");
        }

        public static DbType ToDbType(BigQueryDbType type) {
            DbType dbType;
            if(BigQueryDbTypeToDbType.TryGetValue(type, out dbType))
                return dbType;
            throw new NotSupportedException("Can't convert " + type + "for DbType.");
        }

        public static DbType ToDbType(Type type) {
            DbType dbType;
            try {
                dbType = (DbType)Enum.Parse(typeof(DbType), type.Name);
            }
            catch {
                dbType = DbType.Object;
            }
            return dbType;    
        }
    }
}
