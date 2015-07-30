#if DEBUGTEST
namespace DevExpress.DataAccess.BigQuery.Tests {
    public class BigQueryCommandTestsOAuth : BigQueryCommandTestsBase {
        protected override string GetConnectionString() {
            return ConnectionStringHelper.OAuthConnectionString;
        }
    }
}
#endif
