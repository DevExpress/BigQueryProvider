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

namespace DevExpress.DataAccess.BigQuery.EntityFarmework7
{
    // TODO: Implementation is copied from Npgsql...
    public class BigQueryTypeMapper : RelationalTypeMapper
    {
        readonly RelationalTypeMapping typeMappingString      = new RelationalTypeMapping("string");
        readonly RelationalTypeMapping typeMappingInteger     = new RelationalTypeMapping("integer");
        readonly RelationalTypeMapping typeMappingFloat       = new RelationalTypeMapping("float");
        readonly RelationalTypeMapping typeMappingBoolean     = new RelationalTypeMapping("bool", DbType.Boolean);
        readonly RelationalTypeMapping typeMappingTimestamp   = new RelationalTypeMapping("timestamp", DbType.DateTime);
        // TODO: RECORD

        private readonly Dictionary<string, RelationalTypeMapping> simpleNameMappings;
        private readonly Dictionary<Type, RelationalTypeMapping> simpleMappings;

        public BigQueryTypeMapper() {
            this.simpleNameMappings = new Dictionary<string, RelationalTypeMapping>(StringComparer.OrdinalIgnoreCase);
            this.simpleMappings = new Dictionary<Type, RelationalTypeMapping> {
                {typeof(byte), this.typeMappingInteger},
                {typeof(short), this.typeMappingInteger},
                {typeof(int), this.typeMappingInteger},
                {typeof(long), this.typeMappingInteger},
                {typeof(float), this.typeMappingFloat},
                {typeof(double), this.typeMappingFloat},
                {typeof(decimal), this.typeMappingFloat},
                {typeof(string), this.typeMappingString},
                {typeof(byte[]), this.typeMappingString},
                {typeof(DateTime), this.typeMappingTimestamp},
                {typeof(DateTimeOffset), this.typeMappingTimestamp},
                {typeof(TimeSpan), this.typeMappingTimestamp},
                {typeof(bool), this.typeMappingBoolean},

                //{ typeof(Guid),           this.typeMappingUuid        },
                //{ typeof(BitArray),       this.typeMappingBit         },
                //{ typeof(char), _int },
                //{ typeof(sbyte), new RelationalTypeMapping("smallint") },
                //{ typeof(ushort), new RelationalTypeMapping("int") },
                //{ typeof(uint), new RelationalTypeMapping("bigint") },
                //{ typeof(ulong), new RelationalTypeMapping("numeric(20, 0)") },
            };
        }

        protected override string GetColumnType(IProperty property) {
            return property.BigQuery().ColumnType;
        }

        public override RelationalTypeMapping GetDefaultMapping(Type clrType) {
            Check.NotNull(clrType, nameof(clrType));
            if(clrType == typeof(decimal)) {
                return this.typeMappingFloat;
            }
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
