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
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace DevExpress.DataAccess.BigQuery {
    public class BigQueryCommand : DbCommand, ICloneable {
        const int defaultTimeout = 30;

        readonly BigQueryParameterCollection bigQueryParameterCollection = new BigQueryParameterCollection();
        CommandType commandType;
        int commandTimeout;

        public BigQueryCommand(BigQueryCommand command)
            : this(command.CommandText, command.Connection) {
            commandType = command.CommandType;
            commandTimeout = command.commandTimeout;
            foreach(BigQueryParameter bigQueryParameter in command.Parameters) {
                Parameters.Add(bigQueryParameter.Clone());
            }
        }

        public BigQueryCommand(string commandText, BigQueryConnection connection)
            : this(commandText) {
            Connection = connection;
        }

        public BigQueryCommand(string commandText) {
            CommandText = commandText;
        }

        public BigQueryCommand() : this(string.Empty) { }

        [DefaultValue("")]
        public override string CommandText { get; set; }

        [DefaultValue(defaultTimeout)]
        public override int CommandTimeout {
            get { return commandTimeout; }
            set {
                if(value < 0)
                    throw new ArgumentOutOfRangeException("value", value, "CommandTimeout can't be less than zero");
                commandTimeout = value;
            }
        }

        [DefaultValue(CommandType.Text)]
        public override CommandType CommandType {
            get { return commandType; }
            set {
                if(value == CommandType.StoredProcedure)
                    throw new ArgumentOutOfRangeException("value", value, "BigQuery does not support stored procedures");
                commandType = value;
            }
        }

        public override UpdateRowSource UpdatedRowSource {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        [DefaultValue(null)]
        public new BigQueryConnection Connection { get; set; }

        public override void Prepare() {
            throw new NotSupportedException();
        }

        public override bool DesignTimeVisible {
            get { return false; }
            set { throw new NotSupportedException(); }
        }

        public override void Cancel() { }

        public override async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            cancellationToken.Register(Cancel);
            using(DbDataReader dbDataReader = await ExecuteDbDataReaderAsync(CommandBehavior.Default, cancellationToken).ConfigureAwait(false)) {
                while(await dbDataReader.NextResultAsync(cancellationToken))
                    ;
                return dbDataReader.RecordsAffected;
            }
        }

        public override int ExecuteNonQuery() {
            var task = ExecuteNonQueryAsync();
            try {
                return task.Result;
            }
            catch(AggregateException e) {
                throw e.Flatten().InnerException;
            }
        }

        public override async Task<object> ExecuteScalarAsync(CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            cancellationToken.Register(Cancel);
            object result = null;
            using(DbDataReader dbDataReader = await ExecuteDbDataReaderAsync(CommandBehavior.Default, cancellationToken).ConfigureAwait(false)) {
                if(await dbDataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    if(dbDataReader.FieldCount > 0)
                        result = dbDataReader.GetValue(0);
            }
            return result;
        }

        public override object ExecuteScalar() {
            var task = ExecuteScalarAsync(CancellationToken.None);
            try {
                return task.Result;
            }
            catch(AggregateException e) {
                throw e.Flatten().InnerException;
            }
        }

        protected override DbConnection DbConnection {
            get { return Connection; }
            set { Connection = (BigQueryConnection)value; }
        }

        protected override DbParameterCollection DbParameterCollection {
            get { return bigQueryParameterCollection; }
        }

        protected override DbTransaction DbTransaction { get; set; }

        protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            cancellationToken.Register(Cancel);
            var reader = new BigQueryDataReader(behavior, this, Connection.Service);
            await reader.InitializeAsync(cancellationToken).ConfigureAwait(false);
            return reader;
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) {
            var task = ExecuteDbDataReaderAsync(behavior, CancellationToken.None);
            try {
                return task.Result;
            }
            catch(AggregateException e) {
                throw e.Flatten().InnerException;
            }
        }

        protected override DbParameter CreateDbParameter() {
            return new BigQueryParameter();
        }

        object ICloneable.Clone() {
            return new BigQueryCommand(this);
        }
    }
}
