/*
   Copyright 2015-2018 Developer Express Inc.

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

#if DEBUGTEST
using System.Configuration;

namespace DevExpress.DataAccess.BigQuery.Tests {
    public static class ConnectionStringHelper {
        public static string OAuthConnectionString {
            get { return ConfigurationManager.ConnectionStrings["bigqueryConnectionStringOAuth"].ConnectionString; }
        }
        public static string P12ConnectionString {
            get { return ConfigurationManager.ConnectionStrings["bigqueryConnectionStringP12"].ConnectionString; }
        }
    }
}
#endif