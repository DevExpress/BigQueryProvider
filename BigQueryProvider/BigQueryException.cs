/*
   Copyright 2015-2017 Developer Express Inc.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System.Data.Common;
using Google;

namespace DevExpress.DataAccess.BigQuery {
    /// <summary>
    /// The exception thrown when BigQuery returns a server-side warning or error.
    /// </summary>
    public class BigQueryException : DbException {
        /// <summary>
        /// Initializes a new instance of the BigQueryExcpetion class with the specified message.
        /// </summary>
        /// <param name="message">The message describing the current exception.</param>
        public BigQueryException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the BigQueryExcpetion class with the specified settings.
        /// </summary>
        /// <param name="message">The message describing the current exception.</param>
        /// <param name="innerException">A GoogleApiException object representing.</param>
        public BigQueryException(string message, GoogleApiException innerException) : base(message, innerException) { }
    }
}
