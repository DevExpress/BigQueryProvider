// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Entity.Query;
using Microsoft.Data.Entity.Query.Methods;

namespace DevExpress.DataAccess.BigQuery.EntityFarmework7 {
    public class BigQueryCompositeMemberTranslator : RelationalCompositeMemberTranslator { //TODO: Implement it
        readonly List<IMemberTranslator> sqlServerTranslators = new List<IMemberTranslator> {
            //new StringLengthTranslator(),
            //new DateTimeNowTranslator()
        };

        protected override IReadOnlyList<IMemberTranslator> Translators { get { return base.Translators.Concat(this.sqlServerTranslators).ToList(); } }
    }
}
