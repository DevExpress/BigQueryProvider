#if DEBUGTEST
using System;
using System.Data;
using Xunit;
namespace DevExpress.DataAccess.BigQuery.Tests {
    public class BigQueryParametersCollectionTests : IDisposable {
        public BigQueryParametersCollectionTests() {
            var paramCollection = new BigQueryParameterCollection();
            var param0 = new BigQueryParameter("param0", DbType.String);
            var param1 = new BigQueryParameter("param1", DbType.String);
            var param2 = new BigQueryParameter("param2", DbType.String);
            var param3 = new BigQueryParameter("param3", DbType.String);
            var param4 = new BigQueryParameter("param4", DbType.String);
            var param5 = new BigQueryParameter("param5", DbType.String);
            paramCollection.Insert(0, param0);
            paramCollection.Insert(1, param1);
            paramCollection.Insert(2, param2);
            paramCollection.Insert(3, param3);
            paramCollection.Insert(4, param4);
            paramCollection.Insert(5, param5);
            Collection = paramCollection;
        }

        public void Dispose() { }

        public BigQueryParameterCollection Collection { get; private set; }

        [Fact]
        public void RemoveByParameterTest() {
            var collection = Collection;
            var param = new BigQueryParameter("removeMe", DbType.String);
            collection.Insert(3, param);
            Assert.Equal(7, collection.Count);
            collection.Remove(param);
            Assert.Equal(6, collection.Count);
            Assert.False(collection.Contains(param));
            Assert.Throws<InvalidOperationException>(() => collection.Remove(param));
        }

        [Fact]
        public void RemoveAtTest() {
            var collection = Collection;
            Assert.Equal(6, collection.Count);
            collection.RemoveAt(3);
            Assert.Throws<IndexOutOfRangeException>(() => collection[5]);
            Assert.Equal(5, collection. Count);
            collection.RemoveAt(0);//param0
            collection.RemoveAt(0);//param1
            collection.RemoveAt(0);//param2
            Assert.Equal(2, collection.Count);
            collection.RemoveAt(0);//param4
            collection.RemoveAt(0);//param5
            Assert.Equal(0, collection.Count);
        }

        [Fact]
        public void AddRangeTest() {
            var collection = Collection;
            var arrayOfParameters = new BigQueryParameter[] {
                new BigQueryParameter("param6", DbType.String), 
                new BigQueryParameter("param7", DbType.String),
                new BigQueryParameter("param8", DbType.String), 
            };
            Assert.Equal(6, collection. Count);
            collection.AddRange(arrayOfParameters);
            Assert.Equal(9, collection.Count);
            Assert.Equal(6, collection.IndexOf("param6"));
            Assert.Equal(7, collection.IndexOf("param7"));
            Assert.Equal(8, collection.IndexOf("param8"));
        }

        [Fact]
        public void ContainsTest() {
            var collection = Collection;
            Assert.Equal(true, collection.Contains("param0"));
            Assert.Equal(false, collection.Contains("lalala"));
        }

        [Fact]
        public void IndexOfTest() {
            var collection = Collection;
            Assert.Equal(0, collection.IndexOf("param0"));
            Assert.Equal(1, collection.IndexOf("param1"));
            Assert.Equal(4, collection.IndexOf("param4"));
            Assert.Equal(-1, collection.IndexOf("param8"));
        }

        [Fact]
        public void ConstructorTest() {
            var paramCollection = new BigQueryParameterCollection();
            Assert.NotNull(paramCollection);
            Assert.Equal(0, paramCollection.Count);
            Assert.Empty(paramCollection);
            Assert.Equal(false, paramCollection.IsFixedSize);
            Assert.Equal(false, paramCollection.IsReadOnly);
        }

        [Fact]
        public void InsertTest() {
            var paramCollection = new BigQueryParameterCollection();
            var param0 = new BigQueryParameter("param0", DbType.String);
            paramCollection.Insert(0, param0);
            Assert.Equal(paramCollection[0], param0);
            Assert.Equal(paramCollection["param0"], param0);
            Assert.Equal(1, paramCollection.Count);

            var param1 = new BigQueryParameter("param1", DbType.String);
            paramCollection.Insert(1, param1);
            Assert.Equal(paramCollection[1], param1);
            Assert.Equal(paramCollection["param1"], param1);
            Assert.Equal(2, paramCollection.Count);

            var param2 = new BigQueryParameter("param2", DbType.String);
            paramCollection.Insert(2, param2);
            Assert.Equal(paramCollection[2], param2);
            Assert.Equal(paramCollection["param2"], param2);
            Assert.Equal(3, paramCollection.Count);
        }
    }
}
#endif