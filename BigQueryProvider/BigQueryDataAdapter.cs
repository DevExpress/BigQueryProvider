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

using System.Data;
using System.Data.Common;

namespace DevExpress.DataAccess.BigQuery {
    /// <summary>
    /// Represents the method that will handle the RowUpdating event of a BigQueryDataAdapter.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A BigQueryRowUpdatingEventArgs object containing event data.</param>
    public delegate void BigQueryRowUpdatingEventHandler(object sender, BigQueryRowUpdatingEventArgs e);

    /// <summary>
    /// Represents the method that will handle the RowUpdated event of a BigQueryDataAdapter.
    /// </summary>
    /// <param name="sender">The source of the event. </param>
    /// <param name="e">A BigQueryRowUpdatedEventArgs object containing event data.</param>
    public delegate void BigQueryRowUpdatedEventHandler(object sender, BigQueryRowUpdatedEventArgs e);

    public class BigQueryDataAdapter : DbDataAdapter {
        /// <summary>
        /// Initializes a new instance of the BigQueryDataAdapter class with default settings.
        /// </summary>
        public BigQueryDataAdapter() { }

        //TODO: XmlDoc
        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectCommand"></param>
        public BigQueryDataAdapter(BigQueryCommand selectCommand) {
            SelectCommand = selectCommand;
        }

        //TODO: XmlDoc
        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectCommandText"></param>
        /// <param name="connection"></param>
        public BigQueryDataAdapter(string selectCommandText, BigQueryConnection connection) : this(new BigQueryCommand(selectCommandText, connection)) { }

        /// <summary>
        ///  Occurs during the update before a command is executed against the data source.
        /// </summary>
        public event BigQueryRowUpdatingEventHandler RowUpdating;

        /// <summary>
        /// Occurs after a command has been executed against the data source.
        /// </summary>
        public event BigQueryRowUpdatedEventHandler RowUpdated;

        /// <summary>
        /// Specifies a command used to obtain rows from a BigQuery data table.
        /// </summary>
        /// <value>
        /// A BigQuery command used to obtain rows from a BigQuery data table.
        /// </value>
        public new BigQueryCommand SelectCommand {
            get { return (BigQueryCommand)base.SelectCommand; }
            set { base.SelectCommand = value; }
        }

        public new BigQueryCommand DeleteCommand {
            get { return (BigQueryCommand)base.DeleteCommand; }
            set { base.DeleteCommand = value; }
        }

        /// <summary>
        ///  Specifies a command used to insert rows into a BigQuery data table.
        /// </summary>
        /// <value>
        /// A BigQuery command used to insert rows into a BigQuery data table.
        /// </value>
        public new BigQueryCommand InsertCommand {
            get { return (BigQueryCommand)base.InsertCommand; }
            set { base.InsertCommand = value; }
        }

        /// <summary>
        /// Specifies a command used to update rows of a BigQuery data table.
        /// </summary>
        /// <value>
        /// A BigQuery command used to update rows of a BigQuery data table.
        /// </value>
        public new BigQueryCommand UpdateCommand {
            get { return (BigQueryCommand)base.UpdateCommand; }
            set { base.UpdateCommand = value; }
        }

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
    }

    public class BigQueryRowUpdatingEventArgs : RowUpdatingEventArgs {
        /// <summary>
        ///  Initializes a new instance of the BigQueryRowUpdatingEventArgs class.
        /// </summary>
        /// <param name="dataRow">The DataRow sent throught an Update.</param>
        /// <param name="command">A BigQueryCommand being executed when Update is called.</param>
        /// <param name="statementType">A StatementType enumeration value indicating the type of the statement being executed.</param>
        /// <param name="tableMapping">The DataTableMapping sent through an Update.</param>
        public BigQueryRowUpdatingEventArgs(DataRow dataRow, IDbCommand command, StatementType statementType,
            DataTableMapping tableMapping)
            : base(dataRow, command, statementType, tableMapping) { }
    }

    public class BigQueryRowUpdatedEventArgs : RowUpdatedEventArgs {
        /// <summary>
        /// Initializes a new instance of the BigQueryRowUpdatedEventArgs class.
        /// </summary>
        /// <param name="dataRow">The DataRow sent throught an Update.</param>
        /// <param name="command"> A BigQueryCommand that has been executed as a result of calling Update.</param>
        /// <param name="statementType">A StatementType enumeration value indicating the type of the executed statement.</param>
        /// <param name="tableMapping">The DataTableMapping sent through an Update.</param>
        public BigQueryRowUpdatedEventArgs(DataRow dataRow, IDbCommand command, StatementType statementType,
            DataTableMapping tableMapping)
            : base(dataRow, command, statementType, tableMapping) { }
    }
}
