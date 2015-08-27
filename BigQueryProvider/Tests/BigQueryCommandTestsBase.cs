/*
   Copyright 2015 Developer Express Inc.

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
        const string commandText = "SELECT * FROM [testdata." + TestingInfrastructureHelper.NatalityTableName + "] LIMIT 10";
        const string commandTextWithFilter = "SELECT * FROM [testdata." + TestingInfrastructureHelper.Natality2TableName + "] WHERE {0} LIMIT 10";
        const string filterByString = "state = @state";
        const string filterByBool = "mother_married = @mother_married";
        const string filterByNull = "mother_married = null";
        const string injectedViaSingleQuotesValue = "CA' or 1=1--";
        const string injectedViaDoubleQuotesValue = @"CA"" or 1=1--";
        const string injectedViaBackSlashesValue = @"CA\' or 1=1--";
        const string normalValue = "CA";
        const bool trueValue = true;

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
                dbCommand.CommandText = "natality";
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
                dbCommand.CommandText = "select 1 from [testdata.natality]";
                var executeScalarResult = dbCommand.ExecuteScalar();
                Assert.NotNull(executeScalarResult);
                Assert.Equal(1, int.Parse(executeScalarResult.ToString()));
            }
        }

        [Fact]
        public async void ExecuteScalarReaderTest_Async() {
            using (var dbCommand = connection.CreateCommand()) {
                dbCommand.CommandText = "select 1 from [testdata.natality]";
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
        [InlineData(filterByString, "@state", normalValue, true)]
        [InlineData(filterByString, "state", injectedViaSingleQuotesValue, false)]
        [InlineData(filterByString, "state", injectedViaDoubleQuotesValue, false)]
        [InlineData(filterByString, "state", injectedViaBackSlashesValue, false)]
        [InlineData(filterByBool, "mother_married", trueValue, true)]
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
        public void FilterByNullTest() {
            using(var dbCommand = connection.CreateCommand()) {
                dbCommand.CommandText = string.Format(commandTextWithFilter, filterByNull);
                var reader = dbCommand.ExecuteReader(CommandBehavior.Default);
                Assert.False(reader.HasRows, "Fix issue #119");
            }
        }

        public void Dispose() {
            connection.Close();
        }
    }
}
#endif
