using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace RearCop.Common
{
    public class AdafruitHandler
    {
        private readonly AppConfig appConfig;
        private readonly Utilitities appUtilities;
        private readonly IMemoryCache cache;
        private readonly HttpClient appHttpClient;
        public AdafruitHandler(IOptions<AppConfig> pAppConfig,
        Utilitities pAppUtilities,
        IMemoryCache pCache,
        IHttpClientFactory pHttpClientFactory)
        {
            appConfig = pAppConfig.Value;
            appUtilities = pAppUtilities;
            cache=pCache;
            appHttpClient =pHttpClientFactory.CreateClient();
        }

        public void SendC2DPayload(
            ReturnModel devicePayload
            ,string deviceId
            ,string mqttURL
            ,string mqttUserName
            ,string mqttKey
            ,string value=null)
        {

            var messageJSON = appUtilities.SerializeObjectToJSON(devicePayload);
            dynamic message = new ExpandoObject();
            message.value = messageJSON;
            var contentJSON = JsonSerializer.Serialize(message);
            var content = new StringContent(contentJSON, Encoding.UTF8, "application/json");
            appHttpClient.DefaultRequestHeaders.Clear();
            appHttpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            appHttpClient.DefaultRequestHeaders.Remove("X-AIO-Key");
            appHttpClient.DefaultRequestHeaders.Add("X-AIO-Key",mqttKey);
            mqttURL = mqttURL + mqttUserName + "/feeds/" + deviceId +".c2d/data";
           
            try
            {
                var response = appHttpClient.PostAsync(mqttURL, content).Result;
                 if (!response.IsSuccessStatusCode)
                {
                    throw new ApplicationException("enable to send c2d");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        


        public void SendC2DPayload(string pinName
            ,string pinValue
            ,string deviceId
            ,string mqttURL
            ,string mqttUserName
            ,string mqttKey
            ,string value=null)
        {
            var messageJSON = "{\"" + pinName.ToLower() + "\":\"" + pinValue +"\"}";
            dynamic message = new ExpandoObject();
            message.value = messageJSON;
        
            var contentJSON = JsonSerializer.Serialize(message);
            var content = new StringContent(contentJSON, Encoding.UTF8, "application/json");
            appHttpClient.DefaultRequestHeaders.Clear();
            appHttpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            appHttpClient.DefaultRequestHeaders.Remove("X-AIO-Key");
            appHttpClient.DefaultRequestHeaders.Add("X-AIO-Key",mqttKey);
            mqttURL = mqttURL + mqttUserName + "/feeds/" + deviceId +".c2d/data";
           
            try
            {
                var response = appHttpClient.PostAsync(mqttURL, content).Result;
                 if (!response.IsSuccessStatusCode)
                {
                    throw new ApplicationException("enable to send c2d");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<AdaFruitModel> ReadFeedData(string deviceId
            , string mqttURL
            , string mqttUserName
            , string mqttKey
            ,string feedName)
        {

            List<AdaFruitModel> objResponse = new List<AdaFruitModel>();

            appHttpClient.DefaultRequestHeaders.Remove("X-AIO-Key");
            appHttpClient.DefaultRequestHeaders.Remove("Content-Type");
            appHttpClient.DefaultRequestHeaders.Add("X-AIO-Key", mqttKey);
            appHttpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            try
            {

                var response = appHttpClient.GetAsync("https://" + mqttURL + "/" + mqttUserName+ "/groups/" + deviceId + "/feeds/"+ feedName+"/data").Result;

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        using var responseStream = response.Content.ReadAsStreamAsync().Result;
                        objResponse = appUtilities.DeSerializeAdafruitFeedListObject(responseStream);
                    }
                    catch
                    {
                        return objResponse;
                    }
                }

                return objResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AdaFruitModel ReadFeedLastData(string deviceId
            , string mqttURL
            , string mqttUserName
            , string mqttKey
            ,string feedName)
        {

            var objResponse = new AdaFruitModel();
            appHttpClient.DefaultRequestHeaders.Remove("X-AIO-Key");
            appHttpClient.DefaultRequestHeaders.Add("X-AIO-Key", mqttKey);
            try
            {

                var response = appHttpClient.GetAsync(mqttURL +  mqttUserName+ "/feeds/" + deviceId + "."+ feedName +"/data/last").Result;

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        using var responseStream = response.Content.ReadAsStreamAsync().Result;
                        objResponse = appUtilities.DeSerializeAdafruitObject(responseStream);
                    }
                    catch (Exception ex)
                    {
                        return objResponse;
                    }
                }

                return objResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AdaFruitModel ReadPingFeedLastData(string deviceId
            , string mqttURL
            , string mqttUserName
            , string mqttKey
            ,string feedName)
        {

            var objResponse = new AdaFruitModel();
            appHttpClient.DefaultRequestHeaders.Remove("X-AIO-Key");
            appHttpClient.DefaultRequestHeaders.Add("X-AIO-Key", mqttKey);
            try
            {

                var response = appHttpClient.GetAsync(mqttURL +  mqttUserName+ "/feeds/" + feedName +"/data/last").Result;

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        using var responseStream = response.Content.ReadAsStreamAsync().Result;
                        objResponse = appUtilities.DeSerializeAdafruitObject(responseStream);
                    }
                    catch (Exception ex)
                    {
                        return objResponse;
                    }
                }

                return objResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool RegisterDevice(string deviceId,string mqttURL,string mqttUserName,string mqttKey)
        {
            bool returnValue=false;

            appHttpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            appHttpClient.DefaultRequestHeaders.Add("X-AIO-Key", mqttKey);

            dynamic runTimeObjectFeedName = new ExpandoObject();
            runTimeObjectFeedName.name = "c2d";
            runTimeObjectFeedName.history=false;

            dynamic runTimeObjectForFeeds = new ExpandoObject();
            runTimeObjectForFeeds.feed = runTimeObjectFeedName;
            
            var contentJSON = JsonSerializer.Serialize(runTimeObjectForFeeds);
            
            StringContent contentFeed = new StringContent(contentJSON, Encoding.UTF8, "application/json");
            string urlFeed = mqttURL + mqttUserName + "/groups/" + deviceId + "/feeds";

            dynamic runTimeObjectforGroup = new ExpandoObject();
            runTimeObjectforGroup.name = deviceId;

            var contentGroupJSON = JsonSerializer.Serialize(runTimeObjectforGroup);
            StringContent contentGroup = new StringContent(contentGroupJSON, Encoding.UTF8);
            contentGroup.Headers.Remove("Content-Type");
            contentGroup.Headers.Add("Content-Type", "application/json");

      
            string urlGroups = mqttURL + mqttUserName + "/groups";
          
            var responseGroup = appHttpClient.PostAsync(urlGroups, contentGroup).Result;

            if (responseGroup.IsSuccessStatusCode)
            {
                var responseFeed = appHttpClient.PostAsync(urlFeed, contentFeed).Result;
                if (responseFeed.IsSuccessStatusCode)
                {
                    returnValue =true;
                }
            }
            return returnValue;
        }

    }
}