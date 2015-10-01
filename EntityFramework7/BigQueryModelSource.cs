// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity.Internal;
using Microsoft.Data.Entity.Metadata.Conventions.Internal;

namespace DevExpress.DataAccess.BigQuery.EntityFarmework7 {
    public class BigQueryModelSource : ModelSource {
        public BigQueryModelSource([NotNull] IDbSetFinder setFinder, [NotNull] ICoreConventionSetBuilder coreConventionSetBuilder)
            : base(setFinder, coreConventionSetBuilder) {
        }
    }
}
