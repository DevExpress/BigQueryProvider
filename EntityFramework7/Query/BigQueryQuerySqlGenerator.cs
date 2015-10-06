// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Query.Expressions;
using Microsoft.Data.Entity.Query.Sql;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Utilities;

namespace DevExpress.DataAccess.BigQuery.EntityFarmework7.Query {
    public class BigQueryQuerySqlGenerator : DefaultQuerySqlGenerator {
        readonly BigQueryDatabaseConnection connection;

        public BigQueryQuerySqlGenerator([NotNull] SelectExpression selectExpression, [NotNull]IRelationalTypeMapper typeMapper, [NotNull]BigQueryDatabaseConnection connection) :
            base(selectExpression, typeMapper) {
            this.connection = connection;
        }

        protected override string ConcatOperator { get { return "+"; } }

        protected override string TrueLiteral { get { return "TRUE"; } }

        protected override string FalseLiteral { get { return "FALSE"; } }

        protected override string TypedTrueLiteral { get { return "BOOLEAN(1)"; } }

        protected override string TypedFalseLiteral { get { return "BOOLEAN(0)"; } }

        protected override string DelimitIdentifier(string identifier) {
            return string.Format("[{0}]", identifier);
        }

        public override Expression VisitTable(TableExpression tableExpression) {
            Sql.Append(DelimitIdentifier(string.Format("{0}.{1}", connection.DbConnection.Database, tableExpression.Table)));
            Sql.Append(" ");
            Sql.Append(string.IsNullOrWhiteSpace(tableExpression.Alias) ? DelimitIdentifier(tableExpression.Table) : DelimitIdentifier(tableExpression.Alias));
            return tableExpression;
        }

        public override Expression VisitColumn(ColumnExpression columnExpression) {
            string tableAlias = string.IsNullOrWhiteSpace(columnExpression.TableAlias) ? this.connection.DbConnection.Database : columnExpression.TableAlias;
            Sql.Append(DelimitIdentifier(string.Format("{0}.{1}", tableAlias, columnExpression.Name)));
            return columnExpression;
        }

        protected override void GenerateTop(SelectExpression selectExpression) {
            // Handled by GenerateLimitOffset
        }

        protected override void GenerateLimitOffset(SelectExpression selectExpression) {
            Check.NotNull(selectExpression, nameof(selectExpression));
            if(selectExpression.Limit != null)
                Sql.AppendLine().Append("LIMIT ").Append(selectExpression.Limit);
            if(selectExpression.Offset != null)
                throw new NotSupportedException();
        }
    }
}
