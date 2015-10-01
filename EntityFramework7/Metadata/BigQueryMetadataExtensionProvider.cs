// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;

namespace DevExpress.DataAccess.BigQuery.EntityFarmework7.Metadata {
    public class BigQueryMetadataExtensionProvider : IRelationalMetadataExtensionProvider { //TODO: Check it for BigQuery
        public virtual IRelationalEntityTypeAnnotations For(IEntityType entityType) => entityType.BigQuery();
        public virtual IRelationalForeignKeyAnnotations For(IForeignKey foreignKey) => foreignKey.BigQuery();
        public virtual IRelationalIndexAnnotations For(IIndex index) => index.BigQuery();
        public virtual IRelationalKeyAnnotations For(IKey key) => key.BigQuery();
        public virtual IRelationalModelAnnotations For(IModel model) => model.BigQuery();
        public virtual IRelationalPropertyAnnotations For(IProperty property) => property.BigQuery();
    }
}
