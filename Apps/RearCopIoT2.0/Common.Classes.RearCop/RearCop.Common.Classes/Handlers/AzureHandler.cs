using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.Azure.Devices;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace RearCop.Common
{
    public class AzureHandler
    {
        private readonly AppConfig appConfig;
        private readonly Utilitities appUtilities;
        private readonly IMemoryCache cache;
        private readonly HttpClient appHttpClient;
        public AzureHandler(IOptions<AppConfig> pAppConfig,
        Utilitities pAppUtilities,
        IMemoryCache pCache,
        IHttpClientFactory pHttpClientFactory)
        {
            appConfig = pAppConfig.Value;
            appUtilities = pAppUtilities;
            cache=pCache;
            appHttpClient =pHttpClientFactory.CreateClient();
        }

        public string GetDeviceConnectionString(string saSToken, string deviceId)
        {
            var connectionString = string.Empty;
            var deviceSaSToken =Microsoft.Azure.Devices.Client.AuthenticationMethodFactory.CreateAuthenticationWithToken(deviceId,saSToken);
            connectionString = Microsoft.Azure.Devices.Client.IotHubConnectionStringBuilder.Create(appConfig.IoTHubName, deviceSaSToken).ToString();
            return connectionString;
        }

        public void SendPayload(ReturnModel devicePayload,string deviceId, string conString)
        {
            var messageStr = appUtilities.SerializeObjectToJSON(devicePayload);
            var message = new Microsoft.Azure.Devices.Message(Encoding.ASCII.GetBytes(messageStr));
           
            var serviceClient = ServiceClient.CreateFromConnectionString(conString);
            try
            {
                serviceClient.PurgeMessageQueueAsync(deviceId);
                serviceClient.SendAsync(deviceId, message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        private string CreateToken(string resourceUri,
                                   string keyName,
                                   string key)
        {
            //For example we decided to create a token expiring in a week
            TimeSpan sinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
            var week = ((60 * 60 * 24 * 7)*2)+5000;
            //expiry 5 min
            //var hrs = 60 * 5;
            var expiry = Convert.ToString(sinceEpoch.TotalSeconds + week);
            //IoT Hub Security docs here: https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-security
            string stringToSign = HttpUtility.UrlEncode(resourceUri) + "\n" + expiry;
            HMACSHA256 hmac = new HMACSHA256(Convert.FromBase64String(key));
            var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
            var sasToken = String.Format(CultureInfo.InvariantCulture, 
                                        "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}", 
                                        HttpUtility.UrlEncode(resourceUri), 
                                        HttpUtility.UrlEncode(signature), 
                                        expiry, 
                                        keyName);
            return sasToken;
        }

        public string GetSaSTokenFromCache(string deviceToken)
        {
            var cacheKey = deviceToken + "-sastoken";
            var sasToken =string.Empty;

		    if (cache.TryGetValue(cacheKey, out sasToken))
            {
                return sasToken;
            }
            else
            {
                sasToken = CreateToken(appConfig.IoTHubName, 
                        appConfig.IoTHubConnectionStrings.FirstOrDefault().IoTHubPolicyName,
                        appConfig.IoTHubConnectionStrings.FirstOrDefault().IoTHubPrimeryKey);

                 // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(14));

                cache.Set(cacheKey,sasToken, cacheEntryOptions);
                return sasToken;
            }
        }


        public HubDeviceResponse GetAzureDevice(string deviceId)
        {
            var obj= new HubDevice();
            obj.DeviceId = deviceId;
            var objResponce = new HubDeviceResponse();

            obj.SaSToken = GetSaSTokenFromCache(deviceId);

            appHttpClient.DefaultRequestHeaders.Remove("Authorization");
            appHttpClient.DefaultRequestHeaders.Remove("Accept");

            appHttpClient.DefaultRequestHeaders.Add("Authorization", obj.SaSToken);
            appHttpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var response = appHttpClient.GetAsync("https://"+ appConfig.IoTHubName +"/devices/"+ 
            obj.DeviceId +"?api-version=" 
            + appConfig.IoTHubAPIVersion).Result;

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    using var responseStream = response.Content.ReadAsStreamAsync().Result;
                    objResponce = appUtilities.DeSerializeObject(responseStream);
                }
                catch
                {
                    return objResponce;
                }
            }
            else
            {
                obj = new HubDevice();
            }

            return objResponce;
        }
    }
}