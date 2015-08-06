#if DEBUGTEST
using System.Configuration;

namespace DevExpress.DataAccess.BigQuery.Tests {
    public static class ConnectionStringHelper {
        public static string OAuthConnectionString {
            get { return ConfigurationManager.ConnectionStrings["bigqueryConnectionStringOAuth"].ConnectionString; }
        }
        public static string P12ConnectionString {
            get { return ConfigurationManager.ConnectionStrings["bigqueryConnectionStringP12"].ConnectionString; }
        }
    }
}
#endif