namespace DevExpress.DataAccess.BigQuery.Tests {
    public class BigQueryTestsOAuth : BigQueryTestsBase {
        protected override string GetConnectionString() {
            return ConnectionStringHelper.OAuthConnectionString;
        }
    }
}
