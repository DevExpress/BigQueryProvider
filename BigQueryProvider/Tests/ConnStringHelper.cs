#if DEBUGTEST
using System.Configuration;
using System.Data.Common;

namespace DevExpress.DataAccess.BigQuery.Tests {
    public static class ConnStringHelper {
        public static string ConnectionString {
            get { return ConfigurationManager.ConnectionStrings["bigqueryConnectionStringOAuth"].ConnectionString; }
        }
    }
}
#endif