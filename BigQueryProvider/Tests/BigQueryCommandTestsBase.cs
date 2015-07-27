#if DEBUGTEST
using System;
using System.Collections.Generic;
using System.Data;
using Google.Apis.Bigquery.v2.Data;
using NUnit.Framework;

namespace DevExpress.DataAccess.BigQuery.Tests {
    [TestFixture]
    public abstract class BigQueryCommandTestsBase {
        BigQueryConnection connection;
        DataTable natalitySchemaTable;
        const string commandText = "SELECT * FROM [testdata.natality3] LIMIT 10";

        protected abstract string GetConnectionString();

        [TestFixtureSetUp]
        public void Initialize() {
            natalitySchemaTable = new DataTable();
            natalitySchemaTable.Columns.Add("ColumnName", typeof(string));
            natalitySchemaTable.Columns.Add("DataType", typeof(Type));
            natalitySchemaTable.Rows.Add("weight_pounds", typeof(float));
            natalitySchemaTable.Rows.Add("is_male", typeof(bool));

            var schema = new TableSchema();

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

            schema.Fields = new List<TableFieldSchema> { weight_pounds, is_male };

            var table = new Table { Schema = schema };

            connection = new BigQueryConnection(GetConnectionString());

            table.TableReference = new TableReference {
                DatasetId = connection.DataSetId,
                ProjectId = connection.ProjectId,
                TableId = "natality3"
            };
            
            connection.Open();
            try {
                connection.Service.Tables.Insert(table, connection.ProjectId, connection.DataSetId).Execute();
            }
            finally {
                connection.Close();
            }
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            connection = new BigQueryConnection(GetConnectionString());
            connection.Open();
            try
            {
                connection.Service.Tables.Delete(connection.ProjectId, connection.DataSetId, "natality3").Execute();
            }
            finally
            {
                connection.Close();
            }
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
                dbCommand.CommandText = "select * from [testdata.natality2] where state=@state";
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
                dbCommand.CommandText = "select * from [testdata.natality2] where state=@state";
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
                dbCommand.CommandText = "select * from [testdata.natality2] where state=@state";
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