// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;
using DevExpress.DataAccess.BigQuery.EntityFarmework7.Metadata;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Internal;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Utilities;

namespace DevExpress.DataAccess.BigQuery.EntityFarmework7.Migrations
{
    public class BigQueryHistoryRepository : HistoryRepository //TODO: Implement it
    {
        private readonly BigQueryUpdateSqlGenerator _sql;

        public BigQueryHistoryRepository(
            [NotNull] IDatabaseCreator databaseCreator,
            [NotNull] ISqlStatementExecutor executor,
            [NotNull] BigQueryDatabaseConnection connection,
            [NotNull] IDbContextOptions options,
            [NotNull] IMigrationsModelDiffer modelDiffer,
            [NotNull] BigQueryMigrationsSqlGenerator sqlGenerator,
            [NotNull] BigQueryMetadataExtensionProvider annotations,
            [NotNull] BigQueryUpdateSqlGenerator sql)
            : base(
                  databaseCreator,
                  executor,
                  connection,
                  options,
                  modelDiffer,
                  sqlGenerator,
                  annotations,
                  sql)
        {
            Check.NotNull(sql, nameof(sql));

            this._sql = sql;
        }

        protected override string ExistsSql
        {
            get
            {
                var builder = new StringBuilder();

                builder.Append("SELECT 1 FROM pg_catalog.pg_class c JOIN pg_catalog.pg_namespace n ON n.oid=c.relnamespace WHERE ");

                if (TableSchema != null)
                {
                    builder
                        .Append("n.nspname='")
                        .Append(this._sql.EscapeLiteral(TableSchema))
                        .Append("' AND ");
                }

                builder
                    .Append("c.relname='")
                    .Append(this._sql.EscapeLiteral(TableName))
                    .Append("';");

                return builder.ToString();
            }
        }

        protected override bool Exists(object value) => value != null && value != DBNull.Value;

        public override string GetInsertScript(HistoryRow row)
        {
            Check.NotNull(row, nameof(row));

            return new StringBuilder().Append("INSERT INTO ")
                .Append(this._sql.DelimitIdentifier(TableName, TableSchema))
                .Append(" (")
                .Append(this._sql.DelimitIdentifier(MigrationIdColumnName))
                .Append(", ")
                .Append(this._sql.DelimitIdentifier(ProductVersionColumnName))
                .AppendLine(")")
                .Append("VALUES ('")
                .Append(this._sql.EscapeLiteral(row.MigrationId))
                .Append("', '")
                .Append(this._sql.EscapeLiteral(row.ProductVersion))
                .Append("');")
                .ToString();
        }

        public override string GetDeleteScript(string migrationId)
        {
            Check.NotEmpty(migrationId, nameof(migrationId));

            return new StringBuilder().Append("DELETE FROM ")
                .AppendLine(this._sql.DelimitIdentifier(TableName, TableSchema))
                .Append("WHERE ")
                .Append(this._sql.DelimitIdentifier(MigrationIdColumnName))
                .Append(" = '")
                .Append(this._sql.EscapeLiteral(migrationId))
                .Append("';")
                .ToString();
        }

        public override string GetCreateIfNotExistsScript()
        {
            var builder = new IndentedStringBuilder();

            builder.Append("IF NOT EXISTS (SELECT 1 FROM pg_catalog.pg_class c JOIN pg_catalog.pg_namespace n ON n.oid=c.relnamespace WHERE ");

            if (TableSchema != null)
            {
                builder
                    .Append("n.nspname='")
                    .Append(this._sql.EscapeLiteral(TableSchema))
                    .Append("' AND ");
            }

            builder
                .Append("c.relname='")
                .Append(this._sql.EscapeLiteral(TableName))
                .Append("') THEN");


            builder.AppendLines(GetCreateScript());

            builder.Append(GetEndIfScript());

            return builder.ToString();
        }

        public override string GetBeginIfNotExistsScript(string migrationId)
        {
            Check.NotEmpty(migrationId, nameof(migrationId));

            return new StringBuilder()
                .Append("IF NOT EXISTS(SELECT * FROM ")
                .Append(this._sql.DelimitIdentifier(TableName, TableSchema))
                .Append(" WHERE ")
                .Append(this._sql.DelimitIdentifier(MigrationIdColumnName))
                .Append(" = '")
                .Append(this._sql.EscapeLiteral(migrationId))
                .AppendLine("')")
                .Append("BEGIN")
                .ToString();
        }

        public override string GetBeginIfExistsScript(string migrationId)
        {
            Check.NotEmpty(migrationId, nameof(migrationId));

            return new StringBuilder()
                .Append("IF EXISTS(SELECT * FROM ")
                .Append(this._sql.DelimitIdentifier(TableName, TableSchema))
                .Append(" WHERE ")
                .Append(this._sql.DelimitIdentifier(MigrationIdColumnName))
                .Append(" = '")
                .Append(this._sql.EscapeLiteral(migrationId))
                .AppendLine("')")
                .Append("BEGIN")
                .ToString();
        }

        public override string GetEndIfScript() => "END IF";
    }
}
