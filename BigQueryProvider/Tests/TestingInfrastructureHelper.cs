/*
   Copyright 2015-2017 Developer Express Inc.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

#if DEBUGTEST
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Google.Apis.Bigquery.v2;
using Google.Apis.Bigquery.v2.Data;
using Xunit;

namespace DevExpress.DataAccess.BigQuery.Tests {
    public class TestingInfrastructureHelper : IDisposable {
        public TestingInfrastructureHelper() {
            connection = new BigQueryConnection(ConnectionStringHelper.P12ConnectionString);
            connection.Open();
        }

        public const string NatalityTableName = "natality";
        public const string Natality2TableName = "natality2";
        public const string NatalityViewName = "natalityview";
        public const string TimesTableName = "times";

        readonly BigQueryConnection connection;

        [Fact(Skip = "Explicit")]
        public void CreateDBTables() {
            CreateNatalityTable();
            CreateNatality2Table();
            CreateNatalityView();
            CreateTimesTable();
        }

        [Fact(Skip = "Explicit")]
        public void CreateTimesTable() {
            CreateDatasetIfRequired();

            var table = new Table {
                Schema = CreateTimesTableSchema(),
                TableReference = new TableReference {
                    DatasetId = connection.DataSetId,
                    ProjectId = connection.ProjectId,
                    TableId = TimesTableName
                }
            };

            InsertTable(table);

            UploadData(table);
        }

        TableSchema CreateTimesTableSchema() {
            var time = new TableFieldSchema {
                Name = "time",
                Type = "TIME",
                Mode = "NULLABLE"
            };

            var date = new TableFieldSchema {
                Name = "date",
                Type = "DATE",
                Mode = "NULLABLE"
            };

            var timestamp = new TableFieldSchema {
                Name = "timestamp",
                Type = "TIMESTAMP",
                Mode = "NULLABLE"
            };

            var datetime = new TableFieldSchema {
                Name = "datetime",
                Type = "DATETIME",
                Mode = "NULLABLE"
            };

            return new TableSchema { Fields = new List<TableFieldSchema> { time, date, timestamp, datetime  } };
        }

        void CreateDatasetIfRequired() {
            var dataSetList = connection.Service.Datasets.List(connection.ProjectId).Execute();
            if (dataSetList.Datasets == null || dataSetList.Datasets.All(d => d.DatasetReference.DatasetId != connection.DataSetId)) {
                var dataSet = new Dataset {
                    DatasetReference = new DatasetReference { DatasetId = connection.DataSetId, ProjectId = connection.ProjectId }
                };

                connection.Service.Datasets.Insert(dataSet, connection.ProjectId).Execute();
            }
        }

        [Fact(Skip = "Explicit")]
        public void CreateNatalityTable() {
            CreateDatasetIfRequired();

            var table = new Table {
                Schema = CreateNatalityTableSchema(),
                TableReference = new TableReference {
                    DatasetId = connection.DataSetId,
                    ProjectId = connection.ProjectId,
                    TableId = NatalityTableName
                }
            };

            InsertTable(table);

            UploadData(table);
        }

        static TableSchema CreateNatalityTableSchema() {
            var weight_pounds = new TableFieldSchema {
                Name = "weight_pounds",
                Type = "FLOAT",
                Mode = "NULLABLE"
            };

            var is_male = new TableFieldSchema {
                Name = "is_male",
                Type = "BOOLEAN",
                Mode = "NULLABLE"
            };

            return new TableSchema { Fields = new List<TableFieldSchema> { weight_pounds, is_male } };
        }

        void InsertTable(Table table) {
            var tableList = connection.Service.Tables.List(connection.ProjectId, connection.DataSetId).Execute();

            if (tableList.Tables != null && tableList.Tables.Any(t => t.TableReference.TableId == table.TableReference.TableId))
                connection.Service.Tables.Delete(connection.ProjectId, connection.DataSetId, table.TableReference.TableId).Execute();

            connection.Service.Tables.Insert(table, connection.ProjectId, connection.DataSetId).Execute();
        }

        void UploadData(Table table) {
            Job job = new Job();
            var config = new JobConfiguration();
            var configLoad = new JobConfigurationLoad {
                Schema = table.Schema,
                DestinationTable = table.TableReference,
                Encoding = "ISO-8859-1",
                CreateDisposition = "CREATE_IF_NEEDED",
                WriteDisposition = "",
                FieldDelimiter = ",",
                AllowJaggedRows = true,
                SourceFormat = "CSV"
            };

            config.Load = configLoad;
            job.Configuration = config;

            var jobId = "---" + Environment.TickCount;

            var jobRef = new JobReference {
                JobId = jobId,
                ProjectId = connection.ProjectId
            };
            job.JobReference = jobRef;
            using (
                Stream stream =
                    Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream(string.Format("DevExpress.DataAccess.BigQuery.Tests.{0}.csv",
                            table.TableReference.TableId))) {
                var insertMediaUpload = new JobsResource.InsertMediaUpload(connection.Service,
                    job, job.JobReference.ProjectId, stream, "application/octet-stream");
                insertMediaUpload.Upload();
            }

            while (true) {
                Job job1 = connection.Service.Jobs.Get(connection.ProjectId, jobId).Execute();

                if (job1.Status.State.Equals("DONE")) {
                    break;
                }
                Thread.Sleep(5000);
            }
        }

        [Fact(Skip = "Explicit")]
        public void CreateNatality2Table() {
            CreateDatasetIfRequired();

            var schema = CreateNatality2TableSchema();

            var table = new Table {
                Schema = schema,
                TableReference = new TableReference {
                    DatasetId = connection.DataSetId,
                    ProjectId = connection.ProjectId,
                    TableId = Natality2TableName
                }
            };


            InsertTable(table);

            UploadData(table);
        }

        static TableSchema CreateNatality2TableSchema() {
            var state = new TableFieldSchema {
                Name = "state",
                Type = "STRING",
                Mode = "NULLABLE"
            };

            var source_year = new TableFieldSchema {
                Name = "source_year",
                Type = "INTEGER",
                Mode = "NULLABLE"
            };

            var year = new TableFieldSchema {
                Name = "year",
                Type = "INTEGER",
                Mode = "NULLABLE"
            };

            var weight_pounds = new TableFieldSchema {
                Name = "weight_pounds",
                Type = "FLOAT",
                Mode = "NULLABLE"
            };

            var mother_married = new TableFieldSchema {
                Name = "mother_married",
                Type = "BOOLEAN",
                Mode = "NULLABLE"
            };

            return new TableSchema {
                Fields = new List<TableFieldSchema> { state, source_year, year, weight_pounds, mother_married }
            };
        }

        [Fact(Skip = "Explicit")]
        void CreateNatalityView() {
            CreateDatasetIfRequired();

            Table table = new Table {
                TableReference = new TableReference {
                    DatasetId = connection.DataSetId,
                    ProjectId = connection.ProjectId,
                    TableId = NatalityViewName
                },
                View = new ViewDefinition {
                    Query = string.Format(@"SELECT [{1}.year] [year], [{1}.weight_pounds] [weight], [{1}.state] [state]
                                            FROM [{0}.{1}] [{1}]", this.connection.DataSetId, Natality2TableName)
                }
            };

            InsertTable(table);
        }

        public void Dispose() {
            connection.Close();
        }
    }
}
#endif