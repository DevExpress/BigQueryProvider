// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;

namespace DevExpress.DataAccess.BigQuery.EntityFarmework7.Extensions {
    public class BigQueryContextOptionsBuilder : RelationalDbContextOptionsBuilder<BigQueryContextOptionsBuilder, BigQueryOptionsExtension> {
        public BigQueryContextOptionsBuilder([NotNull] DbContextOptionsBuilder optionsBuilder)
            : base(optionsBuilder) {
        }

        protected override BigQueryOptionsExtension CloneExtension() {
            return new BigQueryOptionsExtension(OptionsBuilder.Options.GetExtension<BigQueryOptionsExtension>());
        }
    }
}
