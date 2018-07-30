using System.Globalization;

namespace DevExpress.DataAccess.BigQuery.Native {
    public static class ObjectUtils {
        public static string ToInvariantString(this object value, string format = "{0}") {
            return string.Format(CultureInfo.InvariantCulture, format, value);
        }
    }
}
