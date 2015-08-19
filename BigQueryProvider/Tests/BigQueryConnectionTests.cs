#if DEBUGTEST
using System;
using System.Data;
using Xunit;

namespace DevExpress.DataAccess.BigQuery.Tests {
    public class BigQueryConnectionTests {
        [Fact]
        public void OpenConnectionTest() {
            using(BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString)) {
                Assert.NotNull(connection.ConnectionString);
                Assert.True(string.Equals(ConnectionStringHelper.OAuthConnectionString, connection.ConnectionString, StringComparison.OrdinalIgnoreCase));
                Assert.Equal(ConnectionState.Closed, connection.State);
                Assert.Null(connection.Service);
                connection.Open();
                Assert.NotNull(connection.ConnectionString);
                Assert.True(string.Equals(ConnectionStringHelper.OAuthConnectionString, connection.ConnectionString, StringComparison.OrdinalIgnoreCase));
                Assert.Equal(ConnectionState.Open, connection.State);
                Assert.NotNull(connection.Service);
            }
        }

        [Fact]
        public void OpenCloseConnectionTest() {
            using(BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString)) {
                Assert.NotNull(connection.ConnectionString);
                Assert.True(string.Equals(ConnectionStringHelper.OAuthConnectionString, connection.ConnectionString, StringComparison.OrdinalIgnoreCase));
                Assert.Equal(ConnectionState.Closed, connection.State);
                Assert.Null(connection.Service);
                connection.Open();
                Assert.NotNull(connection.ConnectionString);
                Assert.True(string.Equals(ConnectionStringHelper.OAuthConnectionString, connection.ConnectionString, StringComparison.OrdinalIgnoreCase));
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
                Assert.True(string.Equals(ConnectionStringHelper.OAuthConnectionString, connection.ConnectionString, StringComparison.OrdinalIgnoreCase));
                Assert.Equal(ConnectionState.Closed, connection.State);
                Assert.Null(connection.Service);
                connection.Open();
                Assert.NotNull(connection.ConnectionString);
                Assert.True(string.Equals(ConnectionStringHelper.OAuthConnectionString, connection.ConnectionString, StringComparison.OrdinalIgnoreCase));
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
                var tableNames = connection.GetTableNames();
                Assert.Equal(2, tableNames.Length);
                Assert.Equal("natality", tableNames[0]);
                Assert.Equal("natality2", tableNames[1]);
            }
        }

        [Fact]
        public void GetDataSetNamesTest() {
            using(BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString)) {
                connection.Open();
                string[] dataSetNames = connection.GetDataSetNames();
                Assert.Equal(1, dataSetNames.Length);
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
                Assert.Equal(0, dbCommand.Parameters.Count);
                Assert.Equal((CommandType)0, dbCommand.CommandType);
            }
        }

        [Fact(Skip = "We haven't yet something other database for tests --Repushko Anton")]
        public void ChangeDatabaseOpenTest() {
            using(BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString)) {
                connection.Open();
                string newDatabase = "";
                connection.ChangeDatabase(newDatabase);
                Assert.NotNull(connection.Service);
                Assert.Equal(connection.DataSetId, newDatabase);
                Assert.Equal(connection.State, ConnectionState.Open);
            }
        }

        [Fact(Skip = "We haven't yet something other database for tests --Repushko Anton")]
        public void ChangeDatabaseCloseTest() {
            using(BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString)) {
                string newDatabase = "";
                connection.ChangeDatabase(newDatabase);
                Assert.NotNull(connection.Service);
                Assert.Equal(connection.DataSetId, newDatabase);
                Assert.Equal(connection.State, ConnectionState.Open);
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
#endif