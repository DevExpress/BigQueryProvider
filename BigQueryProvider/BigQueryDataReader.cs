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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.DataAccess.BigQuery.Native;
using Google;
using Google.Apis.Bigquery.v2;
using Google.Apis.Bigquery.v2.Data;

namespace DevExpress.DataAccess.BigQuery {
    public class BigQueryDataReader : DbDataReader {
        #region static
        static string PrepareCommandText(BigQueryCommand command) {
            return command.CommandType == CommandType.TableDirect 
                ? string.Format("SELECT * FROM [{0}.{1}]", command.Connection.DataSetId, command.CommandText) 
                : command.CommandText;
        }

        static string PrepareParameterValue(BigQueryParameter parameter) {
            if(parameter.BigQueryDbType == BigQueryDbType.String) {
                var invariantString = parameter.Value.ToInvariantString();
                var trimmedString = invariantString.Substring(0, parameter.Size);
                var escapedString = EscapeString(trimmedString);
                return string.Format("'{0}'", escapedString);
            }
            string format = parameter.BigQueryDbType == BigQueryDbType.Timestamp 
                ? "TIMESTAMP('{0:u}')" 
                : "{0}";
            return parameter.Value.ToInvariantString(format);
        }

        static string EscapeString(string invariantString) {
            return invariantString
                .Replace(@"\", @"\\")
                .Replace("'", @"\'")
                .Replace("\"", @"""");
        }

        static DateTime UnixTimeStampToDateTime(object timestamp) {
            if(timestamp is DateTime)
                return (DateTime)timestamp;
            return UnixTimeStampToDateTime(double.Parse(timestamp.ToString(), CultureInfo.InvariantCulture));
        }

        static DateTime UnixTimeStampToDateTime(double unixTimeStamp) {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
            return dtDateTime;
        }
        #endregion

        const char parameterPrefix = '@';

        readonly BigQueryCommand bigQueryCommand;
        readonly BigqueryService bigQueryService;
        readonly CommandBehavior behavior;
        IEnumerator<TableRow> enumerator;
        IEnumerator<TableList.TablesData> tables;
        IList<TableRow> rows;
        TableSchema schema;
        int fieldsCount;
        bool disposed;

        internal BigQueryDataReader(CommandBehavior behavior, BigQueryCommand command, BigqueryService service) {
            this.behavior = behavior;
            bigQueryService = service;
            bigQueryCommand = command;
        }

        public override void Close() {
            Dispose();
        }

        public override T GetFieldValue<T>(int ordinal) {
            object value = GetValue(ordinal);
            return ChangeValueType<T>(value, ordinal);
        }

        public override DataTable GetSchemaTable() {
            DisposeCheck();
            if(tables.Current == null)
                return null;
            string projectId = bigQueryCommand.Connection.ProjectId;
            string dataSetId = bigQueryCommand.Connection.DataSetId;
            string tableId = tables.Current.TableReference.TableId;

            DataTable dataTable = new DataTable { TableName = tableId };
            dataTable.Columns.Add("ColumnName", typeof(string));
            dataTable.Columns.Add("DataType", typeof(Type));

            try {
                Table tableSchema = bigQueryService.Tables.Get(projectId, dataSetId, tableId).Execute();
                foreach(var tableFieldSchema in tableSchema.Schema.Fields) {
                    dataTable.Rows.Add(tableFieldSchema.Name, BigQueryTypeConverter.ToType(tableFieldSchema.Type));
                }
            }
            catch(GoogleApiException e) {
                throw e.Wrap();
            }

            return dataTable;
        }

        public override bool NextResult() {
            DisposeCheck();
            if(behavior == CommandBehavior.SchemaOnly) {
                return tables.MoveNext();
            }
            return false;
        }

        public override bool Read() {
            DisposeCheck();
            return enumerator.MoveNext();
        }

        public override int Depth {
            get { return 0; }
        }

        public override bool IsClosed {
            get { return disposed; }
        }

        public override int RecordsAffected {
            get { return 0; }
        }

        public override bool GetBoolean(int ordinal) {
            object value = GetValue(ordinal);
            return ChangeValueType<bool>(value, ordinal);
        }

        public override byte GetByte(int ordinal) {
            throw new NotSupportedException();
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length) {
            throw new NotSupportedException();
        }

        public override char GetChar(int ordinal) {
            throw new NotSupportedException();
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length) {
            throw new NotSupportedException();
        }

        public override Guid GetGuid(int ordinal) {
            throw new NotSupportedException();
        }

        public override short GetInt16(int ordinal) {
            object value = GetValue(ordinal);
            return ChangeValueType<short>(value, ordinal);
        }

        public override int GetInt32(int ordinal) {
            object value = GetValue(ordinal);
            return ChangeValueType<int>(value, ordinal);
        }

        public override long GetInt64(int ordinal) {
            object value = GetValue(ordinal);
            return ChangeValueType<long>(value, ordinal);
        }

        public override DateTime GetDateTime(int ordinal) {
            object value = GetValue(ordinal);
            return ChangeValueType<DateTime>(value, ordinal);
        }

        public override string GetString(int ordinal) {
            object value = GetValue(ordinal);
            return ChangeValueType<string>(value, ordinal);
        }

        public override decimal GetDecimal(int ordinal) {
            throw new NotSupportedException();
        }

        public override double GetDouble(int ordinal) {
            throw new NotSupportedException();
        }

        public override float GetFloat(int ordinal) {
            object value = GetValue(ordinal);
            return ChangeValueType<float>(value, ordinal);
        }

        public override string GetName(int ordinal) {
            DisposeCheck();
            RangeCheck(ordinal);
            return schema.Fields[ordinal].Name;
        }

        public override int GetProviderSpecificValues(object[] values) {
            return GetValues(values);
        }

        public override int GetValues(object[] values) {
            DisposeCheck();
            for(int i = 0; i < fieldsCount; i++) {
                values[i] = ChangeValueType(GetValue(i), i);
            }
            return values.Length;
        }

        public override bool IsDBNull(int ordinal) {
            DisposeCheck();
            RangeCheck(ordinal);
            return GetValue(ordinal) == null;
        }

        public override int VisibleFieldCount {
            get { return FieldCount; }
        }

        public override int FieldCount {
            get {
                DisposeCheck();
                return fieldsCount;
            }
        }

        public override object this[int ordinal] {
            get {
                DisposeCheck();
                return GetValue(ordinal);
            }
        }

        public override object this[string name] {
            get {
                DisposeCheck();
                int ordinal = GetOrdinal(name);
                return GetValue(ordinal);
            }
        }

        public override bool HasRows {
            get {
                DisposeCheck();
                return rows != null && rows.Count > 0;
            }
        }

        public override int GetOrdinal(string name) {
            DisposeCheck();
            for(int i = 0; i < schema.Fields.Count; i++) {
                if(schema.Fields[i].Name == name)
                    return i;
            }
            return -1;
        }

        public override string GetDataTypeName(int ordinal) {
            DisposeCheck();
            return GetFieldType(ordinal).Name;
        }

        public override Type GetProviderSpecificFieldType(int ordinal) {
            return GetFieldType(ordinal);
        }

        public override Type GetFieldType(int ordinal) {
            DisposeCheck();
            RangeCheck(ordinal);
            string type = schema.Fields[ordinal].Type;
            Type fieldType = BigQueryTypeConverter.ToType(type);
            if(fieldType != null)
                return fieldType;
            throw new ArgumentOutOfRangeException("ordinal", ordinal, "No field with ordinal");
        }

        public override object GetProviderSpecificValue(int ordinal) {
            return GetValue(ordinal);
        }

        public override object GetValue(int ordinal) {
            DisposeCheck();
            RangeCheck(ordinal);
            var value = enumerator.Current.F[ordinal].V;
            return ChangeValueType(value, ordinal);
        }

        public override IEnumerator GetEnumerator() {
            DisposeCheck();
            return enumerator;
        }

        internal async Task InitializeAsync(CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            try {
                if(behavior == CommandBehavior.SchemaOnly) {
                    TableList tableList = await bigQueryService.Tables.List(bigQueryCommand.Connection.ProjectId, bigQueryCommand.Connection.DataSetId).ExecuteAsync(cancellationToken).ConfigureAwait(false);
                    tables = tableList.Tables.GetEnumerator();
                } else {
                    ((BigQueryParameterCollection)bigQueryCommand.Parameters).Validate();
                    JobsResource.QueryRequest request = CreateRequest();
                    QueryResponse queryResponse = await request.ExecuteAsync(cancellationToken).ConfigureAwait(false);
                    ProcessQueryResponse(queryResponse);
                }
            }
            catch(GoogleApiException e) {
                throw e.Wrap();
            }
        }

        protected override void Dispose(bool disposing) {
            if(disposed)
                return;
            if(disposing) {
                if(enumerator != null) {
                    enumerator.Dispose();
                }
            }
            disposed = true;
            base.Dispose(disposing);
        }

        JobsResource.QueryRequest CreateRequest() {
            BigQueryParameterCollection collection = (BigQueryParameterCollection)bigQueryCommand.Parameters;
            foreach(BigQueryParameter parameter in collection) {
                bigQueryCommand.CommandText = bigQueryCommand.CommandText.Replace(parameterPrefix + parameter.ParameterName.TrimStart(parameterPrefix), PrepareParameterValue(parameter));
            }
            QueryRequest queryRequest = new QueryRequest { Query = PrepareCommandText(bigQueryCommand), TimeoutMs = bigQueryCommand.CommandTimeout != 0 ? (int)TimeSpan.FromSeconds(bigQueryCommand.CommandTimeout).TotalMilliseconds : int.MaxValue };
            JobsResource.QueryRequest request = bigQueryService.Jobs.Query(queryRequest, bigQueryCommand.Connection.ProjectId);
            return request;
        }

        void ProcessQueryResponse(QueryResponse queryResponse) {
            if(queryResponse.JobComplete.HasValue && !queryResponse.JobComplete.Value) {
                throw new BigQueryException("Timeout is reached");
            }
            rows = queryResponse.Rows;
            schema = queryResponse.Schema;
            fieldsCount = schema.Fields.Count;
            if(rows != null) {
                enumerator = rows.GetEnumerator();
            } else {
                rows = new TableRow[] { };
                enumerator = rows.GetEnumerator();
            }
        }

        T ChangeValueType<T>(object value, int ordinal) {
            object changed = ChangeValueType(value, ordinal);
            if(changed is T)
                return (T)ChangeValueType(value, ordinal);
            Type type = typeof(T);
            return (T)Convert.ChangeType(changed, type);
        }

        object ChangeValueType(object value, int ordinal) {
            if(value == null)
                return null;
            Type conversionType = BigQueryTypeConverter.ToType(schema.Fields[ordinal].Type);
            if(conversionType == typeof(DateTime))
                return UnixTimeStampToDateTime(value);
            return Convert.ChangeType(value, conversionType, CultureInfo.InvariantCulture);
        }

        void DisposeCheck() {
            if(disposed)
                throw new ObjectDisposedException("DataReader disposed");
        }

        void RangeCheck(int index) {
            if(index < 0 || fieldsCount <= index)
                throw new IndexOutOfRangeException("index out of range");
        }

        ~BigQueryDataReader() {
            Dispose(false);
        }
    }
}
