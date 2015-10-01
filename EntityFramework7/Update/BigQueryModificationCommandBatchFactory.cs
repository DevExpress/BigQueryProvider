// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Update;

namespace DevExpress.DataAccess.BigQuery.EntityFarmework7.Update {
    // TODO: Implement it
    public class BigQueryModificationCommandBatchFactory : ModificationCommandBatchFactory {
        public BigQueryModificationCommandBatchFactory([NotNull] IUpdateSqlGenerator sqlGenerator)
            : base(sqlGenerator) {
        }

        public override ModificationCommandBatch Create(IDbContextOptions options, IRelationalMetadataExtensionProvider metadataExtensionProvider) {
            return new BigQueryModificationCommandBatch(UpdateSqlGenerator);
        }
    }
}
