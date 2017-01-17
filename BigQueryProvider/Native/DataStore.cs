/*
   Copyright 2015 Developer Express Inc.

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

using System;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Json;
using Google.Apis.Util.Store;
using Newtonsoft.Json.Linq;

namespace DevExpress.DataAccess.BigQuery.Native {
    public class DataStore : IDataStore {
        string jsonContent;

        public DataStore(string refreshToken, string accessToken) {
            jsonContent = NewtonsoftJsonSerializer.Instance.Serialize(new TokenResponse {
                RefreshToken = refreshToken,
                AccessToken = accessToken,
                TokenType = "Bearer"
            });
        }

        public string AccessToken {
            get {
                JObject jObject = JObject.Parse(jsonContent);

                return (string)jObject.SelectToken("access_token");
            }
        }

        public string RefreshToken {
            get {
                JObject jObject = JObject.Parse(jsonContent);

                return (string)jObject.SelectToken("refresh_token");
            }
        }

        public Task StoreAsync<T>(string key, T value) {
            if(string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            jsonContent = NewtonsoftJsonSerializer.Instance.Serialize(value);

            return Task.Delay(0);
        }

        public Task DeleteAsync<T>(string key) {
            if(string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            jsonContent = null;
            return Task.Delay(0);
        }

        public Task<T> GetAsync<T>(string key) {
            if(string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
            try {
                tcs.SetResult(NewtonsoftJsonSerializer.Instance.Deserialize<T>(jsonContent));
            }
            catch(Exception ex) {
                tcs.SetException(ex);
            }
            return tcs.Task;
        }

        public Task ClearAsync() {
            jsonContent = null;
            return Task.Delay(0);
        }
    }
}
