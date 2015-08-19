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

            return TaskEx.Delay(0);
        }

        public Task DeleteAsync<T>(string key) {
            if(string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            jsonContent = null;
            return TaskEx.Delay(0);
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
            return TaskEx.Delay(0);
        }
    }
}
