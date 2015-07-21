#if DEBUGTEST
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using NUnit.Framework;

namespace DevExpress.DataAccess.BigQuery.Tests {
    [TestFixture]
    public class BigQueryTests {
        [Test]
        public void OpenConnectionTest() {
            var connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString);
            connection.Open();
            Assert.AreEqual(ConnectionState.Open, connection.State);

            connection.Close();
            Assert.AreEqual(ConnectionState.Closed, connection.State);
        }
        
        [Test]
        public void DataReaderTest() {
            int rowsCount = 10;

            var connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = string.Format("SELECT * FROM [testdata.natality] LIMIT {0}", rowsCount);
            var reader = command.ExecuteReader();
            int counter = 0;
            var list = new List<object[]>();
            while ((reader.Read())) {
                object[] values = new object[reader.FieldCount];
                list.Add(values);
                int i = reader.GetValues(values);
                Assert.AreEqual(i, reader.FieldCount);
                counter++;
            }

            Assert.AreEqual(rowsCount, counter);

        }
    }
}
#endif