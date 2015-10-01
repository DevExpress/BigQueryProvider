// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Storage;

namespace DevExpress.DataAccess.BigQuery.EntityFarmework7
{
    // TODO: Implementation is copied from Npgsql...
    public class BigQueryTypeMapper : RelationalTypeMapper
    {
        readonly RelationalTypeMapping typeMappingTinyint     = new RelationalTypeMapping("integer", DbType.Byte);
        readonly RelationalTypeMapping typeMappingSmallint    = new RelationalTypeMapping("integer", DbType.Int16);
        readonly RelationalTypeMapping typeMappingInt         = new RelationalTypeMapping("integer", DbType.Int32);
        readonly RelationalTypeMapping typeMappingBigint      = new RelationalTypeMapping("integer", DbType.Int64);
        readonly RelationalTypeMapping typeMappingReal        = new RelationalTypeMapping("float", DbType.Single);
        readonly RelationalTypeMapping typeMappingDouble      = new RelationalTypeMapping("float", DbType.Double);
        readonly RelationalTypeMapping typeMappingDecimal     = new RelationalTypeMapping("float", DbType.Decimal);

        // TODO: Look at the SqlServerMaxLengthMapping optimization, it may be relevant for us too
        readonly RelationalTypeMapping typeMappingText        = new RelationalTypeMapping("text", DbType.String);
        // TODO: The other text types, char
        readonly RelationalTypeMapping typeMappingByteArray   = new RelationalTypeMapping("bytea", DbType.Binary);

        readonly RelationalTypeMapping typeMappingTimestamp   = new RelationalTypeMapping("timestamp", DbType.DateTime);
        readonly RelationalTypeMapping typeMappingTimestamptz = new RelationalTypeMapping("timestamptz", DbType.DateTimeOffset);
        readonly RelationalTypeMapping typeMappingDate        = new RelationalTypeMapping("date", DbType.Date);
        readonly RelationalTypeMapping typeMappingTime        = new RelationalTypeMapping("time", DbType.Time);
        // TODO: DbType?
        readonly RelationalTypeMapping typeMappingTimetz      = new RelationalTypeMapping("timetz");
        readonly RelationalTypeMapping typeMappingInterval    = new RelationalTypeMapping("interval");

        readonly RelationalTypeMapping typeMappingUuid        = new RelationalTypeMapping("uuid", DbType.Guid);
        readonly RelationalTypeMapping typeMappingBit         = new RelationalTypeMapping("bit");
        readonly RelationalTypeMapping typeMappingBool        = new RelationalTypeMapping("bool", DbType.Boolean);

        private readonly Dictionary<string, RelationalTypeMapping> simpleNameMappings;
        private readonly Dictionary<Type, RelationalTypeMapping> simpleMappings;

        public BigQueryTypeMapper()
        {
            this.simpleNameMappings
                = new Dictionary<string, RelationalTypeMapping>(StringComparer.OrdinalIgnoreCase)
                {
                    { "integer",          this.typeMappingBigint      },
                    { "float",            this.typeMappingDouble      },
                    { "decimal",          this.typeMappingDecimal     },
                    { "numeric",          this.typeMappingDecimal     },
                    { "text",             this.typeMappingText        },
                    { "bytea",            this.typeMappingByteArray   },
                    { "timestamp",        this.typeMappingTimestamp   },
                    { "timestamptz",      this.typeMappingTimestamptz },
                    { "date",             this.typeMappingDate        },
                    { "time",             this.typeMappingTime        },
                    { "timetz",           this.typeMappingTimetz      },
                    { "interval",         this.typeMappingInterval    },
                    { "uuid",             this.typeMappingUuid        },
                    { "bit",              this.typeMappingBit         },
                    { "bool",             this.typeMappingBool        },
                };

            this.simpleMappings
                = new Dictionary<Type, RelationalTypeMapping>
                {
                    { typeof(byte),           this.typeMappingTinyint     },
                    { typeof(short),          this.typeMappingSmallint    },
                    { typeof(int),            this.typeMappingInt         },
                    { typeof(long),           this.typeMappingBigint      },
                    { typeof(float),          this.typeMappingReal        },
                    { typeof(double),         this.typeMappingDouble      },
                    { typeof(decimal),        this.typeMappingDecimal     },
                    { typeof(string),         this.typeMappingText        },
                    { typeof(byte[]),         this.typeMappingByteArray       },
                    { typeof(DateTime),       this.typeMappingTimestamp   },
                    { typeof(DateTimeOffset), this.typeMappingTimestamptz },
                    { typeof(TimeSpan),       this.typeMappingTime        },
                    { typeof(Guid),           this.typeMappingUuid        },
                    { typeof(BitArray),       this.typeMappingBit         },
                    { typeof(bool),           this.typeMappingBool        },

                    //{ typeof(char), _int },
                    //{ typeof(sbyte), new RelationalTypeMapping("smallint") },
                    //{ typeof(ushort), new RelationalTypeMapping("int") },
                    //{ typeof(uint), new RelationalTypeMapping("bigint") },
                    //{ typeof(ulong), new RelationalTypeMapping("numeric(20, 0)") },
                };
        }

        protected override string GetColumnType(IProperty property) => property.BigQuery().ColumnType;

        protected override IReadOnlyDictionary<Type, RelationalTypeMapping> SimpleMappings { get { return this.simpleMappings; } }

        protected override IReadOnlyDictionary<string, RelationalTypeMapping> SimpleNameMappings { get { return this.simpleNameMappings; } }
    }
}
