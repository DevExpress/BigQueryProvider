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

        internal string ProjectId {
            get {
                return (string)connectionStringBuilder["ProjectId"];
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

        #region OAuth
        internal string OAuthRefreshToken {
            get {
                return connectionStringBuilder.ContainsKey("OAuthRefreshToken") ? (string)connectionStringBuilder["OAuthRefreshToken"] : null;
            }
            set {
                connectionStringBuilder["OAuthRefreshToken"] = value;
                ConnectionString = connectionStringBuilder.ConnectionString;

            }
        }

        internal string OAuthAccessToken {
            get {
                return connectionStringBuilder.ContainsKey("OAuthAccessToken") ? (string)connectionStringBuilder["OAuthAccessToken"] : null;
            }
            set {
                connectionStringBuilder["OAuthAccessToken"] = value;
                ConnectionString = connectionStringBuilder.ConnectionString;
            }
        }

        internal string OAuthClientId {
            get {
                return (string)connectionStringBuilder["OAuthClientId"];
            }
        }

        internal string OAuthClientSecret {
            get {
                return (string)connectionStringBuilder["OAuthClientSecret"];
            }
        }
        #endregion

        internal BigqueryService Service { get; private set; }

        public BigQueryConnection() { }

        public BigQueryConnection(string connectionString) {
            ConnectionString = connectionString;
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) {
            CheckDisposed();
            throw new NotSupportedException();
        }

        public override void ChangeDatabase(string databaseName) {
            CheckDisposed();
            if(IsOpened)
                Close();
            DataSetId = databaseName;
            try {
                InitializeService();
            }
            catch(GoogleApiException e) {
                state = ConnectionState.Broken;
                throw e.Wrap();
            }
        }

        public async override Task OpenAsync(CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            CheckDisposed();
            if(IsOpened)
                throw new InvalidOperationException("Connection allready open");
            try {
                await InitializeServiceAsync().ConfigureAwait(false);
            }
            catch(GoogleApiException e) {
                state = ConnectionState.Broken;
                throw e.Wrap();
            }
        }

        public override void Open() {
            CheckDisposed();
            if(IsOpened)
                throw new InvalidOperationException("Connection allready open");
            try {
                InitializeService();
            }
            catch(GoogleApiException e) {
                state = ConnectionState.Broken;
                throw e.Wrap();
            }
        }

        void InitializeService() {
            CheckDisposed();
            state = ConnectionState.Connecting;
            Service = CreateService().Result;
            JobsResource.ListRequest listRequest = Service.Jobs.List(ProjectId);
            listRequest.Execute();
            state = ConnectionState.Open;
        }

        async Task InitializeServiceAsync() {
            CheckDisposed();
            state = ConnectionState.Connecting;
            Service = await CreateService().ConfigureAwait(false);
            JobsResource.ListRequest listRequest = Service.Jobs.List(ProjectId);
            await listRequest.ExecuteAsync().ConfigureAwait(false);
            state = ConnectionState.Open;
        }

        async Task<BigqueryService> CreateService() {
            IConfigurableHttpClientInitializer credential;
            if (string.IsNullOrEmpty(PrivateKeyFileName)) {
                var dataStore = new DataStore(OAuthRefreshToken, OAuthAccessToken);

                var clientSecrets = new ClientSecrets {
                    ClientId = OAuthClientId,
                    ClientSecret = OAuthClientSecret
                };

                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(clientSecrets,
                    new[] {BigqueryService.Scope.Bigquery},
                    "user",
                    CancellationToken.None,
                    dataStore).ConfigureAwait(false);

                OAuthRefreshToken = dataStore.RefreshToken;
                OAuthAccessToken = dataStore.AccessToken;
            }
            else {
                X509Certificate2 certificate = new X509Certificate2(PrivateKeyFileName, "notasecret", X509KeyStorageFlags.Exportable);
                credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(ServiceAccountEmail) {
                        Scopes = new[] {BigqueryService.Scope.Bigquery}
                    }.FromCertificate(certificate));
            }
            
            return new BigqueryService(new BaseClientService.Initializer {
                HttpClientInitializer = credential,
                ApplicationName = applicationName
            });
        }

        public override void Close() {
            CheckDisposed();
            if(!IsOpened)
                return;
            state = ConnectionState.Closed;
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

        public string[] GetTableNames() {
            CheckDisposed();
            CheckOpen();
            TableList tableList;
            try {
                tableList = Service.Tables.List(ProjectId, DataSetId).Execute();
            }
            catch(GoogleApiException e) {
                throw e.Wrap();
            }
            return tableList.Tables.Select(t => t.Id.Split('.')[1]).ToArray();
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

        protected override DbCommand CreateDbCommand() {
            return CreateCommand();
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

        object ICloneable.Clone() {
            return new BigQueryConnection(ConnectionString);
        }
    }
}
