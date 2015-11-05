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
    public class BigQueryCommandBuilder : DbCommandBuilder {
        /// <summary>
        /// Initializes a new instance of the BigQueryCommandBuilder class with default settings.
        /// </summary>
        public BigQueryCommandBuilder() {
            QuotePrefix = "[";
            QuoteSuffix = "]";
        }

        /// <summary>
        /// Initializes a new instance of the BigQueryCommandBuilder class with the specified data adapter.
        /// </summary>
        /// <param name="dataAdapter">A BigQueryDataAdapter for which to generate commands.</param>
        public BigQueryCommandBuilder(BigQueryDataAdapter dataAdapter) {
            DataAdapter = dataAdapter;
        }

        /// <summary>
        /// Specifies a BigQuery data adapter for which commands are generated.
        /// </summary>
        /// <value>
        /// A BigQueryDataAdapter object.
        /// </value>
        public new BigQueryDataAdapter DataAdapter {
            get { return (BigQueryDataAdapter)base.DataAdapter; }
            set { DataAdapter = value; }
        }

        /// <summary>
        /// Returns a command that deletes records from a data source.
        /// </summary>
        /// <returns>An automatically generated BigQuery command used to delete rows from a BigQuery data table.</returns>
        public new BigQueryCommand GetDeleteCommand() {
            return (BigQueryCommand)base.GetDeleteCommand();
        }

        //TODO: XmlDoc
        /// <summary>
        /// 
        /// </summary>
        /// <param name="useColumnsForParameterNames"></param>
        /// <returns>An automatically generated BigQuery command used to delete rows from a BigQuery data table.</returns>
        public new BigQueryCommand GetDeleteCommand(bool useColumnsForParameterNames) {
            return (BigQueryCommand)base.GetDeleteCommand(useColumnsForParameterNames);
        }

        /// <summary>
        /// Returns a command that inserts a record into a data source.
        /// </summary>
        /// <returns>An automatically generated BigQuery command used to insert rows into a BigQuery data table.</returns>
        public new BigQueryCommand GetInsertCommand() {
            return (BigQueryCommand)base.GetInsertCommand();
        }

        //TODO: XmlDoc
        /// <summary>
        /// 
        /// </summary>
        /// <param name="useColumnsForParameterNames"></param>
        /// <returns></returns>
        public new BigQueryCommand GetInsertCommand(bool useColumnsForParameterNames) {
            return (BigQueryCommand)base.GetInsertCommand(useColumnsForParameterNames);
        }

        /// <summary>
        /// Returns a properly quoted analog of the specified quoted identifier.
        /// </summary>
        /// <param name="unquotedIdentifier">a string containing an unquoted identifier. </param>
        /// <returns>a string containing a properly quoted identifier.</returns>
        public override string QuoteIdentifier(string unquotedIdentifier) {
            unquotedIdentifier = unquotedIdentifier.Replace("\\", "\\\\").Replace("[", "\\[").Replace("]", "\\]");
            return string.Concat(QuotePrefix, unquotedIdentifier, QuoteSuffix);
        }

        /// <summary>
        /// Returns an unquoted analog of the specified quoted identifier.
        /// </summary>
        /// <param name="quotedIdentifier">a string containing an unquoted identifier.</param>
        /// <returns>a string containing an identifier with quotes removed. </returns>
        public override string UnquoteIdentifier(string quotedIdentifier) {
            string unquotedIdentifier;
            if(string.IsNullOrEmpty(quotedIdentifier)) {
                unquotedIdentifier = quotedIdentifier;
            } else {
                if(quotedIdentifier[0] == '[') {
                    quotedIdentifier = quotedIdentifier.Substring(1, quotedIdentifier.Length - 2);
                    quotedIdentifier = quotedIdentifier.Replace("\\[", "[").Replace("\\]", "]").Replace("\\\\", "\\");
                }
                unquotedIdentifier = quotedIdentifier;
            }
            return unquotedIdentifier;
        }

        protected override DataTable GetSchemaTable(DbCommand sourceCommand) {
            using(IDataReader dataReader = sourceCommand.ExecuteReader(CommandBehavior.SchemaOnly)) {
                return dataReader.GetSchemaTable();
            }
        }

        protected override void ApplyParameterInfo(DbParameter parameter, DataRow row, StatementType statementType, bool whereClause) {
        }

        protected override string GetParameterName(int parameterOrdinal) {
            return "@p" + parameterOrdinal;
        }

        protected override string GetParameterName(string parameterName) {
            return "@" + parameterName;
        }

        protected override string GetParameterPlaceholder(int parameterOrdinal) {
            return GetParameterName(parameterOrdinal);
        }

        protected override void SetRowUpdatingHandler(DbDataAdapter dataAdapter) {
            if(DataAdapter == dataAdapter) {
                ((BigQueryDataAdapter)dataAdapter).RowUpdating -= OnRowUpdatingHandler;
            } else {
                ((BigQueryDataAdapter)dataAdapter).RowUpdating += OnRowUpdatingHandler;
            }
        }

        void OnRowUpdatingHandler(object sender, RowUpdatingEventArgs e) {
            RowUpdatingHandler(e);
        }
    }
}
