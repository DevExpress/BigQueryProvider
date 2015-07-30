#if DEBUGTEST
namespace DevExpress.DataAccess.BigQuery.Tests {
    public class BigQueryCommandTestsP12 : BigQueryCommandTestsBase {
        protected override string GetConnectionString() {
            return ConnectionStringHelper.P12ConnectionString;
        }
    }
}
#endif
