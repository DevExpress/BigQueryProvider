// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Data.Common;
using DevExpress.DataAccess.BigQuery.EntityFarmework7;
using DevExpress.DataAccess.BigQuery.EntityFarmework7.Extensions;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.Data.Entity {
    public static class BigQueryDbContextOptionsExtensions {
        public static BigQueryContextOptionsBuilder UseBigQuery([NotNull] this DbContextOptionsBuilder optionsBuilder, [NotNull] string connectionString) {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));
            Check.NotEmpty(connectionString, nameof(connectionString));

            BigQueryOptionsExtension extension = GetOrCreateExtension(optionsBuilder);
            extension.ConnectionString = connectionString;
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            return new BigQueryContextOptionsBuilder(optionsBuilder);
        }

        public static BigQueryContextOptionsBuilder UseBigQuery([NotNull] this DbContextOptionsBuilder optionsBuilder, [NotNull] DbConnection connection) {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));
            Check.NotNull(connection, nameof(connection));

            BigQueryOptionsExtension extension = GetOrCreateExtension(optionsBuilder);
            extension.Connection = connection;
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            return new BigQueryContextOptionsBuilder(optionsBuilder);
        }

        static BigQueryOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder) {
            BigQueryOptionsExtension existing = optionsBuilder.Options.FindExtension<BigQueryOptionsExtension>();
            return existing != null ? new BigQueryOptionsExtension(existing) : new BigQueryOptionsExtension();
        }
    }
}
