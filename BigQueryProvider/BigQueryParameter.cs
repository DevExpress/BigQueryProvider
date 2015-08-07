﻿using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;

namespace DevExpress.DataAccess.BigQuery {
    public sealed class BigQueryParameter : DbParameter, ICloneable {
        public BigQueryParameter() {
            ResetDbType();
        }

        public BigQueryParameter(string parameterName, object value) {
            ParameterName = parameterName;
            Value = value;
        }

        public BigQueryParameter(string parameterName, DbType dbType) {
            ParameterName = parameterName;
            DbType = dbType;
        }

        public BigQueryParameter(string parameterName, DbType dbType, string sourceColumn)
            : this(parameterName, dbType) {
            SourceColumn = sourceColumn;
        }

        BigQueryDbType? bigQueryDbType;
        DbType? dbType;
        object value;

        public override void ResetDbType() {
            dbType = null;
            value = null;
            ParameterName = null;
            IsNullable = true;
            SourceColumn = null;
            SourceVersion = DataRowVersion.Current;
            Direction = ParameterDirection.Input;
        }

        internal void Validate() {
            if(string.IsNullOrEmpty(ParameterName))
                throw new ArgumentException("Parameter's name is empty");
            if (Value == null)
                throw new ArgumentException("Parameter's value is not initialized");
            if(BigQueryDbType == BigQueryDbType.Unknown)
                throw new NotSupportedException("Unsupported type for BigQuery: " + DbType);
            if (BigQueryTypeConverter.ToType(DbType) ==  null)
                throw new NotSupportedException("This DbType " + DbType + " is unsupported by BigQuery");
            try {
                Convert.ChangeType(Value, BigQueryTypeConverter.ToType(DbType));
            }
            catch(Exception) {
                throw new ArgumentException("Can't convert Value " + Value + " to DbType " + DbType);
            }
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
            get;
            set;
        }

        public override bool IsNullable {
            get;
            set;
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

        public override object Value {
            get {
                return value ?? (value = BigQueryTypeConverter.GetDefaultValueFor(DbType));
            }
            set { this.value = value; }
        }

        public override bool SourceColumnNullMapping {
            get;
            set;
        }

        public override int Size {
            get;
            set;
        }

        public BigQueryParameter Clone() {
            BigQueryParameter parameter = new BigQueryParameter(ParameterName, Value) {
                Direction = Direction,
                DbType = DbType
            };
            return parameter;
        }

        object ICloneable.Clone() {
            return Clone();
        }
    }
}
