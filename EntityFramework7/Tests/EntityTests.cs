using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DevExpress.DataAccess.BigQuery.EntityFarmework7.Tests {
    public class EntityTests {
        [Fact]
        public void EntityTest() {
            TestDataEntities context = new TestDataEntities();
            List<Product> products = context.Products.ToList();
            Assert.Equal(13, products.Count);
            List<OrderDetail> details = context.OrderDetails.ToList();
            Assert.Equal(41, details.Count);
            List<OrderHeader> orders = context.OrderHeaders.ToList();
            Assert.Equal(13, orders.Count);
        }

    }
}
