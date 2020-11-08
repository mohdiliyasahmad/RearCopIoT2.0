using Microsoft.AspNetCore.Mvc;
using RearCop.Common;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Dynamic;
using System.Collections.Generic;

namespace Gateway.RearCop.Controllers
{
    [ApiController]
    [Route("/")]
    public class GatewayController : GatewayControllerBase
    {
        public static HttpClient HttpRegisterDevice { get; set; }
        public static HttpClient HttpDeviceStatus { get; set; }
        public static HttpClient HttpC2D { get; set; }
        public static HttpClient HttpD2C { get; set; }
        public static HttpClient HttpService { get; set; }
        public IHttpClientFactory GatewayProvider { get; set; }

        public GatewayController(IHttpClientFactory pclientFactory,
        ILogger<GatewayControllerBase> errorLogger,
        IOptions<GatewayConfig> pConfig,
        IOptions<AppConfig> pAppConfig,
        FirebaseHandler pFirebaseHandler,
        Utilitities pAppUtilities) 
        : base (pclientFactory, errorLogger, pConfig, pAppConfig, pFirebaseHandler,pAppUtilities) { 

           
        }

        [Route("RegisterDevice")]
        public void RegisterDevice()
        {
            try
            {
                CallRegisterMicroService();
            }
            catch (System.Exception ex)
            {
                Response.StatusCode = StatusCodes.Status428PreconditionRequired;
                throw new ApplicationException(ex.Message);
            }
        }

        [Route("Get")]
        [HttpGet]
        public async System.Threading.Tasks.Task<ReturnModel> ServiceAsync()
        {
            try
            {
              return await CallServiceAsync("GET");
            }
            catch (System.Exception ex)
            {
                Response.StatusCode = StatusCodes.Status428PreconditionRequired;
                throw new ApplicationException(ex.Message);
            }
        }

        [Route("Get/{pinName}")]
        [HttpGet("{pinName}")]
        public string Service(string pinName)
        {
            var value = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(pinName.Trim()))
                {
                    Response.StatusCode = StatusCodes.Status428PreconditionRequired;
                    throw new ApplicationException("Pin not passed");
                }

                var objReturnModel = CallServiceAsync("GET", pinName, value).Result;
                value = (string)objReturnModel.GetType().GetProperty(pinName).GetValue(objReturnModel, null);
                return value;

            }
            catch (System.Exception ex)
            {
                Response.StatusCode = StatusCodes.Status428PreconditionRequired;
                throw new ApplicationException(ex.Message);
            }

        }

        [Route("Get/{pinName}/{value}")]
        [HttpGet("{pinName}/{value}")]
        public async System.Threading.Tasks.Task<string> ServiceAsync(string pinName,string value)
        {
            try
            {
                if(string.IsNullOrEmpty(pinName.Trim()))
                {
                    Response.StatusCode = StatusCodes.Status428PreconditionRequired;
                    throw new ApplicationException ("Pin not passed");
                }

                 if(string.IsNullOrEmpty(value.Trim()))
                {
                    Response.StatusCode = StatusCodes.Status428PreconditionRequired;
                    throw new ApplicationException ("pin value not passed");
                }

                await CallServiceAsync("PUT", pinName, value);
                var objReturnModel = CallDeviceServiceAsync("SendC2D",pinName,value);
                return AppUtilities.SerializeObjectToJSON(objReturnModel);
            }
            catch (System.Exception ex)
            {
                Response.StatusCode = StatusCodes.Status428PreconditionRequired;
                throw new ApplicationException(ex.Message);
            }
          
        }

        [Route("IsHardwareConnected")]
        public bool IsHardwareConnectedAsync()
        {
            try
            {
                 return CallDeviceServiceIsHardwareConnected("IsHardwareConnected");
            }
            catch (System.Exception ex)
            {
                Response.StatusCode = StatusCodes.Status428PreconditionRequired;
                throw new ApplicationException(ex.Message);
            }
           
        }

        [Route("ReadFromDevice")]
        [HttpGet]
        public List<AdaFruitModel> ReadFromDevice()
        {
            try
            {
                return D2ClDeviceServiceAsync();
            }
            catch (System.Exception ex)
            {
                Response.StatusCode = StatusCodes.Status428PreconditionRequired;
                throw new ApplicationException("Read dvice gateway failed");
            }
           
        }

        private void CallRegisterMicroService()
        {
            GetHttpClient();
            
            HttpResponseMessage response;
            HubDeviceResponse objResponce = new HubDeviceResponse();
           
            try
            {
                response = ClientHttp.GetAsync(AppConfig.RegisterDeviceEndpoint).Result;  

                if (!response.IsSuccessStatusCode)
                {

                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
           
       }

        private string GetTimeZoneTime(string deviceTime)
        {
            DateTime thisTime = DateTime.Parse(deviceTime);
           // Get India Standard Time zone
            TimeZoneInfo tst = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime istTime = TimeZoneInfo.ConvertTime(thisTime, TimeZoneInfo.Local, tst);      
            return istTime.ToShortDateString() +":"+ istTime.ToLongTimeString();
        }

    
        private async System.Threading.Tasks.Task<ReturnModel> CallServiceAsync(string httpVerb,string url=null,string value=null)
        {
            GetHttpClient();

            HttpResponseMessage response;
            ReturnModel objResponce = new ReturnModel();
           
            if(httpVerb=="PUT")
            {
               response = await ClientHttp.GetAsync(AppConfig.ServiceDeviceEndpoint + url + "?value="+ value); 
            }
            else
            {
               response = await ClientHttp.GetAsync(AppConfig.ServiceDeviceEndpoint);
            }
          
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var responseStream = response.Content.ReadAsStreamAsync().Result;

                    if(httpVerb=="GET")
                    {  
                        var serializeOptions = new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase                   
                        };
                   
                        objResponce = JsonSerializer.DeserializeAsync<ReturnModel>(responseStream,serializeOptions).Result;
                    }
               }
                catch 
                {
                    Response.StatusCode = StatusCodes.Status428PreconditionRequired;
                    return objResponce;
                }
            }
            else
            {
                    Response.StatusCode = StatusCodes.Status428PreconditionRequired;
          
            }
            return objResponce;
        }

        private ReturnModel CallDeviceServiceAsync(string method, string pinName, string value)
        {
            GetHttpClient();

            ReturnModel objResponce = new ReturnModel();

            dynamic message = new ExpandoObject();
            message = "{\"" + pinName + "\":\"" + value +"\"}";

            var contentJSON = JsonSerializer.Serialize(message);
            var content = new StringContent(contentJSON, Encoding.UTF8, "application/json");
            var response = ClientHttp.PostAsync(AppConfig.DeviceEndpoint + pinName + "/"+ value+ "/",content).Result; 
          
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var serializeOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase                   
                    };

                    var responseStream = response.Content.ReadAsStreamAsync().Result;
                    objResponce = JsonSerializer.DeserializeAsync<ReturnModel>(responseStream,serializeOptions).Result;
                }
                catch 
                {
                    Response.StatusCode = StatusCodes.Status428PreconditionRequired;
                    return objResponce;
                }
            }
            else
            {
                    Response.StatusCode = StatusCodes.Status428PreconditionRequired;
            }
            return objResponce;
        }


        private List<AdaFruitModel> D2ClDeviceServiceAsync()
        {
            GetHttpClient();
           
            List<AdaFruitModel> objResponce = new List<AdaFruitModel>();

            var response = ClientHttp.GetAsync(AppConfig.DeviceD2CEndpoint).Result; 
          
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var responseStream = response.Content.ReadAsStreamAsync().Result;
                    objResponce = JsonSerializer.DeserializeAsync<List<AdaFruitModel>>(responseStream).Result;
                }
                catch 
                {
                    Response.StatusCode = StatusCodes.Status428PreconditionRequired;
                    return objResponce;
                }
            }
            else
            {
                    Response.StatusCode = StatusCodes.Status428PreconditionRequired;
            }
            return objResponce;
        }

        private bool CallDeviceServiceIsHardwareConnected(string method)
        {
            GetHttpClient();

            HttpResponseMessage response;
            var objResponce = false;
         
            response = ClientHttp.GetAsync(AppConfig.FetchDeviceEndpoint).Result; 
          
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var serializeOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase                   
                    };

                    var responseStream = response.Content.ReadAsStreamAsync().Result;
                    objResponce = JsonSerializer.DeserializeAsync<bool>(responseStream,serializeOptions).Result;
                }
                catch 
                {
                    Response.StatusCode = StatusCodes.Status428PreconditionRequired;
                    return objResponce;
                }
            }
            else
            {
                    Response.StatusCode = StatusCodes.Status428PreconditionRequired;
            }
            return objResponce;
        }

        private HttpClient GetHttpClient()
        {
            
            ClientHttp.DefaultRequestHeaders.Remove(AppConfig.DeviceKey);
            ClientHttp.DefaultRequestHeaders.Remove(AppConfig.ServiceKey);
            
            ClientHttp.DefaultRequestHeaders.Add(AppConfig.DeviceKey,DeviceID);
            ClientHttp.DefaultRequestHeaders.Add(AppConfig.ServiceKey,ServiceKey);
            return ClientHttp;
        }
    }
}
