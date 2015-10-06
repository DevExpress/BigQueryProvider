using System;
using System.Collections.Generic;

namespace DevExpress.DataAccess.BigQuery.EntityFarmework7.Tests {
    public class OrderHeader {
        public OrderHeader() {
            Details = new HashSet<OrderDetail>();
        }

        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }

        public virtual ICollection<OrderDetail> Details { get; private set; }
    }
}