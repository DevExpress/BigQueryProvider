using Google;

namespace DevExpress.DataAccess.BigQuery.Native {
    public static class GoogleApiExceptionHelper {
        public static BigQueryException Wrap(this GoogleApiException exception) {
            return new BigQueryException(exception.Message, exception);
        }
    }
}