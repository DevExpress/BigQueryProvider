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

using Microsoft.Extensions.Configuration;

namespace DevExpress.DataAccess.BigQuery.Tests {
    public static class ConnectionStringHelper {
        static IConfigurationRoot config;
        static IConfigurationRoot Config {
            get {
                if (config == null) {
                    var builder = new ConfigurationBuilder()
                        .AddJsonFile(System.IO.Path.Combine(Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationBasePath, "appsettings.json"));

                    config = builder.Build();
                }
                return config;
            }
        }
        static string GetConnectionString(string name) {
            return Config.GetSection("ConnectionStrings")[name];
        }

        public static string OAuthConnectionString => GetConnectionString("bigqueryConnectionStringOAuth");

        public static string P12ConnectionString => GetConnectionString("bigqueryConnectionStringP12");

        public static string JsonConnectionString => GetConnectionString("bigqueryConnectionStringJson");
    }
}