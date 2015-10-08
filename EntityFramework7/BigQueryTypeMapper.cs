// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Utilities;

namespace DevExpress.DataAccess.BigQuery.EntityFarmework7 {
    public class BigQueryTypeMapper : RelationalTypeMapper {
        readonly RelationalTypeMapping mappingString = new RelationalTypeMapping("string", DbType.String);
        readonly RelationalTypeMapping mappingInteger = new RelationalTypeMapping("integer", DbType.Int64);
        readonly RelationalTypeMapping mappingFloat = new RelationalTypeMapping("float", DbType.Single);
        readonly RelationalTypeMapping mappingBoolean = new RelationalTypeMapping("boolean", DbType.Boolean);
        readonly RelationalTypeMapping mappingTimestamp = new RelationalTypeMapping("timestamp", DbType.DateTime);
        readonly RelationalTypeMapping mappingRecord = new RelationalTypeMapping("record", DbType.Object);

        readonly Dictionary<string, RelationalTypeMapping> simpleNameMappings;
        readonly Dictionary<Type, RelationalTypeMapping> simpleMappings;

        public BigQueryTypeMapper() {
            this.simpleNameMappings = new Dictionary<string, RelationalTypeMapping>(StringComparer.OrdinalIgnoreCase) {
                {"string", mappingString},
                {"integer", mappingInteger},
                {"float", mappingFloat},
                {"boolean", mappingBoolean},
                {"timestamp", mappingTimestamp},
                {"record", mappingRecord},
            };

            this.simpleMappings = new Dictionary<Type, RelationalTypeMapping> {
                {typeof(long), this.mappingInteger},
                {typeof(ulong), this.mappingInteger},
                {typeof(int), this.mappingInteger},
                {typeof(uint), this.mappingInteger},
                {typeof(short), this.mappingInteger},
                {typeof(ushort), this.mappingInteger},
                {typeof(sbyte), this.mappingInteger},
                {typeof(byte), this.mappingInteger},

                {typeof(float), this.mappingFloat},
                {typeof(double), this.mappingFloat},
                {typeof(decimal), this.mappingFloat},

                {typeof(string), this.mappingString},
                {typeof(char), this.mappingString},
                {typeof(byte[]), this.mappingString},

                {typeof(DateTime), this.mappingTimestamp},
                {typeof(DateTimeOffset), this.mappingTimestamp},
                {typeof(TimeSpan), this.mappingTimestamp},

                {typeof(bool), this.mappingBoolean},

                {typeof(object), this.mappingRecord},
            };
        }

        protected override string GetColumnType(IProperty property) {
            return property.BigQuery().ColumnType;
        }

        public override RelationalTypeMapping GetDefaultMapping(Type clrType) {
            Check.NotNull(clrType, nameof(clrType));
            return base.GetDefaultMapping(clrType);
        }

        protected override RelationalTypeMapping GetCustomMapping(IProperty property) {
            RelationalTypeMapping relationalTypeMapping = base.GetCustomMapping(property);
            return relationalTypeMapping;
        }

        public override RelationalTypeMapping MapPropertyType(IProperty property) {
            RelationalTypeMapping relationalTypeMapping = base.MapPropertyType(property);
            return relationalTypeMapping;
        }

        protected override RelationalTypeMapping MapString(IProperty property, int maxBoundedLength, Func<int, RelationalTypeMapping> boundedMapping, RelationalTypeMapping unboundedMapping, RelationalTypeMapping defaultMapping, RelationalTypeMapping keyMapping = null) {
            RelationalTypeMapping relationalTypeMapping = base.MapString(property, maxBoundedLength, boundedMapping, unboundedMapping, defaultMapping, keyMapping);
            return relationalTypeMapping;
        }

        protected override RelationalTypeMapping MapByteArray(IProperty property, int maxBoundedLength, Func<int, RelationalTypeMapping> boundedMapping, RelationalTypeMapping unboundedMapping, RelationalTypeMapping defaultMapping, RelationalTypeMapping keyMapping = null, RelationalTypeMapping rowVersionMapping = null) {
            RelationalTypeMapping relationalTypeMapping = base.MapByteArray(property, maxBoundedLength, boundedMapping, unboundedMapping, defaultMapping, keyMapping, rowVersionMapping);
            return relationalTypeMapping;
        }

        protected override IReadOnlyDictionary<Type, RelationalTypeMapping> SimpleMappings { get { return this.simpleMappings; } }

        protected override IReadOnlyDictionary<string, RelationalTypeMapping> SimpleNameMappings { get { return this.simpleNameMappings; } }
    }
}
