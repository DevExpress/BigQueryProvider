using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using NUnit.Framework;

namespace DevExpress.DataAccess.BigQuery.Tests {
    [TestFixture]
    public class BigQueryTests {
        [Test]
        public void OpenConnectionTest() {
            var connStringBuilder = new DbConnectionStringBuilder();
            connStringBuilder["PrivateKeyFileName"] = Program.PrivateKeyFileName;
            connStringBuilder["ProjectID"] = "zymosimeter";
            connStringBuilder["ServiceAccountEmail"] = "227277881286-l0fodnq2h35m58b80up9vi4g83p1ogus@developer.gserviceaccount.com";
            var connection = new BigQueryConnection(connStringBuilder.ConnectionString);
            connection.Open();
            Assert.AreEqual(ConnectionState.Open, connection.State);

            connection.Close();
            Assert.AreEqual(ConnectionState.Closed, connection.State);
        }
        
        [Test]
        public void DataReaderTest() {
            int rowsCount = 10;
            var connStringBuilder = new DbConnectionStringBuilder();
            connStringBuilder["PrivateKeyFileName"] = Program.PrivateKeyFileName;
            connStringBuilder["ProjectID"] = "zymosimeter";
            connStringBuilder["ServiceAccountEmail"] = "227277881286-l0fodnq2h35m58b80up9vi4g83p1ogus@developer.gserviceaccount.com";
            connStringBuilder["DataSetId"] = "testdata";
            var connection = new BigQueryConnection(connStringBuilder.ConnectionString);
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
