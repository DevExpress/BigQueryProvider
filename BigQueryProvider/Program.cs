using System;
using System.Data.Common;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Bigquery.v2;
using Google.Apis.Bigquery.v2.Data;
using Google.Apis.Services;

namespace DevExpress.DataAccess.BigQuery {
    class Program {
        public static string PrivateKeyFileName = @"E:\work\data_access\BigQueryProvider\WindowsFormsApplication1\zymosimeter-e34a09c6f230.p12";

        static void Main(string[] args) {
            var connectionStringBuilder = new DbConnectionStringBuilder();
            connectionStringBuilder["PrivateKeyFileName"] = Program.PrivateKeyFileName;
            connectionStringBuilder["ProjectID"] = "zymosimeter";

            var ApplicationName = "APP1";       // GB Application Name
            var ProjectID = "zymosimeter";         // GB Project ID
            var DataSet = "testdata";                // GB DataSet ID
            var TableName = "natality";             // GB Table ID

            // Refer To https://developers.google.com/bigquery/authorization#service-accounts-server
            var serviceAccountEmail = "227277881286-l0fodnq2h35m58b80up9vi4g83p1ogus@developer.gserviceaccount.com";
            var PrivateKeyFileName = Program.PrivateKeyFileName;
            var PrivateKeyPassword = "notasecret";
            var certificate = new X509Certificate2(PrivateKeyFileName, PrivateKeyPassword, X509KeyStorageFlags.Exportable);

            //==============================================================================================
            #region Setup credential
            ServiceAccountCredential credential = new ServiceAccountCredential(
               new ServiceAccountCredential.Initializer(serviceAccountEmail) {
                   Scopes = new[] { BigqueryService.Scope.Bigquery }
               }.FromCertificate(certificate));
            #endregion
            //==============================================================================================

            //==============================================================================================
            #region Create the service.
            var service = new BigqueryService(new BaseClientService.Initializer() {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });
            #endregion
            //==============================================================================================

            //==============================================================================================
            #region Read Dataset and DataTable

            var projects1 = service.Projects.List();
            var resp = projects1.Execute();
            var datasetRequest = service. Datasets.List(ProjectID);
            DatasetList datasetList = datasetRequest.Execute();


            // Get list of Dataset
            foreach(var item in datasetList.Datasets) {
                var ProjectDatasetID = item.Id;

                // Project : DataSetID
                Console.WriteLine(ProjectDatasetID);

                // Get list of tables
                var response = service.Tables.List(ProjectDatasetID.Split(':')[0],
                    ProjectDatasetID.Split(':')[1]).Execute();
             
                var tablelist = service.Tables.List(ProjectDatasetID.Split(':')[0],
                ProjectDatasetID.Split(':')[1]).Execute().Tables;
                foreach(var tbl in tablelist) {
                    Console.WriteLine(tbl.Id);
                }
            }
            #endregion
            //==============================================================================================

            //==============================================================================================
            // Read Record
            //var rows = service.Tabledata.List(ProjectID, DataSet, TableName).Execute().Rows.Take(100);

            //foreach(var item in rows) {
            //    Console.WriteLine(item.F[0].V);
            //}

                JobsResource.QueryRequest queryRequest = service.Jobs.Query(new QueryRequest() { Query = "SELECT *  FROM [testdata.natality] LIMIT 1000" },
                    ProjectID);
                var queryResponse = queryRequest.Execute();
                
            foreach(var item in queryResponse.Rows) {
                Console.WriteLine(item.F[0].V);
            }
                

            //


            //==============================================================================================

            //==============================================================================================
            //// Insert Record
            //var d = new TableDataInsertAllRequest();
            //d.Rows = new List<tabledatainsertallrequest.rowsdata>();
            //d.Kind = "bigquery#tableDataInsertAllRequest";

            //// Check @ https://developers.google.com/bigquery/streaming-data-into-bigquery for InsertId usage
            //var r = new TableDataInsertAllRequest.RowsData();
            //r.InsertId = "RowOne";
            //r.Json = new Dictionary<string, object="">();
            //r.Json.Add("id", "ID1");
            //r.Json.Add("Created", 
            //    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            //d.Rows.Add(r);

            //r = new TableDataInsertAllRequest.RowsData();
            //r.InsertId = "RowTwo";
            //r.Json = new Dictionary<string, object="">();
            //r.Json.Add("id", "ID2");
            //r.Json.Add("Created", 
            //    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            //d.Rows.Add(r);

            //var requestResponse = service.Tabledata.InsertAll(d, ProjectID, DataSet, TableName).Execute();

            //==============================================================================================

        }
    }
}
