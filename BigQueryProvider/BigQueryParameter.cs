using System;
using System.Data;
using System.Data.Common;
using System.Globalization;
using DevExpress.DataAccess.BigQuery.Native;

namespace DevExpress.DataAccess.BigQuery {
    public sealed class BigQueryParameter : DbParameter, ICloneable {
        const int maxStringSize = 2097152;
        BigQueryDbType? bigQueryDbType;
        DbType? dbType;
        object value;
        ParameterDirection direction;
        int size;

        public BigQueryParameter() {
            ResetDbType();
        }

        public BigQueryParameter(string parameterName, object value) : this() {
            ParameterName = parameterName;
            Value = value;
        }

        public BigQueryParameter(string parameterName, DbType dbType) : this() {
            ParameterName = parameterName;
            DbType = dbType;
        }

        public BigQueryParameter(string parameterName, DbType dbType, string sourceColumn)
            : this(parameterName, dbType) {
            SourceColumn = sourceColumn;
        }

        public BigQueryParameter(string parameterName, BigQueryDbType bigQueryDbType) : this() {
            ParameterName = parameterName;
            BigQueryDbType = bigQueryDbType;
        }

        public BigQueryParameter(string parameterName, BigQueryDbType bigQueryDbType, string sourceColumn)
            : this(parameterName, bigQueryDbType) {
            SourceColumn = sourceColumn;
        }

        public BigQueryDbType BigQueryDbType {
            get {
                if(bigQueryDbType.HasValue)
                    return bigQueryDbType.Value;
                if(value != null)
                    return BigQueryTypeConverter.ToBigQueryDbType(value.GetType());
                return BigQueryDbType.Unknown;
            }
            set {
                bigQueryDbType = value;
                dbType = BigQueryTypeConverter.ToDbType(value);
            }
        }

        public override DbType DbType {
            get {
                if(dbType.HasValue)
                    return dbType.Value;
                if(value != null)
                    return BigQueryTypeConverter.ToDbType(value.GetType());
                return DbType.Object;
            }
            set {
                dbType = value;
                bigQueryDbType = BigQueryTypeConverter.ToBigQueryDbType(value);
            } 
        }

        public override ParameterDirection Direction {
            get { return direction; }
            set {
                if (value != ParameterDirection.Input)
                    throw new ArgumentOutOfRangeException("value", value, "Only input parameters are supported.");
            }
        }

        public override bool IsNullable {
            get { return false; }
            set {
                if(value)
                    throw new ArgumentOutOfRangeException("value", value, "Nullable parameters are not supported");
            }
        }

        public override string ParameterName {
            get;
            set;
        }

        public override string SourceColumn {
            get;
            set;
        }

        public override DataRowVersion SourceVersion {
            get;
            set;
        }

        public override bool SourceColumnNullMapping {
            get;
            set;
        }

        public override object Value {
            get {
                return value ?? (value = BigQueryTypeConverter.GetDefaultValueFor(DbType));
            }
            set { this.value = value; }
        }

        public override int Size {
            get {
                if(size != 0)
                    return size;
                if(DbType == DbType.String) {
                    var stringValue = string.Format(CultureInfo.InvariantCulture, "{0}", value);
                    return stringValue.Length;
                }
                return 0;
            }
            set { size = value; }
        }

        public override void ResetDbType() {
            dbType = null;
            value = null;
            direction = ParameterDirection.Input;
            ParameterName = null;
            SourceColumn = null;
            SourceVersion = DataRowVersion.Current;
        }

        internal void Validate() {
            if(string.IsNullOrEmpty(ParameterName))
                throw new ArgumentException("Parameter's name is empty");
            if(Value == null || Value == DBNull.Value)
                throw new ArgumentException("Null parameter's values is not supported");
            if(BigQueryDbType == BigQueryDbType.Unknown)
                throw new NotSupportedException("Unsupported type for BigQuery: " + DbType);
            if(Size > maxStringSize)
                throw new ArgumentException("Exceeded the maximum permissible length of the value in " +  maxStringSize);
            try {
                Convert.ChangeType(Value, BigQueryTypeConverter.ToType(DbType));
            }
            catch(Exception) {
                throw new ArgumentException("Can't convert Value " + Value + " to DbType " + DbType);
            }
        }

        object ICloneable.Clone() {
            return Clone();
        }

        public BigQueryParameter Clone() {
            BigQueryParameter parameter = new BigQueryParameter(ParameterName, Value) {
                Direction = Direction,
                IsNullable = IsNullable,
                DbType = DbType,
                BigQueryDbType = BigQueryDbType,
                SourceColumnNullMapping = SourceColumnNullMapping,
                Size = Size,
                SourceColumn = SourceColumn,
                SourceVersion = SourceVersion,
            };
            return parameter;
        }
    }
}
