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
        const string commandText = "SELECT * FROM [testdata." + TestingInfrastructureHelper.NatalityTableName + "] LIMIT 10";

        protected abstract string GetConnectionString();

        [TestFixtureSetUp]
        public void Initialize() {
            natalitySchemaTable = new DataTable();
            natalitySchemaTable.Columns.Add("ColumnName", typeof(string));
            natalitySchemaTable.Columns.Add("DataType", typeof(Type));
            natalitySchemaTable.Rows.Add("weight_pounds", typeof(float));
            natalitySchemaTable.Rows.Add("is_male", typeof(bool));
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
                dbCommand.CommandText = string.Format("select * from [testdata.{0}] where state=@state", TestingInfrastructureHelper.Natality2TableName);
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
                dbCommand.CommandText = string.Format("select * from [testdata.{0}] where state=@state", TestingInfrastructureHelper.Natality2TableName);
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
                dbCommand.CommandText = string.Format("select * from [testdata.{0}] where state=@state", TestingInfrastructureHelper.Natality2TableName);
                param.Value = @"CA\' or 1=1--";
                param.ParameterName = "state";
                dbCommand.Parameters.Add(param);
                var result = (BigQueryDataReader)dbCommand.ExecuteReader(CommandBehavior.Default);
                Assert.IsFalse(result.Read());
            }
        }

        [Test, ExpectedException(typeof(BigQueryException))]
        public void MismatchingNumericParameterTypeTest() {
            using(var dbCommand = connection.CreateCommand()) {
                var param = dbCommand.CreateParameter();
                dbCommand.CommandText = "select * from [testdata.natality2] where year=@year";
                param.Value = "string, but expected number";
                param.ParameterName = "year";
                dbCommand.Parameters.Add(param);
                dbCommand.ExecuteReader(CommandBehavior.Default);
            }
        }

        [Test, ExpectedException(typeof(BigQueryException))]
        public void MismatchingBooleanParameterTypeTest() {
            using(var dbCommand = connection.CreateCommand()) {
                var param = dbCommand.CreateParameter();
                dbCommand.CommandText = "select * from [testdata.natality2] where mother_married=@mother_married";
                param.Value = "string, but excepted bool";
                param.ParameterName = "mother_married";
                dbCommand.Parameters.Add(param);
                dbCommand.ExecuteReader(CommandBehavior.Default);
            }
        }

    }

}
#endif