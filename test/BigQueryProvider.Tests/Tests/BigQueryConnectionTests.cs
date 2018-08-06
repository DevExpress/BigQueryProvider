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

using System;
using System.Data;
using Xunit;

namespace DevExpress.DataAccess.BigQuery.Tests {
    public class BigQueryConnectionTests {
        [Fact]
        public void OpenConnectionTest() {
            using(BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString)) {
                Assert.NotNull(connection.ConnectionString);
                Assert.Equal(ConnectionStringHelper.OAuthConnectionString, connection.ConnectionString, ignoreCase: true);
                Assert.Equal(ConnectionState.Closed, connection.State);
                Assert.Null(connection.Service);
                connection.Open();
                Assert.NotNull(connection.ConnectionString);
                Assert.Equal(ConnectionStringHelper.OAuthConnectionString, connection.ConnectionString, ignoreCase: true);
                Assert.Equal(ConnectionState.Open, connection.State);
                Assert.NotNull(connection.Service);
            }
        }

        [Fact]
        public async void OpenConnectionTest_Async() {
            using (BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString)) {
                Assert.NotNull(connection.ConnectionString);
                Assert.Equal(ConnectionStringHelper.OAuthConnectionString, connection.ConnectionString, ignoreCase: true);
                Assert.Equal(ConnectionState.Closed, connection.State);
                Assert.Null(connection.Service);
                await connection.OpenAsync();
                Assert.NotNull(connection.ConnectionString);
                Assert.Equal(ConnectionStringHelper.OAuthConnectionString, connection.ConnectionString, ignoreCase: true);
                Assert.Equal(ConnectionState.Open, connection.State);
                Assert.NotNull(connection.Service);
            }
        }

        [Fact]
        public void OpenCloseConnectionTest() {
            using(BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString)) {
                Assert.NotNull(connection.ConnectionString);
                Assert.Equal(ConnectionStringHelper.OAuthConnectionString, connection.ConnectionString, ignoreCase: true);
                Assert.Equal(ConnectionState.Closed, connection.State);
                Assert.Null(connection.Service);
                connection.Open();
                Assert.NotNull(connection.ConnectionString);
                Assert.Equal(ConnectionStringHelper.OAuthConnectionString, connection.ConnectionString, ignoreCase: true);
                Assert.Equal(ConnectionState.Open, connection.State);
                Assert.NotNull(connection.Service);
                connection.Close();
                Assert.Equal(ConnectionState.Closed, connection.State);
            }
        }

        [Fact]
        public void OpenDisposeConnectionTest() {
            BigQueryConnection connection;
            using(connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString)) {
                Assert.NotNull(connection.ConnectionString);
                Assert.Equal(ConnectionStringHelper.OAuthConnectionString, connection.ConnectionString, ignoreCase: true);
                Assert.Equal(ConnectionState.Closed, connection.State);
                Assert.Null(connection.Service);
                connection.Open();
                Assert.NotNull(connection.ConnectionString);
                Assert.Equal(ConnectionStringHelper.OAuthConnectionString, connection.ConnectionString, ignoreCase: true);
                Assert.Equal(ConnectionState.Open, connection.State);
                Assert.NotNull(connection.Service);
                connection.Dispose();
            }
            Assert.Equal(ConnectionState.Closed, connection.State);
        }

        [Fact]
        public void GetTableNamesTest() {
            using(BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString)) {
                connection.Open();
                string[] tableNames = connection.GetTableNames();
                Assert.Equal(3, tableNames.Length);
                Assert.Equal(TestingInfrastructureHelper.NatalityTableName, tableNames[0]);
                Assert.Equal(TestingInfrastructureHelper.Natality2TableName, tableNames[1]);
                Assert.Equal(TestingInfrastructureHelper.TimesTableName, tableNames[2]);
            }
        }

        [Fact]
        public void GetViewNamesTest() {
            using(BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString)) {
                connection.Open();
                string[] tableNames = connection.GetViewNames();
                Assert.Single(tableNames);
                Assert.Equal(TestingInfrastructureHelper.NatalityViewName, tableNames[0]);
            }
        }

        [Fact]
        public void GetDataSetNamesTest() {
            using(BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString)) {
                connection.Open();
                string[] dataSetNames = connection.GetDataSetNames();
                Assert.Single(dataSetNames);
                Assert.Equal("testdata", dataSetNames[0]);
            }
        }

        [Fact]
        public void CreateDbCommandTest() {
            using(BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString)) {
                connection.Open();
                var dbCommand = connection.CreateCommand();
                Assert.NotNull(dbCommand);
                Assert.NotNull(dbCommand.Connection);
                Assert.Same(connection, dbCommand.Connection);
                Assert.Equal("", dbCommand.CommandText);
                Assert.Null(dbCommand.Transaction);
                Assert.NotNull(dbCommand.Parameters);
                Assert.Empty(dbCommand.Parameters);
                Assert.Equal((CommandType)0, dbCommand.CommandType);
            }
        }

        [Fact(Skip = "No database for it")]
        public void ChangeDatabaseOpenTest() {
            using(BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString)) {
                connection.Open();
                string newDatabase = "";
                connection.ChangeDatabase(newDatabase);
                Assert.NotNull(connection.Service);
                Assert.Equal(connection.DataSetId, newDatabase);
                Assert.Equal(ConnectionState.Open, connection.State);
            }
        }

        [Fact(Skip = "No database for it")]
        public void ChangeDatabaseCloseTest() {
            using(BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString)) {
                string newDatabase = "";
                connection.ChangeDatabase(newDatabase);
                Assert.NotNull(connection.Service);
                Assert.Equal(connection.DataSetId, newDatabase);
                Assert.Equal(ConnectionState.Open, connection.State);
            }
        }

        [Fact]
        public void DisposeChangeDatabaseTest() {
            BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString);
            connection.Open();
            connection.Dispose();
            Assert.Throws<ObjectDisposedException>(() => {
                connection.ChangeDatabase("");
            });
        }

        [Fact]
        public void DisposeGetTableNamesTest() {
            BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString);
            connection.Dispose();

            Assert.Throws<ObjectDisposedException>(() => { connection.GetTableNames(); });
        }

        [Fact]
        public void DisposeCreateDbCommandTest() {
            BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString);
            connection.Dispose();
            Assert.Throws<ObjectDisposedException>(() => { connection.CreateCommand(); });
        }

        [Fact]
        public void DisposeOpenConnectionTest() {
            BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString);
            connection.Dispose();
            Assert.Throws<ObjectDisposedException>(() => { connection.Open(); });

        }

        [Fact]
        public void DisposeCloseConnectionTest() {
            BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString);
            connection.Dispose();
            Assert.Throws<ObjectDisposedException>(() => {
                connection.Close();
            });

        }

        [Fact]
        public void DisposeDisposeConnectionTest() {
            BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString);
            connection.Dispose();
            connection.Dispose();
        }
    }
}