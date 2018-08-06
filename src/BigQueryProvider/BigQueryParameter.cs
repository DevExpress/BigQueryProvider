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

using System;
using System.Data;
using System.Data.Common;
using System.Globalization;
using DevExpress.DataAccess.BigQuery.Native;

namespace DevExpress.DataAccess.BigQuery {
    /// <summary>
    /// A parameter to a BigQueryCommand.
    /// </summary>
    public sealed class BigQueryParameter : DbParameter, ICloneable {
        const int maxStringSize = 2097152;
        BigQueryDbType? bigQueryDbType;
        DbType? dbType;
        object value;
        ParameterDirection direction;
        int? size;

        /// <summary>
        /// Initializes a new instance of the BigQueryParameter class with default settings.
        /// </summary>
        public BigQueryParameter() {
            ResetDbType();
        }

        /// <summary>
        /// Initializes a new instance of the BigQueryParameter class with the specified settings.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        public BigQueryParameter(string parameterName, object value) : this() {
            ParameterName = parameterName;
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the BigQueryParameter class with the specified settings.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="dbType">A DBType enumeration value specifying the data type of the parameter.</param>
        public BigQueryParameter(string parameterName, DbType dbType) : this() {
            ParameterName = parameterName;
            DbType = dbType;
        }

        /// <summary>
        /// Initializes a new instance of the BigQueryParameter class with the specified settings.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="bigQueryDbType">A BigQueryDbType enumeration value specifying the data type of the parameter.</param>
        public BigQueryParameter(string parameterName, BigQueryDbType bigQueryDbType) : this() {
            ParameterName = parameterName;
            BigQueryDbType = bigQueryDbType;
        }

        /// <summary>
        ///  Specifies the type of the parameter specific to Big Query.
        /// </summary>
        /// <value>
        /// A BigQueryDbType enumeration value.
        /// </value>
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

        /// <summary>
        /// Specifies the DbType of the parameter.
        /// </summary>
        /// <value>
        /// A DBType enumeration value.
        /// </value>
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

        /// <summary>
        /// Specifies whether the parameter is read-only, write-only, bidirectional or a stored procedure return value. This implementation only supports ParameterDirrection.Input as its value.
        /// </summary>
        /// <value>
        ///  A ParameterDirrection enumeration value. 
        /// </value>
        public override ParameterDirection Direction {
            get => direction;
            set {
                if (value != ParameterDirection.Input)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Only input parameters are supported.");
            }
        }

        /// <summary>
        /// Specifies whether or not the parameter can receive DBNull as its value.
        /// </summary>
        /// <value>
        /// true, if the parameter can receive DBNull as its value; otherwise false. 
        /// </value>
        public override bool IsNullable {
            get => false;
            set {
                if(value)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Nullable parameters are not supported");
            }
        }

        /// <summary>
        /// Specifies the name of the parameter.
        /// </summary>
        /// <value>
        /// The name of the parameter. 
        /// </value>
        public override string ParameterName {
            get;
            set;
        }

        /// <summary>
        /// Specifies the name of the source column used for loading and returning the parameter's Value. 
        /// </summary>
        /// <value>
        /// The name of a source column.
        /// </value>
        public override string SourceColumn {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <summary>
        ///  Specifies the DataRowVersion of the source row to be used when obtaining the parameter's value.
        /// </summary>
        /// <value>
        /// A DataRowVersion enumeration value.
        /// </value>
        public override DataRowVersion SourceVersion {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// Specifies whether the source column of the current parameter is nullable.
        /// </summary>
        /// <value>
        /// true, if the source column is nullable; otherwise false. 
        /// </value>
        public override bool SourceColumnNullMapping {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// Specifies the value of the parameter.
        /// </summary>
        /// <value>
        /// an Object specifying the value of the parameter.
        /// </value>
        public override object Value {
            get => value ?? (value = BigQueryTypeConverter.GetDefaultValueFor(DbType));
            set => this.value = value;
        }

        /// <summary>
        /// Specifies the maximum size of data in the source column.
        /// </summary>
        /// <value>
        /// The size of data in the source column.
        /// </value>
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
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The value can not be less than 0");
                size = value;
            }
        }

        /// <summary>
        /// Resets the data type associated with the parameter.
        /// </summary>
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

        /// <summary>
        ///  Creates a shallow copy of the current BigQueryParameter.
        /// </summary>
        /// <returns>a shallow copy of the current BigQueryParameter.</returns>
        public BigQueryParameter Clone() {
            BigQueryParameter parameter = new BigQueryParameter(ParameterName, Value) {
                Direction = Direction,
                IsNullable = IsNullable,
                DbType = DbType,
                BigQueryDbType = BigQueryDbType,
                Size = Size
            };
            return parameter;
        }
    }
}
