using DevExpress.DataAccess.BigQuery.EntityFarmework7.Metadata;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Utilities;

// ReSharper disable once CheckNamespace

namespace Microsoft.Data.Entity {
    public static class BigQueryMetadataExtensions {
        public static IRelationalEntityTypeAnnotations BigQuery([NotNull] this IEntityType entityType) {
            return new RelationalEntityTypeAnnotations(Check.NotNull(entityType, nameof(entityType)), BigQueryAnnotationNames.Prefix);
        }

        public static RelationalEntityTypeAnnotations BigQuery([NotNull] this EntityType entityType) {
            return (RelationalEntityTypeAnnotations)BigQuery((IEntityType)entityType);
        }

        public static IRelationalForeignKeyAnnotations BigQuery([NotNull] this IForeignKey foreignKey) {
            return new RelationalForeignKeyAnnotations(Check.NotNull(foreignKey, nameof(foreignKey)), BigQueryAnnotationNames.Prefix);
        }

        public static RelationalForeignKeyAnnotations BigQuery([NotNull] this ForeignKey foreignKey) {
            return (RelationalForeignKeyAnnotations)BigQuery((IForeignKey)foreignKey);
        }

        public static IRelationalIndexAnnotations BigQuery([NotNull] this IIndex index) {
            return new RelationalIndexAnnotations(Check.NotNull(index, nameof(index)), BigQueryAnnotationNames.Prefix);
        }

        public static RelationalIndexAnnotations BigQuery([NotNull] this Index index) {
            return (RelationalIndexAnnotations)BigQuery((IIndex)index);
        }

        public static IRelationalKeyAnnotations BigQuery([NotNull] this IKey key) {
            return new RelationalKeyAnnotations(Check.NotNull(key, nameof(key)), BigQueryAnnotationNames.Prefix);
        }

        public static RelationalKeyAnnotations BigQuery([NotNull] this Key key) {
            return (RelationalKeyAnnotations)BigQuery((IKey)key);
        }

        public static RelationalModelAnnotations BigQuery([NotNull] this Model model) {
            return (RelationalModelAnnotations)BigQuery((IModel)model);
        }

        public static IRelationalModelAnnotations BigQuery([NotNull] this IModel model) {
            return new RelationalModelAnnotations(Check.NotNull(model, nameof(model)), BigQueryAnnotationNames.Prefix);
        }

        public static IRelationalPropertyAnnotations BigQuery([NotNull] this IProperty property) {
            return new RelationalPropertyAnnotations(Check.NotNull(property, nameof(property)), BigQueryAnnotationNames.Prefix);
        }

        public static RelationalPropertyAnnotations BigQuery([NotNull] this Property property) {
            return (RelationalPropertyAnnotations)BigQuery((IProperty)property);
        }
    }
}
