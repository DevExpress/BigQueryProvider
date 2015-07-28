#if DEBUGTEST
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Google.Apis.Bigquery.v2;
using Google.Apis.Bigquery.v2.Data;
using NUnit.Framework;

namespace DevExpress.DataAccess.BigQuery.Tests {
    [TestFixture]
    public abstract class BigQueryCommandTestsBase {
        BigQueryConnection connection;
        DataTable natalitySchemaTable;
        const string natalityTableName = "natality";
        const string natality2TableName = "natality2";
        const string commandText = "SELECT * FROM [testdata." + natalityTableName + "] LIMIT 10";

        protected abstract string GetConnectionString();

        [TestFixtureSetUp]
        public void Initialize() {
            natalitySchemaTable = new DataTable();
            natalitySchemaTable.Columns.Add("ColumnName", typeof(string));
            natalitySchemaTable.Columns.Add("DataType", typeof(Type));
            natalitySchemaTable.Rows.Add("weight_pounds", typeof(float));
            natalitySchemaTable.Rows.Add("is_male", typeof(bool));
        }

        [Test, Explicit]
        public void CreateTestingInfrastructure() {
            CreateNatalityTable();
            CreateNatality2Table();
        }

        [Test, Explicit]
        public void CreateNatalityTable() {
            var table = new Table {
                Schema = CreateNatalityTableSchema(),
                TableReference = new TableReference {
                    DatasetId = connection.DataSetId,
                    ProjectId = connection.ProjectId,
                    TableId = natalityTableName
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

            return new TableSchema {Fields = new List<TableFieldSchema> {weight_pounds, is_male}};
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

        [Test, Explicit]
        public void CreateNatality2Table() {
            var schema = CreateNatality2TableSchema();

            var table = new Table {
                Schema = schema,
                TableReference = new TableReference {
                    DatasetId = connection.DataSetId,
                    ProjectId = connection.ProjectId,
                    TableId = natality2TableName
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
                Fields = new List<TableFieldSchema> {state, source_year, year, weight_pounds, mother_married}
            };
        }

        [SetUp]
        public void OpenConnection() {
            connection = new BigQueryConnection(GetConnectionString());
            connection.Open();
        }

        [TearDown]
        public void CloseConnection() {
            connection.Close();
        }

        [Test]
        public void ExecuteReaderTest_TypeText() {
            using(var dbCommand = connection.CreateCommand()) {
                dbCommand.CommandText = commandText;
                dbCommand.CommandType = CommandType.Text;
                var dbDataReader = dbCommand.ExecuteReader();
                Assert.IsNotNull(dbDataReader);
                Assert.AreEqual(2, dbDataReader.FieldCount);
            }
        }

        [Test]
        public void ExecuteReaderTest_TypeTableDirect() {
            using(var dbCommand = connection.CreateCommand()) {
                dbCommand.CommandText = "natality";
                dbCommand.CommandType = CommandType.TableDirect;
                var dbDataReader = dbCommand.ExecuteReader();
                Assert.IsNotNull(dbDataReader);
                Assert.AreEqual(2, dbDataReader.FieldCount);
            }
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ExecuteReader_TypeStoredProcedure() {
            using(var dbCommand = connection.CreateCommand()) {
                dbCommand.CommandType = CommandType.StoredProcedure;
            }
        }

        [Test]
        public void ExecuteScalarReaderTest() {
            using(var dbCommand = connection.CreateCommand()) {
                dbCommand.CommandText = "select 1 from [testdata.natality]";
                var executeScalarResult = dbCommand.ExecuteScalar();
                Assert.IsNotNull(executeScalarResult);
                Assert.AreEqual(1, int.Parse(executeScalarResult.ToString()));
            }
        }

        [Test]
        public void CommandSchemaBehaviorTest() {
            using(var dbCommand = connection.CreateCommand()) {
                var dbDataReader = dbCommand.ExecuteReader(CommandBehavior.SchemaOnly);
                dbDataReader.NextResult();
                DataTable schemaTable = dbDataReader.GetSchemaTable();
                Assert.IsTrue(DataTableComparer.Equals(natalitySchemaTable, schemaTable));
            }
        }

        [Test, ExpectedException(typeof(System.InvalidOperationException))]
        public void CommandCloseConnectionTest() {
            connection.Close();
            using(var dbCommand = connection.CreateCommand()) {
            }
        }

        [Test]
        public void EscapingSingleQoutesTest() {
            using(var dbCommand = connection.CreateCommand()) {
                var param = dbCommand.CreateParameter();
                dbCommand.CommandText = string.Format("select * from [testdata.{0}] where state=@state", natality2TableName);
                param.Value = "CA' or 1=1--";
                param.ParameterName = "state";
                dbCommand.Parameters.Add(param);
                var result = (BigQueryDataReader)dbCommand.ExecuteReader(CommandBehavior.Default);
                Assert.IsFalse(result.Read());
            }
        }

        [Test]
        public void EscapingDoubleQoutesTest() {
            using(var dbCommand = connection.CreateCommand()) {
                var param = dbCommand.CreateParameter();
                dbCommand.CommandText = string.Format("select * from [testdata.{0}] where state=@state", natality2TableName);
                param.Value = @"CA"" or 1=1--";
                param.ParameterName = "state";
                dbCommand.Parameters.Add(param);
                var result = (BigQueryDataReader)dbCommand.ExecuteReader(CommandBehavior.Default);
                Assert.IsFalse(result.Read());
            }
        }

        [Test]
        public void EscapingBackSlashesTest() {
            using(var dbCommand = connection.CreateCommand()) {
                var param = dbCommand.CreateParameter();
                dbCommand.CommandText = string.Format("select * from [testdata.{0}] where state=@state", natality2TableName);
                param.Value = @"CA\' or 1=1--";
                param.ParameterName = "state";
                dbCommand.Parameters.Add(param);
                var result = (BigQueryDataReader)dbCommand.ExecuteReader(CommandBehavior.Default);
                Assert.IsFalse(result.Read());
            }
        }
    }

}
#endif