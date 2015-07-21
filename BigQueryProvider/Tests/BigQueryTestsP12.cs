namespace DevExpress.DataAccess.BigQuery.Tests {
    public class BigQueryTestsP12 : BigQueryTestsBase {
        protected override string GetConnectionString() {
            return ConnectionStringHelper.P12ConnectionString;
        }
    }
}
