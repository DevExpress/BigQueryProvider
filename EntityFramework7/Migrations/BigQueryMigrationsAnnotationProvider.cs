using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;

namespace DevExpress.DataAccess.BigQuery.EntityFarmework7.Migrations {
    public class BigQueryMigrationsAnnotationProvider : MigrationsAnnotationProvider {
        public override IEnumerable<IAnnotation> For(IProperty property) {
            //TODO: Write here something for value generation
            if(property.ValueGenerated == ValueGenerated.OnAdd && property.ClrType.IsIntegerForSerial()) {
                //yield return new Annotation(NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.Serial, true);
                yield return null;
            }

            // TODO: Named sequences

            // TODO: We don't support ValueGenerated.OnAddOrUpdate, so should we throw an exception?
            // Other providers don't seem to...
        }
    }
}
