using System.Globalization;

namespace DevExpress.DataAccess.BigQuery.Native {
    public static class ObjectUtils {
        public static string ToInvariantString(this object value) {
            return ToInvariantString(value, "{0}");
        }

        public static string ToInvariantString(this object value, string format) {
            return string.Format(CultureInfo.InvariantCulture, format, value);
        }
    }
}
