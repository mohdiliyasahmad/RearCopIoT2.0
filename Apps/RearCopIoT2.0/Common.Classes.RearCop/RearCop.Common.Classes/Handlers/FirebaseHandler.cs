using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace RearCop.Common
{
    public class FirebaseHandler
    {
        private readonly IMemoryCache cache;
        private readonly HttpClient httpClient;
        private readonly AppConfig appConfig;
        private readonly Utilitities appUtilitites;
        public FirebaseHandler(IHttpClientFactory httpClientFactory, 
        IOptions<AppConfig> pAppConfig,
        IMemoryCache pCache,
        Utilitities pAppUtlities)
        {
            httpClient = httpClientFactory.CreateClient();
            appConfig = pAppConfig.Value;
            cache =pCache;
            appUtilitites =pAppUtlities;
        }
       
        public bool ValidateDeviceAsync(string deviceToken)
        {
            return  GetDeviceFromCacheAsync(deviceToken);
        }

        public bool ValidateDeviceTokenAsync(string deviceToken)
        {
            var url = appConfig.DatabaseServerUri + "Devices/" + deviceToken + "/" + deviceToken + appConfig.DatabaseServerUriPostFix;

            string result = string.Empty;

            try
            {
                var response = httpClient.GetAsync(url).Result;
            
                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                }
                
                result = JsonSerializer.Deserialize<string>(result);
                return deviceToken == result;
            }
            catch
            {
                throw new ApplicationException("device token not authorized");
            }
        }

        public async Task<string> GetDataAsync(string url, string accessToken, HttpClient httpClient)
        {
            string result = string.Empty;
            try
            {
               var response = await httpClient.GetAsync(url);
            
               if (response.IsSuccessStatusCode)
                {
                    var obj = response.Content.ReadAsStringAsync().Result;
                    return obj;
                }
                else
                {
                    return null;
                }
              
            }
            catch (Exception ex)
            {
               return result + ex.Message;
            }
        }

        public async Task<string> PutDataAsync(string url, string jsonString, HttpClient httpClient, string accessToken=null)
        {
            string result = string.Empty;
            try
            {
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync(url, content);

               if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                }
                return result = jsonString;
            }
            catch 
            {
                return result;
            }
        }

        public ReturnModel GetDeviceData(string deviceID,
                    string serverURL,
                    string serverUri,
                    string serverUriPostFix,
                    HttpClient httpClient)
        {
            serverURL = serverUri +"/Devices/"+ deviceID +"/Transaction/";
            var result = this.GetDataAsync(serverURL + serverUriPostFix, null, httpClient);

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var obj = JsonSerializer.Deserialize<ReturnModel>(result.GetAwaiter().GetResult());//,serializeOptions);

            return obj;
        }

        public ReturnModel CookDataBeforeSend(ReturnModel obj)
        {
            obj.V1 = string.IsNullOrEmpty(obj.V1) ? "0" : obj.V1;
            obj.V2 = string.IsNullOrEmpty(obj.V2) ? "0" : obj.V2;
            obj.V3 = string.IsNullOrEmpty(obj.V3) ? "0" : obj.V3;
            obj.V4 = string.IsNullOrEmpty(obj.V4) ? "0" : obj.V4;
            obj.V5 = string.IsNullOrEmpty(obj.V5) ? "0" : obj.V5;
            obj.V6 = string.IsNullOrEmpty(obj.V6) ? "0" : obj.V6;
            obj.V7 = string.IsNullOrEmpty(obj.V7) ? "0" : obj.V7;
            obj.V8 = string.IsNullOrEmpty(obj.V8) ? "0" : obj.V8;
            obj.V9 = string.IsNullOrEmpty(obj.V9) ? "0" : obj.V9;
            obj.V10 = string.IsNullOrEmpty(obj.V10) ? "0" : obj.V10;
            obj.V11 = string.IsNullOrEmpty(obj.V11) ? "0" : obj.V11;
            obj.V12 = string.IsNullOrEmpty(obj.V12) ? "0" : obj.V12;
            obj.V13 = string.IsNullOrEmpty(obj.V13) ? "0" : obj.V13;
            obj.V14 = string.IsNullOrEmpty(obj.V14) ? "0" : obj.V14;
            obj.V15 = string.IsNullOrEmpty(obj.V15) ? "0" : obj.V15;
            obj.V16 = string.IsNullOrEmpty(obj.V16) ? "0" : obj.V16;
            //obj.DefaultValues.ResetTime = string.IsNullOrEmpty(obj.DefaultValues.ResetTime) ? "5" : obj.DefaultValues.ResetTime;

            return obj;
        }


        public bool GetDeviceFromCacheAsync(string deviceToken)
        {
            var cacheKey = deviceToken + "-device";
            var deviceID = string.Empty;
		 
            if (cache.TryGetValue(cacheKey, out deviceID))
            {
                return true;
            }
            else
            {
                 // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.MaxValue);

                var returnResult = ValidateDeviceTokenAsync(deviceToken);
                if (returnResult)
                {
                    cache.Set(cacheKey,deviceToken,cacheEntryOptions);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool RegisterDeviceAsync(string deviceID)
        {
            var url = appConfig.DatabaseServerUri + "Devices/" + deviceID
            + "/" + deviceID
            + appConfig.DatabaseServerUriPostFix;

            var payLoad = appUtilitites.SerializeObjectToJSON(deviceID);
            try
            {
                PutDataAsync(url, payLoad, httpClient);
                return true;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

    }
}