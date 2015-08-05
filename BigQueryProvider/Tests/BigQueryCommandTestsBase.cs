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
using Xunit;

namespace DevExpress.DataAccess.BigQuery.Tests {
    public abstract class BigQueryCommandTestsBase : IDisposable {
        BigQueryConnection connection;
        DataTable natalitySchemaTable;
        const string commandText = "SELECT * FROM [testdata." + TestingInfrastructureHelper.NatalityTableName + "] LIMIT 10";

        protected abstract string GetConnectionString();

        protected BigQueryCommandTestsBase() {
            natalitySchemaTable = new DataTable();
            natalitySchemaTable.Columns.Add("ColumnName", typeof (string));
            natalitySchemaTable.Columns.Add("DataType", typeof (Type));
            natalitySchemaTable.Rows.Add("weight_pounds", typeof (float));
            natalitySchemaTable.Rows.Add("is_male", typeof (bool));

            connection = new BigQueryConnection(GetConnectionString());
            connection.Open();
        }

        [Fact]
        public void ExecuteReaderTest_TypeText() {
            using(var dbCommand = connection.CreateCommand()) {
                dbCommand.CommandText = commandText;
                dbCommand.CommandType = CommandType.Text;
                var dbDataReader = dbCommand.ExecuteReader();
                Assert.NotNull(dbDataReader);
                Assert.Equal(2, dbDataReader.FieldCount);
            }
        }

        [Fact]
        public void ExecuteReaderTest_TypeTableDirect() {
            using(var dbCommand = connection.CreateCommand()) {
                dbCommand.CommandText = "natality";
                dbCommand.CommandType = CommandType.TableDirect;
                var dbDataReader = dbCommand.ExecuteReader();
                Assert.NotNull(dbDataReader);
                Assert.Equal(2, dbDataReader.FieldCount);
            }
        }

        [Fact]
        public void ExecuteReader_TypeStoredProcedure() {
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                using (var dbCommand = connection.CreateCommand()) {
                    dbCommand.CommandType = CommandType.StoredProcedure;
                }
            }
                );
        }

        [Fact]
        public void ExecuteScalarReaderTest() {
            using(var dbCommand = connection.CreateCommand()) {
                dbCommand.CommandText = "select 1 from [testdata.natality]";
                var executeScalarResult = dbCommand.ExecuteScalar();
                Assert.NotNull(executeScalarResult);
                Assert.Equal(1, int.Parse(executeScalarResult.ToString()));
            }
        }

        [Fact]
        public void CommandSchemaBehaviorTest() {
            using(var dbCommand = connection.CreateCommand()) {
                var dbDataReader = dbCommand.ExecuteReader(CommandBehavior.SchemaOnly);
                dbDataReader.NextResult();
                DataTable schemaTable = dbDataReader.GetSchemaTable();
                Assert.True(DataTableComparer.Equals(natalitySchemaTable, schemaTable));
            }
        }

        [Fact]
        public void CommandCloseConnectionTest() {
            connection.Close();

            Assert.Throws<InvalidOperationException>(() => {
                using (var dbCommand = connection.CreateCommand()) {
                }
            });
        }

        [Fact]
        public void EscapingSingleQuotesTest() {
            using(var dbCommand = connection.CreateCommand()) {
                var param = dbCommand.CreateParameter();
                dbCommand.CommandText = string.Format("select * from [testdata.{0}] where state=@state", TestingInfrastructureHelper.Natality2TableName);
                param.Value = "CA' or 1=1--";
                param.ParameterName = "state";
                dbCommand.Parameters.Add(param);
                var result = (BigQueryDataReader)dbCommand.ExecuteReader(CommandBehavior.Default);
                Assert.False(result.Read());
            }
        }

        [Fact]
        public void EscapingDoubleQuotesTest() {
            using(var dbCommand = connection.CreateCommand()) {
                var param = dbCommand.CreateParameter();
                dbCommand.CommandText = string.Format("select * from [testdata.{0}] where state=@state", TestingInfrastructureHelper.Natality2TableName);
                param.Value = @"CA"" or 1=1--";
                param.ParameterName = "state";
                dbCommand.Parameters.Add(param);
                var result = (BigQueryDataReader)dbCommand.ExecuteReader(CommandBehavior.Default);
                Assert.False(result.Read());
            }
        }

        [Fact]
        public void EscapingBackSlashesTest() {
            using(var dbCommand = connection.CreateCommand()) {
                var param = dbCommand.CreateParameter();
                dbCommand.CommandText = string.Format("select * from [testdata.{0}] where state=@state", TestingInfrastructureHelper.Natality2TableName);
                param.Value = @"CA\' or 1=1--";
                param.ParameterName = "state";
                dbCommand.Parameters.Add(param);
                var result = (BigQueryDataReader)dbCommand.ExecuteReader(CommandBehavior.Default);
                Assert.False(result.Read());
            }
        }

        public void Dispose() {
            connection.Close();
        }
    }

}
#endif