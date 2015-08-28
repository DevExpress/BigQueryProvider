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
        int? size;

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
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override DataRowVersion SourceVersion {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override bool SourceColumnNullMapping {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override object Value {
            get {
                return value ?? (value = BigQueryTypeConverter.GetDefaultValueFor(DbType));
            }
            set { this.value = value; }
        }

        public override int Size {
            get {
                if(size.HasValue)
                    return size.Value;
                if(DbType != DbType.String) return 0;
                var invariantString = value.ToInvariantString();
                return invariantString.Length;
            }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", value, "The value can not be less than 0");
                size = value;
            }
        }

        public override void ResetDbType() {
            dbType = null;
            value = null;
            direction = ParameterDirection.Input;
            ParameterName = null;
        }

        internal void Validate() {
            if(string.IsNullOrEmpty(ParameterName))
                throw new ArgumentException("Parameter's name is empty");
            if(Value == null || Value == DBNull.Value)
                throw new ArgumentException("Null parameter's values is not supported");
            if(BigQueryDbType == BigQueryDbType.Unknown)
                throw new NotSupportedException("Unsupported type for BigQuery: " + DbType);
            if(Size > maxStringSize)
                throw new ArgumentException("Value's length in " + Size + " greater than max length in " +  maxStringSize);
            try {
                Convert.ChangeType(Value, BigQueryTypeConverter.ToType(DbType), CultureInfo.InvariantCulture);
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
                Size = Size,
            };
            return parameter;
        }
    }
}
