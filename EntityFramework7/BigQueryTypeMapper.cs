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

        readonly Dictionary<Type, RelationalTypeMapping> simpleMappings;
        readonly Dictionary<string, RelationalTypeMapping> simpleNameMappings;

        protected override IReadOnlyDictionary<Type, RelationalTypeMapping> SimpleMappings { get { return this.simpleMappings; } }
        protected override IReadOnlyDictionary<string, RelationalTypeMapping> SimpleNameMappings { get { return this.simpleNameMappings; } }

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
    }
}
