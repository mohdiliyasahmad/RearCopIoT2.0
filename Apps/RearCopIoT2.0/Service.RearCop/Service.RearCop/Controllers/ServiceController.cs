using System;
using Microsoft.AspNetCore.Mvc;
using RearCop.Common;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace Service.RearCop.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServiceController : ServiceControllerBase
    {
        //public ILogger<ServiceController> ErrorLogger { get; set; }
        public ServiceController(IOptions<AppConfig> pAppConfig,
        IHttpClientFactory pclientFactory,
        ILogger<ServiceControllerBase> errorLogger,
        FirebaseHandler pFirebaseHandler,
        AzureHandler pAzureHandler,
        AdafruitHandler pAdafruitHandler,
        Utilitities pAppUtilities) 
        : base (pAppConfig,pclientFactory,errorLogger,pFirebaseHandler,pAzureHandler, pAdafruitHandler, pAppUtilities) { }

       // GET api/values
        [HttpGet]
        public ReturnModel Get()
        {
            var returnValue = new ReturnModel();
            try
            {
                return  CookDataBeforeSend(DeviceID);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, ex.Message);
                return returnValue;
            }
        }

        // GET /values/V0
        [Route("Get")]
        [HttpGet("{pinName}")]
        public async System.Threading.Tasks.Task<string> GetAsync(string pinName)
        {
            var value = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(pinName))
                {
                    Response.StatusCode =  StatusCodes.Status417ExpectationFailed;
                    throw new ApplicationException ("Pin not passed");
                }

                value = Request.Query["Value"].ToString();

                if (string.IsNullOrWhiteSpace(value.Trim()))
                {
                    Response.StatusCode =  StatusCodes.Status417ExpectationFailed;
                    throw new ApplicationException ("Pin value not passed");
                }
             
                var objJSON = JsonSerializer.Serialize(value);
                ReturnResult = await PutDataAsync(AppConfiguration.DatabaseServerUri +"/Devices/"+ DeviceID + "/Transaction/" + pinName + AppConfiguration.DatabaseServerUriPostFix, objJSON, null);
            
                return ReturnResult;
            }
            catch (Exception ex)
            {
                Response.StatusCode =  StatusCodes.Status429TooManyRequests;
                ErrorLogger.LogError(ex, ex.Message);
                throw new ApplicationException(ex.Message);
            }
        }
       
        private async System.Threading.Tasks.Task<string> GetDataAsync(string url, string accessToken)
        {
            string result = string.Empty;
            try
            {
                return result = await FBHandler.GetDataAsync(url, accessToken,AppHttpClient);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, ex.Message);
                return result + ex.Message;
            }
        }
        private ReturnModel CookDataBeforeSend(string deviceID)
        {
            var result = FBHandler.GetDeviceData(deviceID,
                AppConfiguration.ServerURL, 
                AppConfiguration.DatabaseServerUri, 
                AppConfiguration.DatabaseServerUriPostFix,
                AppHttpClient);

                //result = FBHandler.CookDataBeforeSend(result);
                return result;
        }
        private async System.Threading.Tasks.Task<string> PutDataAsync
        (string url, string jsonString, string accessToken)
        {
            string result = string.Empty;
            try
            {
                await FBHandler.PutDataAsync(url, jsonString, AppHttpClient, accessToken);
                return result = jsonString;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex, ex.Message);
                return result;
            }
        }

    }
}
