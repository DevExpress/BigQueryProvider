#if DEBUGTEST
﻿using System.Configuration;
namespace DevExpress.DataAccess.BigQuery.Tests {
    public static class ConnStringHelper {
        public static string ConnectionString {
            get { return ConfigurationManager.ConnectionStrings["bigqueryConnectionString"].ConnectionString; }
        }
    }
}
#endif
