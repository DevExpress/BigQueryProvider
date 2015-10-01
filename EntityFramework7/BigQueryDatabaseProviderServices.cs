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
using System.Reflection;
using DevExpress.DataAccess.BigQuery.EntityFarmework7.Metadata;
using DevExpress.DataAccess.BigQuery.EntityFarmework7.Migrations;
using DevExpress.DataAccess.BigQuery.EntityFarmework7.Update;
using DevExpress.DataAccess.BigQuery.EntityFarmework7.ValueGeneration;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Conventions.Internal;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Query.Methods;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Update;
using Microsoft.Data.Entity.ValueGeneration;

namespace DevExpress.DataAccess.BigQuery.EntityFarmework7 {
    public class BigQueryDatabaseProviderServices : RelationalDatabaseProviderServices {
        public BigQueryDatabaseProviderServices([NotNull] IServiceProvider services)
            : base(services) {
        }

        public override string InvariantName => GetType().GetTypeInfo().Assembly.GetName().Name;
        public override IDatabase Database => GetService<BigQueryDatabase>();
        public override IDatabaseCreator Creator => GetService<BigQueryDatabaseCreator>();
        public override IRelationalConnection RelationalConnection => GetService<BigQueryDatabaseConnection>();
        public override IRelationalDatabaseCreator RelationalDatabaseCreator => GetService<BigQueryDatabaseCreator>();
        public override IConventionSetBuilder ConventionSetBuilder => GetService<BigQueryConventionSetBuilder>();
        public override IMigrationsAnnotationProvider MigrationsAnnotationProvider => GetService<BigQueryMigrationsAnnotationProvider>();
        public override IHistoryRepository HistoryRepository => GetService<BigQueryHistoryRepository>();
        public override IMigrationsSqlGenerator MigrationsSqlGenerator => GetService<BigQueryMigrationsSqlGenerator>();
        public override IModelSource ModelSource => GetService<BigQueryModelSource>();
        public override IUpdateSqlGenerator UpdateSqlGenerator => GetService<BigQueryUpdateSqlGenerator>();
        public override IValueGeneratorCache ValueGeneratorCache => GetService<BigQueryValueGeneratorCache>();
        public override IRelationalTypeMapper TypeMapper => GetService<BigQueryTypeMapper>();
        public override IModificationCommandBatchFactory ModificationCommandBatchFactory => GetService<BigQueryModificationCommandBatchFactory>();
        public override IRelationalValueBufferFactoryFactory ValueBufferFactoryFactory => GetService<TypedValueBufferFactoryFactory>();
        public override IRelationalMetadataExtensionProvider MetadataExtensionProvider => GetService<BigQueryMetadataExtensionProvider>();
        public override IMethodCallTranslator CompositeMethodCallTranslator => GetService<BigQueryCompositeMethodCallTranslator>();
        public override IMemberTranslator CompositeMemberTranslator => GetService<BigQueryCompositeMemberTranslator>();
    }
}
