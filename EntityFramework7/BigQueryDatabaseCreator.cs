// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using Google.Apis.Bigquery.v2;
using Google.Apis.Bigquery.v2.Data;
using JetBrains.Annotations;
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

        BigQueryConnection Connection { get { return (BigQueryConnection)this.databaseConnection.DbConnection; } }

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
            Connection.Open();
            try {
                BigqueryService service = GetService(Connection);
                if(service == null)
                    throw new Exception("Service not found");
                DbConnectionStringBuilder builder = new DbConnectionStringBuilder {ConnectionString = Connection.ConnectionString};
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
                Connection.Close();
            }
        }

        public override void CreateTables() {
            DbConnectionStringBuilder builder = new DbConnectionStringBuilder { ConnectionString = Connection.ConnectionString };
            string projectId = (string)builder["ProjectId"];
            string databaseName = (string)builder["DatasetId"];
            IReadOnlyList<MigrationOperation> operations = modelDiffer.GetDifferences(null, Model);
            foreach(MigrationOperation migrationOperation in operations) {
                CreateTableOperation createTableOperation = migrationOperation as CreateTableOperation;
                if(createTableOperation != null) {
                    List<TableFieldSchema> tableSchema = createTableOperation.Columns.Select(c => new TableFieldSchema {
                        Name = c.Name,
                        Type = c.ColumnType
                    }).ToList(); //TODO: Review column types and Mode = "REQUIRED" for not nullable fields
                    Table table = new Table {
                        Schema = new TableSchema { Fields = tableSchema },
                        TableReference = new TableReference {
                            ProjectId = projectId,
                            DatasetId = databaseName,
                            TableId = createTableOperation.Name
                        }
                    };
                    Connection.Open();
                    try {
                        BigqueryService service = GetService(Connection);
                        if(service == null)
                            throw new Exception("Service not found");
                        TableList tableList = service.Tables.List(projectId, databaseName).Execute();
                        if(tableList.Tables != null && tableList.Tables.Any(t => t.TableReference.TableId == table.TableReference.TableId))
                            service.Tables.Delete(projectId, databaseName, table.TableReference.TableId).Execute();
                        service.Tables.Insert(table, projectId, databaseName).Execute();
                    } finally {
                        Connection.Close();
                    }

                }
                //TODO: Implement other operations if possible
                throw new NotImplementedException();
            }
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
                return false;
            }
        }

        public override void Delete() {
            BigQueryConnection connection = (BigQueryConnection)this.databaseConnection.DbConnection;
            connection.Open();
            try {
                BigqueryService service = GetService(connection);
                if(service == null)
                    throw new Exception("Service not found");
                DbConnectionStringBuilder builder = new DbConnectionStringBuilder { ConnectionString = connection.ConnectionString };
                string projectId = (string)builder["ProjectId"];
                string databaseName = (string)builder["DatasetId"];
                DatasetList dataSetList = service.Datasets.List(projectId).Execute();
                if(dataSetList.Datasets == null || dataSetList.Datasets.Any(d => d.DatasetReference.DatasetId == databaseName))
                    throw new Exception("Dataset not found");
                service.Datasets.Delete(projectId, databaseName).Execute();
            } finally {
                connection.Close();
            }
        }
    }
}
