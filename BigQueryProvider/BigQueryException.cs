using System.Data.Common;
using Google;

namespace DevExpress.DataAccess.BigQuery {
    public class BigQueryException : DbException {
        public BigQueryException(string message) : base(message) { }

        public BigQueryException(string message, GoogleApiException innerException) : base(message, innerException) { }
    }

    internal static class GoogleApiExceptionHelper {
        internal static BigQueryException Wrap(this GoogleApiException exception) {
            return new BigQueryException(exception.Message, exception);
        }
    }
}
