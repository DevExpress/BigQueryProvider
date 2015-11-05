/*
   Copyright 2015 Developer Express Inc.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.DataAccess.BigQuery.Native;
using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Bigquery.v2;
using Google.Apis.Bigquery.v2.Data;
using Google.Apis.Http;
using Google.Apis.Services;

namespace DevExpress.DataAccess.BigQuery {
    public class BigQueryConnection : DbConnection, ICloneable {
        const string applicationName = "DevExpress.DataAccess.BigQuery ADO.NET Provider";

        ConnectionState state;
        readonly DbConnectionStringBuilder connectionStringBuilder = new DbConnectionStringBuilder();
        bool disposed;

        /// <summary>
        /// Initializes a new instance of the BigQueryConnection class with default settings.
        /// </summary>
        public BigQueryConnection() { }

        /// <summary>
        /// Initializes a new instance of the BigQueryConnection class with the specified connection string.
        /// </summary>
        /// <param name="connectionString">A System.String specifying a connection string.</param>
        public BigQueryConnection(string connectionString) {
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Changes the database for the current connection.
        /// </summary>
        /// <param name="databaseName">A System.String value specifying the database name.</param>
        public override void ChangeDatabase(string databaseName) {
            CheckDisposed();
            DataSetId = databaseName;
        }

        /// <summary>
        /// Asynchronously opens a data connection.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel opening a data connection.</param>
        /// <returns>A System.Threading.Tasks.Task, specifying an asynchronous operation.</returns>
        public override async Task OpenAsync(CancellationToken cancellationToken) {
            CheckDisposed();
            cancellationToken.ThrowIfCancellationRequested();
            if(IsOpened)
                throw new InvalidOperationException("Connection allready open");
            try {
                await InitializeServiceAsync(cancellationToken).ConfigureAwait(false);
            }
            catch(GoogleApiException e) {
                state = ConnectionState.Broken;
                throw e.Wrap();
            }
        }

        /// <summary>
        /// Opens a data connection.
        /// </summary>
        public override void Open() {
            var task = OpenAsync();
            try {
                task.Wait();
            }
            catch(AggregateException e) {
                throw e.Flatten().InnerException;
            }
        }

        /// <summary>
        /// Closes the current data connection.
        /// </summary>
        public override void Close() {
            CheckDisposed();
            if(!IsOpened)
                return;
            state = ConnectionState.Closed;
        }

        /// <summary>
        /// Returns a list of datasets available in the current Google Cloud Platform project.
        /// </summary>
        /// <returns>an array of System.String values containing names of available datasets.</returns>
        public string[] GetDataSetNames() {
            CheckDisposed();
            CheckOpen();
            DatasetList dataSets;
            try {
                dataSets = Service.Datasets.List(ProjectId).Execute();
            }
            catch(GoogleApiException e) {
                throw e.Wrap();
            }
            return dataSets.Datasets.Select(d => d.DatasetReference.DatasetId).ToArray();
        }

        /// <summary>
        /// Returns a list of tables available in the current BigQuery dataset.
        /// </summary>
        /// <returns>an array of System.String values containing names of available data tables.</returns>
        public string[] GetTableNames() {
            return GetDataObjectNames("TABLE");
        }

        /// <summary>
        /// Returns a list of views available in the current BigQuery dataset.
        /// </summary>
        /// <returns>an array of System.String values containing names of available data views.</returns>
        public string[] GetViewNames() {
            return GetDataObjectNames("VIEW");
        }

        string[] GetDataObjectNames(string type) {
            CheckDisposed();
            CheckOpen();
            TableList tableList;
            try {
                tableList = Service.Tables.List(ProjectId, DataSetId).Execute();
            } catch(GoogleApiException e) {
                throw e.Wrap();
            }
            return tableList.Tables.Where(t => t.Type == type).Select(t => t.TableReference.TableId).ToArray();
        }

        /// <summary>
        /// Specifies the connection string used to establish the current data connection.
        /// </summary>
        /// <value>
        /// a System.String value specifying a connection string.
        /// </value>
        public override string ConnectionString {
            get { return connectionStringBuilder.ConnectionString; }
            set { connectionStringBuilder.ConnectionString = value; }
        }

        /// <summary>
        /// Creates a new BigQuery command associated with the current data connection.
        /// </summary>
        /// <returns>a BigQueryCommand object.</returns>
        public new BigQueryCommand CreateCommand() {
            CheckDisposed();
            CheckOpen();
            return new BigQueryCommand { Connection = this };
        }

        /// <summary>
        /// Gets the name of a BigQueryproject to which to connect. 
        /// </summary>
        /// <value>
        /// Gets the name of the BigQuery data source to which to connect.
        /// </value>
        public override string DataSource {
            get {
                CheckOpen();
                return ProjectId;
            }
        }

        /// <summary>
        /// Gets the name of the current Big Query dataset.
        /// </summary>
        /// <value>
        /// The name of the current dataset.
        /// </value>
        public override string Database {
            get {
                CheckOpen();
                return DataSetId;
            }
        }

        /// <summary>
        /// Gets a string containing the version of a database server to which the client is connected. This implementation always throws NotSupportedException.
        /// </summary>
        /// <value>
        /// A string containing the version of database server. 
        /// </value>
        public override string ServerVersion {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the state of the current data connection.
        /// </summary>
        /// <value>
        /// A ConnectionState enumeration value.
        /// </value>
        public override ConnectionState State {
            get { return state; }
        }

        protected override DbCommand CreateDbCommand() {
            return CreateCommand();
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) {
            CheckDisposed();
            throw new NotSupportedException();
        }

        protected override void Dispose(bool disposing) {
            if(disposed)
                return;
            if(disposing) {
                if(Service != null) {
                    Service.Dispose();
                }
            }
            Close();
            disposed = true;
            base.Dispose(disposing);
        }

        internal BigqueryService Service { get; private set; }

        internal string ProjectId {
            get {
                return (string)connectionStringBuilder["ProjectId"];
            }
        }

        internal string DataSetId {
            get {
                return (string)connectionStringBuilder["DataSetId"];
            }

            set {
                if((string)connectionStringBuilder["DataSetId"] == value)
                    return;
                connectionStringBuilder["DataSetId"] = value;
                ConnectionString = connectionStringBuilder.ConnectionString;
            }
        }

        string OAuthRefreshToken {
            get {
                return connectionStringBuilder.ContainsKey("OAuthRefreshToken") ? (string)connectionStringBuilder["OAuthRefreshToken"] : null;
            }
            set {
                connectionStringBuilder["OAuthRefreshToken"] = value;
                ConnectionString = connectionStringBuilder.ConnectionString;

            }
        }

        string OAuthAccessToken {
            get {
                return connectionStringBuilder.ContainsKey("OAuthAccessToken") ? (string)connectionStringBuilder["OAuthAccessToken"] : null;
            }
            set {
                connectionStringBuilder["OAuthAccessToken"] = value;
                ConnectionString = connectionStringBuilder.ConnectionString;
            }
        }

        string OAuthClientId {
            get {
                return (string)connectionStringBuilder["OAuthClientId"];
            }
        }

        string OAuthClientSecret {
            get {
                return (string)connectionStringBuilder["OAuthClientSecret"];
            }
        }

        string ServiceAccountEmail {
            get {
                return connectionStringBuilder.ContainsKey("ServiceAccountEmail") ? (string)connectionStringBuilder["ServiceAccountEmail"] : string.Empty;
            }
        }

        string PrivateKeyFileName {
            get {
                return connectionStringBuilder.ContainsKey("PrivateKeyFileName") ? (string)connectionStringBuilder["PrivateKeyFileName"] : String.Empty;
            }
        }

        bool IsOpened {
            get { return state == ConnectionState.Open; }
        }

        void CheckOpen() {
            if(!IsOpened)
                throw new InvalidOperationException("connection is closed");
        }

        void CheckDisposed() {
            if(disposed) {
                throw new ObjectDisposedException(ToString());
            }
        }

        async Task InitializeServiceAsync(CancellationToken cancellationToken) {
            CheckDisposed();
            cancellationToken.ThrowIfCancellationRequested();
            state = ConnectionState.Connecting;
            Service = await CreateServiceAsync(cancellationToken).ConfigureAwait(false);
            JobsResource.ListRequest listRequest = Service.Jobs.List(ProjectId);
            await listRequest.ExecuteAsync(cancellationToken).ConfigureAwait(false);
            state = ConnectionState.Open;
        }

        async Task<BigqueryService> CreateServiceAsync(CancellationToken cancellationToken) {
            IConfigurableHttpClientInitializer credential;
            cancellationToken.ThrowIfCancellationRequested();
            if(string.IsNullOrEmpty(PrivateKeyFileName)) {
                var dataStore = new DataStore(OAuthRefreshToken, OAuthAccessToken);

                var clientSecrets = new ClientSecrets {
                    ClientId = OAuthClientId,
                    ClientSecret = OAuthClientSecret
                };

                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(clientSecrets,
                    new[] { BigqueryService.Scope.Bigquery },
                    "user",
                    cancellationToken,
                    dataStore).ConfigureAwait(false);

                OAuthRefreshToken = dataStore.RefreshToken;
                OAuthAccessToken = dataStore.AccessToken;
            } else {
                X509Certificate2 certificate = new X509Certificate2(PrivateKeyFileName, "notasecret", X509KeyStorageFlags.Exportable);
                credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(ServiceAccountEmail) {
                    Scopes = new[] { BigqueryService.Scope.Bigquery }
                }.FromCertificate(certificate));
            }

            return new BigqueryService(new BaseClientService.Initializer {
                HttpClientInitializer = credential,
                ApplicationName = applicationName
            });
        }

        object ICloneable.Clone() {
            return new BigQueryConnection(ConnectionString);
        }
    }
}
