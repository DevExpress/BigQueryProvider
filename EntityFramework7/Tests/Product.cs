using System;
using System.Collections.Generic;

namespace DevExpress.DataAccess.BigQuery.EntityFarmework7.Tests {
    public class Product {
        public Product() {
            Details = new HashSet<OrderDetail>();
        }

        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal ListPrice { get; set; }
        public DateTime SellStartDate { get; set; }
        public DateTime? SellEndDate { get; set; }

        public virtual ICollection<OrderDetail> Details { get; private set; }
    }
}