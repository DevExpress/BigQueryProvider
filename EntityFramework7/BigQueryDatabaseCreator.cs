// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Bigquery.v2;
using Google.Apis.Bigquery.v2.Data;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Migrations.Operations;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Utilities;

namespace DevExpress.DataAccess.BigQuery.EntityFarmework7 {
    public class BigQueryDatabaseCreator : RelationalDatabaseCreator {
        static BigqueryService GetService(BigQueryConnection connection) {
            PropertyInfo serviceInfo = connection.GetType().GetProperty("Service", BindingFlags.Instance | BindingFlags.NonPublic);
            if(serviceInfo == null)
                return null;
            BigqueryService service = serviceInfo.GetValue(connection) as BigqueryService;
            return service;
        }

        readonly BigQueryDatabaseConnection databaseConnection;
        readonly IMigrationsModelDiffer modelDiffer;
        readonly IMigrationsSqlGenerator sqlGenerator;
        readonly ISqlStatementExecutor statementExecutor;

        public BigQueryDatabaseCreator(
            [NotNull] BigQueryDatabaseConnection connection,
            [NotNull] IMigrationsModelDiffer modelDiffer,
            [NotNull] IMigrationsSqlGenerator sqlGenerator,
            [NotNull] ISqlStatementExecutor statementExecutor,
            [NotNull] IModel model)
            : base(model) {
            Check.NotNull(connection, nameof(connection));
            Check.NotNull(modelDiffer, nameof(modelDiffer));
            Check.NotNull(sqlGenerator, nameof(sqlGenerator));
            Check.NotNull(statementExecutor, nameof(statementExecutor));

            this.databaseConnection = connection;
            this.modelDiffer = modelDiffer;
            this.sqlGenerator = sqlGenerator;
            this.statementExecutor = statementExecutor;
        }

        public override void Create() {
            BigQueryConnection connection = (BigQueryConnection)this.databaseConnection.DbConnection;
            connection.Open();
            try {
                BigqueryService service = GetService(connection);
                if(service == null)
                    throw new Exception("Service not found");
                DbConnectionStringBuilder builder = new DbConnectionStringBuilder {ConnectionString = connection.ConnectionString};
                string projectId = (string)builder["ProjectId"];
                string databaseName = (string)builder["DatasetId"];
                DatasetList dataSetList = service.Datasets.List(projectId).Execute();
                if(dataSetList.Datasets != null && dataSetList.Datasets.Any(d => d.DatasetReference.DatasetId == databaseName))
                    throw new Exception("Dataset allready exists");
                Dataset dataSet = new Dataset {
                    DatasetReference = new DatasetReference { DatasetId = databaseName, ProjectId = projectId }
                };
                service.Datasets.Insert(dataSet, projectId).Execute();
            } finally {
                connection.Close();
            }
        }

        //public override async Task CreateAsync(CancellationToken cancellationToken = default(CancellationToken)) {
        //    using(BigQueryDatabaseConnection masterConnection = this.databaseConnection.CreateMasterConnection()) {
        //        await this.statementExecutor.ExecuteNonQueryAsync(masterConnection, null, CreateCreateOperations(), cancellationToken);
        //        ClearPool();
        //    }
        //}

        public override void CreateTables() {
            this.statementExecutor.ExecuteNonQuery(this.databaseConnection, this.databaseConnection.DbTransaction, CreateSchemaCommands());
        }

        public override async Task CreateTablesAsync(CancellationToken cancellationToken = default(CancellationToken)) {
            await this.statementExecutor.ExecuteNonQueryAsync(this.databaseConnection, this.databaseConnection.DbTransaction, CreateSchemaCommands(), cancellationToken);
        }

        public override bool HasTables() {
            BigQueryConnection connection = (BigQueryConnection)this.databaseConnection.DbConnection;
            return connection.GetTableNames().Length + connection.GetViewNames().Length > 0;
        }

        public override bool Exists() {
            try {
                this.databaseConnection.Open();
                this.databaseConnection.Close();
                return true;
            } catch(BigQueryException e) {
                if(IsDoesNotExist(e)) {
                    return false;
                }

                throw;
            }
        }

        public override async Task<bool> ExistsAsync(CancellationToken cancellationToken = default(CancellationToken)) {
            try {
                await this.databaseConnection.OpenAsync(cancellationToken);
                this.databaseConnection.Close();
                return true;
            } catch(BigQueryException e) {
                if(IsDoesNotExist(e)) {
                    return false;
                }
                throw;
            }
        }

        public override void Delete() {
            //using(BigQueryDatabaseConnection masterConnection = this.databaseConnection.CreateMasterConnection()) {
            //    this.statementExecutor.ExecuteNonQuery(masterConnection, null, CreateDropCommands());
            //}
        }

        //public override async Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken)) {
        //    using(BigQueryDatabaseConnection masterConnection = this.databaseConnection.CreateMasterConnection()) {
        //        await this.statementExecutor.ExecuteNonQueryAsync(masterConnection, null, CreateDropCommands(), cancellationToken);
        //    }
        //}

        private IEnumerable<SqlBatch> CreateSchemaCommands() {
            return this.sqlGenerator.Generate(this.modelDiffer.GetDifferences(null, Model), Model);
        }

        private string CreateHasTablesCommand() {
            return @"
                 SELECT CASE WHEN COUNT(*) = 0 THEN 0 ELSE 1 END
                 FROM information_schema.tables
                 WHERE table_type = 'BASE TABLE' AND table_schema NOT IN ('pg_catalog', 'information_schema')
               ";
        }

        private IEnumerable<SqlBatch> CreateCreateOperations() {
            return this.sqlGenerator.Generate(new[] {new CreateDatabaseOperation {Name = this.databaseConnection.DbConnection.Database}});
        }

        static bool IsDoesNotExist(BigQueryException exception) {
            return exception.Message == "Database does not exist";
        }

        IEnumerable<SqlBatch> CreateDropCommands() {
            MigrationOperation[] operations = new MigrationOperation[] {
                // TODO Check DbConnection.Database always gives us what we want
                // Issue #775
                new DropDatabaseOperation {Name = this.databaseConnection.DbConnection.Database}
            };

            var masterCommands = this.sqlGenerator.Generate(operations);
            return masterCommands;
        }
    }
}
