/*
   Copyright 2015-2018 Developer Express Inc.

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
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Xunit;

namespace DevExpress.DataAccess.BigQuery.Tests {
    public abstract class BigQueryCommandTestsBase : IDisposable {
        readonly BigQueryConnection connection;
        readonly DataTable natalitySchemaTable;
        
        readonly string commandText;
        readonly string commandTextWithFilter;

        const string dataSetName = "testdata";
        const string filterByString = "state = @state";
        const string filterByBool = "mother_married = @mothermarried";
        const string filterByNull = "mother_married = null";
        const string injectedViaSingleQuotesValue = "CA' or 1=1--";
        const string injectedViaDoubleQuotesValue = @"CA"" or 1=1--";
        const string injectedViaBackSlashesValue = @"CA\' or 1=1--";
        const string normalValue = "CA";
        const bool trueValue = true;

        protected abstract string GetConnectionString();
        protected abstract string PatchConnectionString(string connectionString);

        protected abstract string Escape(string identifier);
        protected abstract string FormatTable(params string[] parts);
        protected abstract string FormatColumn(params string[] parts);

        protected BigQueryCommandTestsBase() {
            natalitySchemaTable = new DataTable();
            natalitySchemaTable.Columns.Add("ColumnName", typeof (string));
            natalitySchemaTable.Columns.Add("DataType", typeof (Type));
            natalitySchemaTable.Rows.Add("weight_pounds", typeof (float));
            natalitySchemaTable.Rows.Add("is_male", typeof (bool));

            #region consts

            commandText = $"select * from {FormatTable(dataSetName, TestingInfrastructureHelper.NatalityTableName)} limit 10";
            commandTextWithFilter = $"select * from {FormatTable(dataSetName, TestingInfrastructureHelper.Natality2TableName)} where {{0}} limit 10";
            #endregion
            
            connection = new BigQueryConnection(PatchConnectionString(GetConnectionString()));
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
        public async void ExecuteReaderTest_TypeText_Async() {
            using (var dbCommand = connection.CreateCommand()) {
                dbCommand.CommandText = commandText;
                dbCommand.CommandType = CommandType.Text;
                var dbDataReader = await dbCommand.ExecuteReaderAsync();
                Assert.NotNull(dbDataReader);
                Assert.Equal(2, dbDataReader.FieldCount);
            }
        }

        [Fact]
        public void ExecuteReaderTest_TypeTableDirect() {
            using(var dbCommand = connection.CreateCommand()) {
                dbCommand.CommandText = TestingInfrastructureHelper.NatalityTableName;
                dbCommand.CommandType = CommandType.TableDirect;
                var dbDataReader = dbCommand.ExecuteReader();
                Assert.NotNull(dbDataReader);
                Assert.Equal(2, dbDataReader.FieldCount);
            }
        }

        [Fact]
        public async void ExecuteReaderTest_TypeTableDirect_Async() {
            using (var dbCommand = connection.CreateCommand()) {
                dbCommand.CommandText = "natality";
                dbCommand.CommandType = CommandType.TableDirect;
                var dbDataReader = await dbCommand.ExecuteReaderAsync();
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
            });
        }

        [Fact]
        public void ExecuteScalarReaderTest() {
            using(var dbCommand = connection.CreateCommand()) {
                dbCommand.CommandText = $"select 1 from {FormatTable(dataSetName, TestingInfrastructureHelper.NatalityTableName)}";
                var executeScalarResult = dbCommand.ExecuteScalar();
                Assert.NotNull(executeScalarResult);
                Assert.Equal(1, int.Parse(executeScalarResult.ToString()));
            }
        }

        [Fact]
        public async void ExecuteScalarReaderTest_Async() {
            using (var dbCommand = connection.CreateCommand()) {
                dbCommand.CommandText = $"select 1 from {FormatTable(dataSetName, TestingInfrastructureHelper.NatalityTableName)}";
                var executeScalarResult = await dbCommand.ExecuteScalarAsync();
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

        [Theory]
        [InlineData(filterByString, "state", normalValue, true)]
        [InlineData(filterByString, "state", injectedViaSingleQuotesValue, false)]
        [InlineData(filterByString, "state", injectedViaDoubleQuotesValue, false)]
        [InlineData(filterByString, "state", injectedViaBackSlashesValue, false)]
        [InlineData(filterByBool, "mothermarried", trueValue, true)]
        public void RunCommandWithParameterTest(string filterString, string parameterName, object parameterValue, bool exceptedReadResult) {
            using(var dbCommand = connection.CreateCommand()) {
                var param = dbCommand.CreateParameter();
                dbCommand.CommandText = string.Format(commandTextWithFilter, filterString);
                param.ParameterName = parameterName;
                param.Value = parameterValue;
                dbCommand.Parameters.Add(param);
                var reader = dbCommand.ExecuteReader(CommandBehavior.Default);
                Assert.Equal(exceptedReadResult, reader.Read());
            }
        }

        [Fact]
        public void ValidateIncorrectParamInCollectionTest() {
            var dbCommand = connection.CreateCommand();
            var param = dbCommand.CreateParameter();
            param.Value = "testValue";
            dbCommand.Parameters.Add(param);
            Assert.Throws<ArgumentException>(() => dbCommand.ExecuteReader(CommandBehavior.Default));
        }

        [Fact]
        public void TimesTest() {
            using(var dbCommand = connection.CreateCommand()) {
                dbCommand.CommandText = TestingInfrastructureHelper.TimesTableName;
                dbCommand.CommandType = CommandType.TableDirect;

                using(var dbDataReader = dbCommand.ExecuteReader()) {
                    object[] values = new object[dbDataReader.FieldCount];

                    dbDataReader.Read();
                    dbDataReader.GetValues(values);
                    Assert.Equal(DateTime.Parse("23:59:59"), values[0]);
                    Assert.Equal(DateTime.Parse("1970-01-24"), values[1]);
                    Assert.Equal(DateTime.Parse("2016-10-19 00:08:00"), values[2]);
                    Assert.Equal(DateTime.Parse("9999-12-31T23:59:59"), values[3]);

                    dbDataReader.Read();
                    dbDataReader.GetValues(values);
                    Assert.Equal(DateTime.Parse("23:59:59.999999"), values[0]);
                    Assert.Equal(DateTime.Parse("1980-01-01"), values[1]);
                    Assert.Equal(DateTime.Parse("2016-10-19 00:08:00"), values[2]);
                    Assert.Equal(DateTime.Parse("9999-12-31T23:59:59.999999"), values[3]);
                }
            }
        }

        public void Dispose() {
            connection.Close();
        }
    }

    public abstract class LegacySqlBigQueryCommandTests : BigQueryCommandTestsBase {
        protected override string Escape(string identifier) {
            return $"[{identifier}]";
        }

        protected override string FormatTable(params string[] parts) {
            return Escape(string.Join(".", parts));
        }

        protected override string FormatColumn(params string[] parts) {
            throw new NotImplementedException();
        }

        protected override string PatchConnectionString(string connectionString) {
            var dbConnectionStringBuilder = new DbConnectionStringBuilder {
                ConnectionString = connectionString,
                ["LegacySql"] = "true"
            };
            return dbConnectionStringBuilder.ConnectionString;
        }
    }
    
    public abstract class StandardSqlBigQueryCommandTests : BigQueryCommandTestsBase {
        protected override string Escape(string identifier) {
            return $"`{identifier}`";
        }

        protected override string FormatTable(params string[] parts) {
            return Escape(string.Join(".", parts));
        }

        protected override string FormatColumn(params string[] parts) {
            throw new NotImplementedException();
        }

        protected override string PatchConnectionString(string connectionString) {
            return connectionString;
        }
    }
}
#endif
