﻿using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Bigquery.v2;
using Google.Apis.Bigquery.v2.Data;
using Google.Apis.Services;

namespace DevExpress.DataAccess.BigQuery {
    public class BigQueryConnection : DbConnection, ICloneable {
        ConnectionState state;
        readonly DbConnectionStringBuilder connectionStringBuilder = new DbConnectionStringBuilder();
        bool disposed = false;

       internal string ProjectId {
            get {
                return (string) connectionStringBuilder["ProjectId"];
            }
        }

        string ServiceAccountEmail {
            get {
                return (string)connectionStringBuilder["ServiceAccountEmail"];
            }
        }

        string PrivateKeyFileName {
            get {
                return (string)connectionStringBuilder["PrivateKeyFileName"];
            }
        }

       internal string DataSetId {
            get {
                return (string)connectionStringBuilder["DataSetId"];
            }

            set {
                if((string) connectionStringBuilder["DataSetId"] == value)
                    return;
                connectionStringBuilder["DataSetId"] = value;
                ConnectionString = connectionStringBuilder.ConnectionString;
            }
        }

        internal BigqueryService Service { get; private set; }

        public BigQueryConnection() {}

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
                this.Close();
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
                await InitializeServiceAsync();
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
            X509Certificate2 certificate = new X509Certificate2(PrivateKeyFileName, "notasecret", X509KeyStorageFlags.Exportable);
            ServiceAccountCredential credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(ServiceAccountEmail) {
                Scopes = new[] {BigqueryService.Scope.Bigquery}
            }.FromCertificate(certificate));

            Service = new BigqueryService(new BaseClientService.Initializer() {
                HttpClientInitializer = credential,
                ApplicationName = "DevExpress.DataAccess.BigQuery ADO.NET Provider"
            });
            JobsResource.ListRequest listRequest = Service.Jobs.List(ProjectId);
            listRequest.Execute();
            state = ConnectionState.Open;
        }

        async Task InitializeServiceAsync() {
            CheckDisposed();
            state = ConnectionState.Connecting;
            X509Certificate2 certificate = new X509Certificate2(PrivateKeyFileName, "notasecret", X509KeyStorageFlags.Exportable);
            ServiceAccountCredential credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(ServiceAccountEmail) {
                Scopes = new[] { BigqueryService.Scope.Bigquery }
            }.FromCertificate(certificate));

            Service = new BigqueryService(new BaseClientService.Initializer() {
                HttpClientInitializer = credential,
                ApplicationName = "DevExpress.DataAccess.BigQuery ADO.NET Provider"
            });
            JobsResource.ListRequest listRequest = Service.Jobs.List(ProjectId);
            await listRequest.ExecuteAsync();
            state = ConnectionState.Open;
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
            this.CheckOpen();
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

        protected override DbCommand CreateDbCommand() {
            CheckDisposed();
            this.CheckOpen();
            return new BigQueryCommand() {Connection = this};
        }

        public override string DataSource {
            get {
                this.CheckOpen();
                return this.ProjectId;
            }
        }

        public override string Database {
            get {
                this.CheckOpen();
                return this.DataSetId;
            }
        }

        public override string ServerVersion {
            get { throw new NotSupportedException();}
        }

        public override ConnectionState State {
            get { return state; }
        }

        object ICloneable.Clone() {
            return new BigQueryConnection(ConnectionString);
        }
    }
}