#if DEBUGTEST
using System;
using System.Data;
using NUnit.Framework;

namespace DevExpress.DataAccess.BigQuery.Tests {
    [TestFixture]
    public class BigQueryConnectionTests {
        [Test]
        public void OpenConnectionTest() {
            using(BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString)) {
                Assert.IsNotNull(connection.ConnectionString);
                Assert.IsTrue(string.Equals(ConnectionStringHelper.OAuthConnectionString, connection.ConnectionString, StringComparison.OrdinalIgnoreCase));
                Assert.AreEqual(ConnectionState.Closed, connection.State);
                Assert.IsNull(connection.Service);
                connection.Open();
                Assert.IsNotNull(connection.ConnectionString);
                Assert.IsTrue(string.Equals(ConnectionStringHelper.OAuthConnectionString, connection.ConnectionString, StringComparison.OrdinalIgnoreCase));
                Assert.AreEqual(ConnectionState.Open, connection.State);
                Assert.IsNotNull(connection.Service);
            }
        }

        [Test]
        public void OpenCloseConnectionTest() {
            using(BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString)) {
                Assert.IsNotNull(connection.ConnectionString);
                Assert.IsTrue(string.Equals(ConnectionStringHelper.OAuthConnectionString, connection.ConnectionString, StringComparison.OrdinalIgnoreCase));
                Assert.AreEqual(ConnectionState.Closed, connection.State);
                Assert.IsNull(connection.Service);
                connection.Open();
                Assert.IsNotNull(connection.ConnectionString);
                Assert.IsTrue(string.Equals(ConnectionStringHelper.OAuthConnectionString, connection.ConnectionString, StringComparison.OrdinalIgnoreCase));
                Assert.AreEqual(ConnectionState.Open, connection.State);
                Assert.IsNotNull(connection.Service);
                connection.Close();
                Assert.AreEqual(ConnectionState.Closed, connection.State);
            }
        }

        [Test]
        public void OpenDisposeConnectionTest() {
            BigQueryConnection connection;
            using(connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString)) {
                Assert.IsNotNull(connection.ConnectionString);
                Assert.IsTrue(string.Equals(ConnectionStringHelper.OAuthConnectionString, connection.ConnectionString, StringComparison.OrdinalIgnoreCase));
                Assert.AreEqual(ConnectionState.Closed, connection.State);
                Assert.IsNull(connection.Service);
                connection.Open();
                Assert.IsNotNull(connection.ConnectionString);
                Assert.IsTrue(string.Equals(ConnectionStringHelper.OAuthConnectionString, connection.ConnectionString, StringComparison.OrdinalIgnoreCase));
                Assert.AreEqual(ConnectionState.Open, connection.State);
                Assert.IsNotNull(connection.Service);
                connection.Dispose();
            }
            Assert.AreEqual(ConnectionState.Closed, connection.State);
        }

        [Test]
        public void GetTableNamesTest() {
            using(BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString)) {
                connection.Open();
                var tableNames = connection.GetTableNames();
                Assert.AreEqual(3, tableNames.Length);
                Assert.AreEqual("natality", tableNames[0]);
                Assert.AreEqual("natality2", tableNames[1]);
                Assert.AreEqual("test1", tableNames[2]);
            }
        }

        [Test]
        public void CreateDbCommandTest() {
            using(BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString)) {
                connection.Open();
                var dbCommand = connection.CreateCommand();
                Assert.IsNotNull(dbCommand);
                Assert.IsNotNull(dbCommand.Connection);
                Assert.AreSame(connection, dbCommand.Connection);
                Assert.AreEqual("", dbCommand.CommandText);
                Assert.IsNull(dbCommand.Transaction);
                Assert.IsNotNull(dbCommand.Parameters);
                Assert.AreEqual(0, dbCommand.Parameters.Count);
                Assert.AreEqual((CommandType)0, dbCommand.CommandType);
            }
        }

        [Test]
        [Ignore("We haven't yet something other database for tests --Repushko Anton")]
        public void ChangeDatabaseOpenTest() {
            using(BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString)) {
                connection.Open();
                string newDatabase = "";
                connection.ChangeDatabase(newDatabase);
                Assert.IsNotNull(connection.Service);
                Assert.AreEqual(connection.DataSetId, newDatabase);
                Assert.AreEqual(connection.State, ConnectionState.Open);
            }
        }

        [Test]
        [Ignore("We haven't yet something other database for tests --Repushko Anton")]
        public void ChangeDatabaseCloseTest() {
            using(BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString)) {
                string newDatabase = "";
                connection.ChangeDatabase(newDatabase);
                Assert.IsNotNull(connection.Service);
                Assert.AreEqual(connection.DataSetId, newDatabase);
                Assert.AreEqual(connection.State, ConnectionState.Open);
            }
        }

        [Test, ExpectedException(typeof(ObjectDisposedException))]
        public void DisposeChangeDatabaseTest() {
            using(BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString)) {
                connection.Open();
                connection.Dispose();
                connection.ChangeDatabase("");
            }
        }

        [Test, ExpectedException(typeof(ObjectDisposedException))]
        public void DisposeGetTableNamesTest() {
            using(BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString)) {
                connection.Dispose();
                connection.GetTableNames();
            }

        }

        [Test, ExpectedException(typeof(ObjectDisposedException))]
        public void DisposeCreateDbCommandTest() {
            using(BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString)) {
                connection.Dispose();
                connection.CreateCommand();
            }
        }

        [Test, ExpectedException(typeof(ObjectDisposedException))]
        public void DisposeOpenConnectionTest() {
            using(BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString)) {
                connection.Dispose();
                connection.Open();
            }
        }

        [Test, ExpectedException(typeof(ObjectDisposedException))]
        public void DisposeCloseConnectionTest() {
            using(BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString)) {
                connection.Dispose();
                connection.Close();
            }
        }

        [Test]
        public void DisposeDisposeConnectionTest() {
            using(BigQueryConnection connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString)) {
                connection.Dispose();
                connection.Dispose();
            }
        }
    }
}
#endif