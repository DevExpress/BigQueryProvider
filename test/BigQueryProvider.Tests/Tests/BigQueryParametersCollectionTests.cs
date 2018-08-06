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
    public class BigQueryParametersCollectionTests {
        const int param0Index = 0;
        const int param1Index = 1;
        const int param2Index = 2;
        const int param3Index = 3;
        const int param4Index = 4;
        const int param5Index = 5;
        const int nonExistentIndex = -1;
        readonly BigQueryParameter param0 = new BigQueryParameter("@param0", DbType.String);
        readonly BigQueryParameter param1 = new BigQueryParameter("@param1", DbType.String);
        readonly BigQueryParameter param2 = new BigQueryParameter("@param2", DbType.String);
        readonly BigQueryParameter param3 = new BigQueryParameter("@param3", DbType.String);
        readonly BigQueryParameter param4 = new BigQueryParameter("@param4", DbType.String);
        readonly BigQueryParameter param5 = new BigQueryParameter("@param5", DbType.String);
        readonly BigQueryParameter nonexistentParameter = new BigQueryParameter("@foo", DbType.String);

        public BigQueryParametersCollectionTests() {
            var paramCollection = new BigQueryParameterCollection();
            paramCollection.Insert(param0Index, param0);
            paramCollection.Insert(param1Index, param1);
            paramCollection.Insert(param2Index, param2);
            paramCollection.Insert(param3Index, param3);
            paramCollection.Insert(param4Index, param4);
            paramCollection.Insert(param5Index, param5);
            Collection = paramCollection;
        }

        public BigQueryParameterCollection Collection { get; }

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
            Collection.RemoveAt(param2Index);
            Assert.Equal(count - 1, Collection.Count);
            count = Collection.Count;
            Assert.False(Collection.Contains(param2));
            for(int i = 0; i < count; i++)
                Collection.RemoveAt(0);
            Assert.Equal(0, Collection.Count);
            Assert.Empty(Collection);
        }

        [Fact]
        public void ContainsTest() {
            Assert.True(Collection.Contains(param0));
            Assert.False(Collection.Contains(nonexistentParameter));
        }

        [Fact]
        public void IndexOfTest() {
            Assert.Equal(param0Index, Collection.IndexOf(param0));
            Assert.Equal(param3Index, Collection.IndexOf(param3));
            Assert.Equal(param4Index, Collection.IndexOf(param4.ParameterName));
            Assert.Equal(param5Index, Collection.IndexOf(param5.ParameterName));

            Assert.Equal(nonExistentIndex, Collection.IndexOf(nonexistentParameter));
            Assert.Equal(nonExistentIndex, Collection.IndexOf(nonexistentParameter.ParameterName));
        }

        [Fact]
        public void InitialState() {
            var paramCollection = new BigQueryParameterCollection();
            Assert.Equal(0, paramCollection.Count);
            Assert.Empty(paramCollection);
            Assert.False(paramCollection.IsFixedSize);
            Assert.False(paramCollection.IsReadOnly);
        }

        [Fact]
        public void InsertTest() {
            var paramCollection = new BigQueryParameterCollection();
            paramCollection.Insert(param0Index, param0);
            Assert.Same(param0, paramCollection[param0Index]);
            Assert.Same(param0, paramCollection[param0.ParameterName]);
            Assert.Equal(1, paramCollection.Count);

            paramCollection.Insert(param1Index, param1);
            Assert.Same(param1, paramCollection[param1Index]);
            Assert.Same(param1, paramCollection[param1.ParameterName]);
            Assert.Equal(2, paramCollection.Count);

            paramCollection.Insert(0, param2);
            Assert.Same(param2, paramCollection[0]);
            Assert.Same(param2, paramCollection[param2.ParameterName]);
            Assert.Equal(3, paramCollection.Count);

            Assert.Same(param0, paramCollection[param0Index + 1]);
            Assert.Same(param0, paramCollection[param0.ParameterName]);

            Assert.Same(param1, paramCollection[param1Index + 1]);
            Assert.Same(param1, paramCollection[param1.ParameterName]);
        }

        [Fact]
        public void AddRangeTest() {
            var collection = new BigQueryParameterCollection();
            var arrayOfParameters = new[] {param0, param1, param2};
            collection.AddRange(arrayOfParameters);
            Assert.Equal(arrayOfParameters.Length, collection.Count);
            for(int i = 0; i < arrayOfParameters.Length; i++) {
                Assert.Same(arrayOfParameters[i], collection[i]);
            }
        }

        [Fact]
        public void ValidateDuplicateParametersTest() {
            var collection = new BigQueryParameterCollection();
            var param = new BigQueryParameter("@param1", DbType.Object);
            collection.Add(param);
            collection.Add(param);
            Assert.Throws<DuplicateNameException>(() => collection.Validate());
        }

        [Fact]
        public void ValidateBadParameterTest() {
            var collection = new BigQueryParameterCollection();
            collection.Add(param0);
            Assert.Throws<ArgumentException>(() => collection.Validate());
        }

        [Fact]
        public void AddTest() {
            var collection = new BigQueryParameterCollection();
            Assert.Equal(0, collection.Add(param0));
            Assert.Throws<ArgumentNullException>(() => collection.Add(null));
            Assert.Throws<ArgumentException>(() => collection.Add("notParameter"));
            Assert.Equal(1, collection.Add("parameter", DbType.String));
            Assert.Equal("parameter", collection[1].ParameterName);
            Assert.Equal(DbType.String, collection[1].DbType);
        }

        [Fact]
        public void ClearTest() {
            Assert.NotEmpty(Collection);
            Collection.Clear();
            Assert.Empty(Collection);
        }

        [Fact]
        public void CopyToTest() {
            var count = Collection.Count;
            BigQueryParameter[] collectionForCopy = new BigQueryParameter[count];
            Collection.CopyTo(collectionForCopy, 0);
            Assert.Equal(count, collectionForCopy.Length);
            for(int i = 0; i < count; i++) {
                Assert.Same(Collection[i], collectionForCopy[i]);
            }
        }

        [Fact]
        public void GetEnumeratorTest() {
            var enumerator = Collection.GetEnumerator();
            Assert.NotNull(enumerator);
        }
    }
}