using System.Data;
using System.Data.Common;

namespace DevExpress.DataAccess.BigQuery {
    public class BigQueryCommandBuilder : DbCommandBuilder {
        public BigQueryDataAdapter DataDataAdapter
        {
            get { return (BigQueryDataAdapter) base.DataAdapter; }
            set { this.DataDataAdapter = value; }
        }

        public BigQueryCommandBuilder() {
            this.QuotePrefix = "[";
            this.QuoteSuffix = "]";
        }

        public BigQueryCommandBuilder(BigQueryDataAdapter dataAdapter) {
            DataDataAdapter = dataAdapter;
        }

        public override string QuoteIdentifier(string unquotedIdentifier) {
            unquotedIdentifier = unquotedIdentifier.Replace("\\", "\\\\").Replace("[", "\\[").Replace("]", "\\]");
            return string.Concat(QuotePrefix, unquotedIdentifier, QuoteSuffix);
        }

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


        public BigQueryCommand GetDeleteCommand() {
            return (BigQueryCommand) base.GetDeleteCommand();
        }

        public BigQueryCommand GetDeleteCommand(bool useColumnsForParameterNames) {
            return (BigQueryCommand) base.GetDeleteCommand(useColumnsForParameterNames);
        }

        public BigQueryCommand GetInsertCommand() {
            return (BigQueryCommand) base.GetInsertCommand();
        }

        public BigQueryCommand GetInsertCommand(bool useColumnsForParameterNames) {
            return (BigQueryCommand) base.GetInsertCommand(useColumnsForParameterNames);
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
            if(this.DataDataAdapter == dataAdapter) {
                ((BigQueryDataAdapter) dataAdapter).RowUpdating -= OnRowUpdatingHandler;
            }
            else {
                ((BigQueryDataAdapter) dataAdapter).RowUpdating += OnRowUpdatingHandler;
            }
        }

        void OnRowUpdatingHandler(object sender, RowUpdatingEventArgs e) {
            this.RowUpdatingHandler(e);
        }
    }
}
