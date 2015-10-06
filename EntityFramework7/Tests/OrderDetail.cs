using System;

namespace DevExpress.DataAccess.BigQuery.EntityFarmework7.Tests {
    public class OrderDetail {
        public int OrderDetailID { get; set; }
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public int OrderQty { get; set; }
        public decimal UnitPrice { get; set; }
        public int ProductID { get; set; }

        public virtual Product Product { get; set; }
        public virtual OrderHeader Header { get; set; }
    }
}