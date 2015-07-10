using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DevExpress.DataAccess.BigQuery.Tests {
    public class DataTableComparer : IEqualityComparer<DataTable> {
        public static bool Equals(DataTable x, DataTable y) {
            if(x == null && y == null)
                return true;
            if(x == null || y == null)
                return false;
            if(x.Rows.Count != y.Rows.Count || x.Columns.Count != y.Columns.Count)
                return false;
            for(int i = 0; i < x.Rows.Count; i++) {
                if(!DataRowComparer.Equals(x.Rows[i], y.Rows[i]))
                    return false;
            }
            return true;
        }

        bool IEqualityComparer<DataTable>.Equals(DataTable x, DataTable y) {
            return Equals(x, y);
        }

        int IEqualityComparer<DataTable>.GetHashCode(DataTable obj) {
            return obj.GetHashCode();
        }
    }

    public class DataRowComparer : IEqualityComparer<DataRow> {
        bool IEqualityComparer<DataRow>.Equals(DataRow x, DataRow y) {
            return Equals(x, y);
        }

        int IEqualityComparer<DataRow>.GetHashCode(DataRow obj) {
            return obj.GetHashCode();
        }

        public static bool Equals(DataRow x, DataRow y) {
            if(x == null && y == null)
                return true;
            if(x == null || y == null)
                return false;
            if(x.ItemArray.Count() != y.ItemArray.Count())
                return false;
            IEnumerator xEnumerator = x.ItemArray.GetEnumerator();
            IEnumerator yEnumerator = y.ItemArray.GetEnumerator();
            while(xEnumerator.MoveNext() && yEnumerator.MoveNext()) {
                var xCurrent = xEnumerator.Current;
                var xType = xCurrent.GetType();
                var yCurrent = yEnumerator.Current;
                var yType = yCurrent.GetType();
                if(xType != yType)
                    return false;
                if(!(xType.IsValueType || xType == typeof(string) ? xCurrent.Equals(yCurrent) : xCurrent == yCurrent))
                    return false;
            }
            return true;
        }
    }
}
