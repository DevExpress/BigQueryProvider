using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Xunit;

namespace DevExpress.DataAccess.BigQuery.EntityFarmework7.Tests {
    public class EntityTests {
        [Fact]
        public void EntityTestSelectAllLists() {
            TestDataEntities context = new TestDataEntities();
            List<Product> products = context.Products.ToList();
            Assert.Equal(13, products.Count);
            List<OrderDetail> details = context.OrderDetails.ToList();
            Assert.Equal(41, details.Count);
            List<OrderHeader> orders = context.OrderHeaders.ToList();
            Assert.Equal(13, orders.Count);
        }

        [Fact]
        public void EntityTestEagerLoading() {
            TestDataEntities context = new TestDataEntities();
            List<OrderHeader> orders = context.OrderHeaders.Include(d => d.Details).ThenInclude(p => p.Product).ToList();
            Assert.Equal(13, orders.Count);
        }

        [Fact]
        public void EntityTestLimit() {
            TestDataEntities context = new TestDataEntities();
            List<OrderHeader> orders = context.OrderHeaders.Take(3).ToList();
            Assert.Equal(3, orders.Count);
        }

        [Fact]
        public void EntityTestOffset() {
            TestDataEntities context = new TestDataEntities();
            Assert.Throws<NotSupportedException>(() => {
                List<OrderHeader> orders = context.OrderHeaders.Skip(3).ToList();
                Assert.Equal(10, orders.Count);
            });
        }

        [Fact]
        public void EntityTestSumInt() {
            TestDataEntities context = new TestDataEntities();
            int sum = context.OrderHeaders.Sum(o => o.Status);
            Assert.Equal(35, sum);
        }

        [Fact]
        public void EntityTestSumDecimal() {
            TestDataEntities context = new TestDataEntities();
            decimal sum = context.Products.Sum(o => o.ListPrice);
            Assert.Equal(474626.1m, sum);
        }

        [Fact]
        public void EntityTestCount() {
            TestDataEntities context = new TestDataEntities();
            int sum = context.Products.Count(o => o.ListPrice < 20000m);
            Assert.Equal(3, sum);
        }
    }
}
