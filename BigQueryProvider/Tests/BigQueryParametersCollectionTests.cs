#if DEBUGTEST
using System;
using System.Data;
using Xunit;
namespace DevExpress.DataAccess.BigQuery.Tests {
    public class BigQueryParametersCollectionTests : IDisposable {
        readonly BigQueryParameter param0 = new BigQueryParameter("param0", DbType.String);
        readonly BigQueryParameter param1 = new BigQueryParameter("param1", DbType.String);
        readonly BigQueryParameter param2 = new BigQueryParameter("param2", DbType.String);
        readonly BigQueryParameter param3 = new BigQueryParameter("param3", DbType.String);
        readonly BigQueryParameter param4 = new BigQueryParameter("param4", DbType.String);
        readonly BigQueryParameter param5 = new BigQueryParameter("param5", DbType.String);
        readonly BigQueryParameter nonexistentParameter = new BigQueryParameter("foo", DbType.String);

        public BigQueryParametersCollectionTests() {
            var paramCollection = new BigQueryParameterCollection();
            paramCollection.Insert(0, param0);
            paramCollection.Insert(1, param1);
            paramCollection.Insert(2, param2);
            paramCollection.Insert(3, param3);
            paramCollection.Insert(4, param4);
            paramCollection.Insert(5, param5);
            Collection = paramCollection;
        }

        public void Dispose() { }

        public BigQueryParameterCollection Collection { get; set; }

        [Fact]
        public void RemoveTest() {
            var count = Collection.Count;
            Collection.Remove(param3);
            Assert.Equal(count - 1, Collection.Count);
            Assert.False(Collection.Contains(param3));
            Assert.Throws<InvalidOperationException>(() => Collection.Remove(param3));
        }

        [Fact]
        public void RemoveAtTest() {
            var count = Collection.Count;
            Collection.RemoveAt(3);
            Assert.Throws<IndexOutOfRangeException>(() => Collection[5]);
            Assert.Equal(count - 1, Collection. Count);
            Collection.RemoveAt(0);
            Collection.RemoveAt(0);
            Collection.RemoveAt(0);
            Collection.RemoveAt(0);
            Collection.RemoveAt(0);
            Assert.Equal(count - 6, Collection.Count);
        }

        [Fact]
        public void ContainsTest() {
            Assert.True(Collection.Contains(param0));
            Assert.False(Collection.Contains(nonexistentParameter));
        }

        [Fact]
        public void IndexOfTest() {
            Assert.Equal(0, Collection.IndexOf(param0));
            Assert.Equal(1, Collection.IndexOf(param1));
            Assert.Equal(4, Collection.IndexOf(param4.ParameterName));
            Assert.Equal(-1, Collection.IndexOf(nonexistentParameter));
            Assert.Equal(-1, Collection.IndexOf(nonexistentParameter.ParameterName));
        }

        [Fact]
        public void ConstructorTest() {
            var paramCollection = new BigQueryParameterCollection();
            Assert.Equal(0, paramCollection.Count);
            Assert.Empty(paramCollection);
            Assert.Equal(false, paramCollection.IsFixedSize);
            Assert.Equal(false, paramCollection.IsReadOnly);
        }

        [Fact]
        public void InsertTest() {
            var paramCollection = new BigQueryParameterCollection();
            paramCollection.Insert(0, param0);
            Assert.Equal(param0, paramCollection[0]);
            Assert.Equal(param0, paramCollection["param0"]);
            Assert.Equal(1, paramCollection.Count);

            paramCollection.Insert(1, param1);
            Assert.Equal(param1, paramCollection[1]);
            Assert.Equal(param1, paramCollection["param1"]);
            Assert.Equal(2, paramCollection.Count);

            paramCollection.Insert(2, param2);
            Assert.Equal(param2, paramCollection[2]);
            Assert.Equal(param2, paramCollection["param2"]);
            Assert.Equal(3, paramCollection.Count);
        }

        [Fact]
        public void AddRangeTest() {
            var collection = new BigQueryParameterCollection();
            var arrayOfParameters = new[] {param0, param1, param2,};
            collection.AddRange(arrayOfParameters);
            Assert.Equal(3, collection.Count);
            Assert.Equal(0, collection.IndexOf(param0));
            Assert.Equal(1, collection.IndexOf(param1));
            Assert.Equal(2, collection.IndexOf(param2));
        }

        [Fact]
        public void ValidateTest() {
            var collection = new BigQueryParameterCollection();
            collection.Add(param0);
            Assert.Throws<ArgumentException>(() => collection.Validate());
        }

        [Fact]
        public void AddTest() {
            var collection = new BigQueryParameterCollection();
            Assert.Equal(0, collection.Add(param0));
            Assert.Throws<ArgumentNullException>(() => collection.Add(null));
            Assert.Throws<Exception>(() => collection.Add("notParameter"));
        }

        [Fact]
        public void ClearTest() {
            Assert.NotEmpty(Collection);
            Collection.Clear();
            Assert.Empty(Collection);
        }

        [Fact]
        public void CopyTo() {
            var count = Collection.Count;
            BigQueryParameter[] collectionForCopy = new BigQueryParameter[count];
            Collection.CopyTo(collectionForCopy, 0);
            Assert.Equal(count, collectionForCopy.Length);
            for(int i = 0; i < count; i++) {
                Assert.Equal(Collection[i], collectionForCopy[i]);
            }
        }

        [Fact]
        public void GetEnumerator() {
            var enumerator = Collection.GetEnumerator();
            Assert.NotNull(enumerator);
        }
    }
}
#endif