using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Google.Apis.Bigquery.v2;
using Google.Apis.Bigquery.v2.Data;

namespace DevExpress.DataAccess.BigQuery {
    public class BigQueryDataReader : DbDataReader {
        readonly BigQueryCommand bigQueryCommand;
        readonly BigqueryService bigQueryService;   
        readonly IEnumerator<TableRow> enumerator;
        readonly int fieldsCount;
        readonly TableSchema schema;
        readonly CommandBehavior behavior;
        readonly IEnumerator<TableList.TablesData> tables;
        readonly IList<TableRow> rows;
        bool disposed;

        public BigQueryDataReader(CommandBehavior behavior, BigQueryCommand command, BigqueryService service) {
            this.behavior = behavior;

            bigQueryService = service;
            bigQueryCommand = command;

            try {
                if(behavior == CommandBehavior.SchemaOnly) {
                    TableList tableList = service.Tables.List(command.Connection.ProjectId, command.Connection.DataSetId).Execute();
                    tables = tableList.Tables.GetEnumerator();
                }
                else {
                    BigQueryParameterCollection collection = (BigQueryParameterCollection) command.Parameters;
                    foreach(BigQueryParameter parameter in collection) {
                        command.CommandText = command.CommandText.Replace("@" + parameter.ParameterName, PrepareParameterValue(parameter.Value).ToString());
                    }
                    QueryRequest queryRequest = new QueryRequest() { Query = PrepareCommandText(command), TimeoutMs = command.CommandTimeout != 0 ? command.CommandTimeout : int.MaxValue };
                    JobsResource.QueryRequest request = service.Jobs.Query(queryRequest, command.Connection.ProjectId);
                    QueryResponse queryResponse = request.Execute();
                    rows = queryResponse.Rows;
                    schema = queryResponse.Schema;
                    if(rows != null) {
                        TableRow firstOrDefault = rows.FirstOrDefault();
                        if(firstOrDefault != null)
                            fieldsCount = firstOrDefault.F.Count;
                        enumerator = rows.GetEnumerator();
                    }
                    else {
                        rows = new TableRow[]{};
                        fieldsCount = 0;
                        enumerator = rows.GetEnumerator();
                    }
                }
            }
            catch(Google.GoogleApiException e) {
                throw e.Wrap();
            }
        }

        static string PrepareCommandText(BigQueryCommand command) {
            return command.CommandType == CommandType.TableDirect ? string.Format("SELECT * FROM [{0}.{1}]", command.Connection.DataSetId, command.CommandText) : command.CommandText;
        }


        static object PrepareParameterValue(object value) {
            Type valueType = value.GetType();
            if(valueType == typeof(string)) {
                value = value.ToString().Replace("'", "''").Replace("\"", @"""").Replace(@"\", @"\\");
            }
            return value;
        }

        public override void Close() {
            this.Dispose();
        }

        public override DataTable GetSchemaTable() {
            this.DisposeCheck();
            string projectId = bigQueryCommand.Connection.ProjectId;
            string dataSetId = bigQueryCommand.Connection.DataSetId;
            string tableId = tables.Current.TableReference.TableId;

            DataTable dataTable = new DataTable() { TableName = tableId };
            dataTable.Columns.Add("ColumnName", typeof(string));
            dataTable.Columns.Add("DataType", typeof(Type));

            try {
                Table tableSchema = bigQueryService.Tables.Get(projectId, dataSetId, tableId).Execute();
                foreach(var tableFieldSchema in tableSchema.Schema.Fields) {
                    dataTable.Rows.Add(tableFieldSchema.Name, FieldType(tableFieldSchema.Type));
                }
            }
            catch(Google.GoogleApiException e) {
                throw e.Wrap();
            }

            return dataTable;
        }

        public override bool NextResult() {
            this.DisposeCheck();
            if (behavior == CommandBehavior.SchemaOnly) {
                return tables.MoveNext();
            }
            return false;
        }

        public override bool Read() {
            this.DisposeCheck();
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
            this.DisposeCheck();
            this.RangeCheck(ordinal);
            return schema.Fields[ordinal].Name;
        }

        public override int GetValues(object[] values) {
            this.DisposeCheck();
            for(int i = 0; i < fieldsCount; i++) {
                values[i] = ChangeValueType(GetValue(i), i);
            }
            return values.Length;
        }

        T ChangeValueType<T>(object value, int ordinal) {
            return (T) ChangeValueType(value, ordinal);
        }

        object ChangeValueType(object value, int ordinal) {
            if(value == null)
                return null;
            Type conversionType = FieldType(schema.Fields[ordinal].Type);
            if(conversionType == typeof(DateTime))
                return UnixTimeStampToDateTime(value);
            return Convert.ChangeType(value,  conversionType);
        }

        public static DateTime UnixTimeStampToDateTime(object timestamp) {
            return UnixTimeStampToDateTime(double.Parse(timestamp.ToString()));
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp) {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
            return dtDateTime;
        }

        public override bool IsDBNull(int ordinal) {
            this.DisposeCheck();
            this.RangeCheck(ordinal);
            return GetValue(ordinal) == null;
        }

        public override int FieldCount {
            get {
                this.DisposeCheck(); 
                return fieldsCount;
            }
        }

        public override object this[int ordinal] {
            get { 
                this.DisposeCheck(); 
                return GetValue(ordinal); }
        }

        public override object this[string name] {
            get {
                this.DisposeCheck();
                int ordinal = GetOrdinal(name);
                return GetValue(ordinal);
            }
        }

        public override bool HasRows {
            get {
                this.DisposeCheck();
                return rows != null && rows.Count > 0;
            }
        }

        public override int GetOrdinal(string name) {
            this.DisposeCheck();
            for(int i = 0; i < schema.Fields.Count; i++) {
                if(schema.Fields[i].Name == name)
                    return i;
            }
            return -1;
        }

        public override string GetDataTypeName(int ordinal) {
            this.DisposeCheck();
            return GetFieldType(ordinal).Name;
        }

        public override Type GetFieldType(int ordinal) {
            this.DisposeCheck();
            this.RangeCheck(ordinal);
            string type = schema.Fields[ordinal].Type;
            Type fieldType = FieldType(type);
            if(fieldType != null)
                return fieldType;
            throw new ArgumentOutOfRangeException("ordinal", ordinal, "No field with ordinal");
        }

        static Type FieldType(string type) {
            switch(type) {
                case "STRING":
                    return typeof(string);
                case "INTEGER":
                    return typeof(int);
                case "FLOAT":
                    return typeof(float);
                case "BOOLEAN":
                    return typeof(bool);
                case "TIMESTAMP":
                    return typeof(DateTime);
                case "RECORD":
                    return typeof(object);
            }
            return null;
        }

        public override object GetValue(int ordinal) {
            this.DisposeCheck();
            this.RangeCheck(ordinal);
            return enumerator.Current.F[ordinal].V;
        }

        public override IEnumerator GetEnumerator() {
            this.DisposeCheck();
            return enumerator;
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

        void DisposeCheck() {
            if(disposed) 
                throw new ObjectDisposedException("DataReader disposed");
        }

        void RangeCheck(int index) {
            if(index < 0 || this.fieldsCount <= index)
                throw new IndexOutOfRangeException("index out of range");
        }

        ~BigQueryDataReader() {
            Dispose(false);
        }
    }
}
