using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Dynamic;

namespace DevExpress.DataAccess.BigQuery {
    public class BigQueryCommand : DbCommand, ICloneable {
        readonly BigQueryParameterCollection bigQueryParameterCollection = new BigQueryParameterCollection();
        CommandType commandType;
        int commandTimeout;
        const int DefaultTimeout = 30;

        public BigQueryCommand(BigQueryCommand command) : this(command.CommandText, command.Connection) {
            this.commandType = command.CommandType;
            this.commandTimeout = command.commandTimeout;
            foreach(BigQueryParameter bigQueryParameter in command.Parameters) {
                this.Parameters.Add(bigQueryParameter.Clone());
            }
        }

        public BigQueryCommand(string commandText, BigQueryConnection connection) : this(commandText) {
            Connection = connection;
        }

        public BigQueryCommand(string commandText) {
            CommandText = commandText;
        }

        public BigQueryCommand() : this(string.Empty) {}

        string EscapingParameters(string commandText) {
            return string.Empty;
        }

        public override void Prepare() {
            throw new NotSupportedException();
        }

        [DefaultValue("")]
        public override string CommandText { get; set; }

        [DefaultValue(DefaultTimeout)]
        public override int CommandTimeout
        {
            get { return commandTimeout; }
            set
            {
                if(value < 0)
                    throw new ArgumentOutOfRangeException("value", value, "CommandTimeout can't be less than zero");
                commandTimeout = value;
            }
        }

        [DefaultValue(CommandType.Text)]
        public override CommandType CommandType
        {
            get { return commandType; }
            set
            {
                if(value == CommandType.StoredProcedure)
                    throw new ArgumentOutOfRangeException("value", value, "BigQuery does not support stored procedures");
                commandType = value;
            }
        }

        public override UpdateRowSource UpdatedRowSource
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        protected override DbConnection DbConnection
        {
            get { return Connection; }
            set { Connection = (BigQueryConnection) value; }
        }

        [DefaultValue(null)]
        public new BigQueryConnection Connection { get; set; }

        protected override DbParameterCollection DbParameterCollection
        {
            get { return bigQueryParameterCollection; }
        }

        protected override DbTransaction DbTransaction { get; set; }

        public override bool DesignTimeVisible
        {
            get { return false; }
            set { throw new NotSupportedException(); }
        }

        public override void Cancel() {}

        protected override DbParameter CreateDbParameter() {
            return new BigQueryParameter();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) {
            return new BigQueryDataReader(behavior, this, Connection.Service);
        }

        public override int ExecuteNonQuery() {
            using(DbDataReader dbDataReader = ExecuteDbDataReader(CommandBehavior.Default)) {
                return dbDataReader.RecordsAffected;
            }
        }

        public override object ExecuteScalar() {
            object result = null;
            using(DbDataReader dbDataReader = ExecuteDbDataReader(CommandBehavior.Default)) {
                if(dbDataReader.Read())
                    if(dbDataReader.FieldCount > 0)
                        result = dbDataReader.GetValue(0);
            }
            return result;
        }

        object ICloneable.Clone() {
            return new BigQueryCommand(this);
        }
    }
}
