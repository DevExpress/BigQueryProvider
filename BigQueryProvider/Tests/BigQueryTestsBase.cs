#if DEBUGTEST
using System;
using System.Collections.Generic;
using System.Data;
using Xunit;

namespace DevExpress.DataAccess.BigQuery.Tests {
    public abstract class BigQueryTestsBase  {
        protected abstract string GetConnectionString();

        [Fact]
        public void OpenConnectionTest() {
            var connection = new BigQueryConnection(ConnectionStringHelper.OAuthConnectionString);
            connection.Open();
            Assert.Equal(ConnectionState.Open, connection.State);

            connection.Close();
            Assert.Equal(ConnectionState.Closed, connection.State);
        }
        
        [Fact]
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
                Assert.Equal(i, reader.FieldCount);
                counter++;
            }

            Assert.Equal(rowsCount, counter);

        }

    }
}
#endif
