using System.Data.Common;

namespace DevExpress.DataAccess.BigQuery.Tests {
    public static class ConnStringHelper {
        static readonly DbConnectionStringBuilder connectionStringBuilder = new DbConnectionStringBuilder();

        public static string ConnectionString {
            get { return connectionStringBuilder.ConnectionString.ToLower(); }
        }


        static ConnStringHelper() {
            connectionStringBuilder["PrivateKeyFileName"] = @"..\..\zymosimeter-e34a09c6f230.p12";
            connectionStringBuilder["ProjectID"] = "zymosimeter";
            connectionStringBuilder["ServiceAccountEmail"] = "227277881286-l0fodnq2h35m58b80up9vi4g83p1ogus@developer.gserviceaccount.com";
            connectionStringBuilder["DataSetId"] = "testdata";
        }
    }
}
