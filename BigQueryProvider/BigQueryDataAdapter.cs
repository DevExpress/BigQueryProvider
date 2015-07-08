using System;
using System.Data;
using System.Data.Common;

namespace DevExpress.DataAccess.BigQuery {
    public delegate void BigQueryRowUpdatingEventHandler(object sender, BigQueryRowUpdatingEventArgs e);

    public delegate void BigQueryRowUpdatedEventHandler(object sender, BigQueryRowUpdatedEventArgs e);

    public class BigQueryDataAdapter : DbDataAdapter {
        public BigQueryDataAdapter() {}

        public BigQueryDataAdapter(BigQueryCommand selectCommand) {
            SelectCommand = selectCommand;
        }

        public BigQueryDataAdapter(string selectCommandText, BigQueryConnection connection) : this(new BigQueryCommand(selectCommandText, connection)) {}

        public event BigQueryRowUpdatingEventHandler RowUpdating;
        public event BigQueryRowUpdatedEventHandler RowUpdated;

        protected override RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping) {
            return new BigQueryRowUpdatingEventArgs(dataRow, command, statementType, tableMapping);
        }

        protected override RowUpdatedEventArgs CreateRowUpdatedEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping) {
            return new BigQueryRowUpdatedEventArgs(dataRow, command, statementType, tableMapping);
        }

        protected override void OnRowUpdating(RowUpdatingEventArgs value) {
            BigQueryRowUpdatingEventArgs args = value as BigQueryRowUpdatingEventArgs;
            if(RowUpdating != null && (args != null)) {
                RowUpdating(this, args);
            }
        }

        protected override void OnRowUpdated(RowUpdatedEventArgs value) {
            BigQueryRowUpdatedEventArgs args = value as BigQueryRowUpdatedEventArgs;
            if(RowUpdated != null && (args != null)) {
                RowUpdated(this, args);
            }
        }

        public new BigQueryCommand SelectCommand {
            get { return (BigQueryCommand) base.SelectCommand; }
            set { base.SelectCommand = value; }
        }

        public new BigQueryCommand DeleteCommand {
            get { return (BigQueryCommand) base.DeleteCommand; }
            set { base.DeleteCommand = value; }
        }

        public new BigQueryCommand InsertCommand {
            get { return (BigQueryCommand) base.InsertCommand; }
            set { base.InsertCommand = value; }
        }

        public new BigQueryCommand UpdateCommand {
            get { return (BigQueryCommand) base.UpdateCommand; }
            set { base.UpdateCommand = value; }
        }
    }

    public class BigQueryRowUpdatingEventArgs : RowUpdatingEventArgs {
        public BigQueryRowUpdatingEventArgs(DataRow dataRow, IDbCommand command, StatementType statementType,
            DataTableMapping tableMapping)
            : base(dataRow, command, statementType, tableMapping) {}
    }

    public class BigQueryRowUpdatedEventArgs : RowUpdatedEventArgs {
        public BigQueryRowUpdatedEventArgs(DataRow dataRow, IDbCommand command, StatementType statementType,
            DataTableMapping tableMapping)
            : base(dataRow, command, statementType, tableMapping) {}
    }
}
