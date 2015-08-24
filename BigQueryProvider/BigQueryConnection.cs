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

        public BigQueryConnection() { }

        public BigQueryConnection(string connectionString) {
            ConnectionString = connectionString;
        }

        public override void ChangeDatabase(string databaseName) {
            CheckDisposed();
            DataSetId = databaseName;
        }

        public async override Task OpenAsync(CancellationToken cancellationToken) {
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

        public override void Open() {
            var task = OpenAsync();
            try {
                task.Wait();
            }
            catch(AggregateException e) {
                throw e.Flatten().InnerException;
            }
        }

        public override void Close() {
            CheckDisposed();
            if(!IsOpened)
                return;
            state = ConnectionState.Closed;
        }

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

        public string[] GetTableNames() {
            return GetDataObjectNames("TABLE");
        }

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

        public override string ConnectionString {
            get { return connectionStringBuilder.ConnectionString; }
            set { connectionStringBuilder.ConnectionString = value; }
        }

        public new BigQueryCommand CreateCommand() {
            CheckDisposed();
            CheckOpen();
            return new BigQueryCommand { Connection = this };
        }

        public override string DataSource {
            get {
                CheckOpen();
                return ProjectId;
            }
        }

        public override string Database {
            get {
                CheckOpen();
                return DataSetId;
            }
        }

        public override string ServerVersion {
            get { throw new NotSupportedException(); }
        }

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
