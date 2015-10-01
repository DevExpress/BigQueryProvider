// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using DevExpress.DataAccess.BigQuery.EntityFarmework7;
using DevExpress.DataAccess.BigQuery.EntityFarmework7.Metadata;
using DevExpress.DataAccess.BigQuery.EntityFarmework7.Migrations;
using DevExpress.DataAccess.BigQuery.EntityFarmework7.Update;
using DevExpress.DataAccess.BigQuery.EntityFarmework7.ValueGeneration;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Framework.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Framework.DependencyInjection
{
    public static class BigQueryEntityServicesBuilderExtensions {
        public static EntityFrameworkServicesBuilder AddBigQuery([NotNull] this EntityFrameworkServicesBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.AddRelational().GetService()
                .AddSingleton<IDatabaseProvider, DatabaseProvider<BigQueryDatabaseProviderServices, BigQueryOptionsExtension>>()
                .TryAdd(new ServiceCollection()
                    .AddSingleton<BigQueryConventionSetBuilder>()
                    .AddSingleton<BigQueryValueGeneratorCache>()
                    .AddSingleton<BigQueryUpdateSqlGenerator>()
                    .AddSingleton<BigQueryTypeMapper>()
                    .AddSingleton<BigQueryModelSource>()
                    .AddSingleton<BigQueryMetadataExtensionProvider>()
                    .AddSingleton<BigQueryMigrationsAnnotationProvider>()
                    .AddScoped<BigQueryModificationCommandBatchFactory>()
                    .AddScoped<BigQueryDatabaseProviderServices>()
                    .AddScoped<BigQueryDatabase>()
                    .AddScoped<BigQueryDatabaseConnection>()
                    .AddScoped<BigQueryMigrationsSqlGenerator>()
                    .AddScoped<BigQueryDatabaseCreator>()
                    .AddScoped<BigQueryHistoryRepository>()
                    .AddScoped<BigQueryCompositeMethodCallTranslator>()
                    .AddScoped<BigQueryCompositeMemberTranslator>());

            return builder;
        }
    }
}
