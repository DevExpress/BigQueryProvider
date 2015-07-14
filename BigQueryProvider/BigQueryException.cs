using System.Data.Common;

namespace DevExpress.DataAccess.BigQuery {
    public class BigQueryException : DbException {
        public BigQueryException(string message) : base(message) { }

        public BigQueryException(string message, Google.GoogleApiException innerException) : base(message, innerException) { }
    }

    internal static class GoogleApiExceptionHelper {
        internal static BigQueryException Wrap(this Google.GoogleApiException exception) {
            return new BigQueryException(exception.Message, exception);
        }
    }
}
