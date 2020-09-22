using System;
using Microsoft.AspNetCore.Mvc;
using RearCop.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Device.RearCop.Controllers
{
    [Route("[controller]")]
    public class AdafruitC2DController : ServiceControllerBase
    {
        //public ILogger<AdafruitC2DController> ErrorLogger { get; set; }
        public AdafruitC2DController(IOptions<AppConfig> pAppConfig,
        IHttpClientFactory pclientFactory,
        ILogger<ServiceControllerBase> errorLogger,
        FirebaseHandler pFirebaseHandler,
        AzureHandler pAzureHandler,
        AdafruitHandler pAdafruitHandler,
        Utilitities pAppUtilities) 
        : base (pAppConfig, 
        pclientFactory, 
        errorLogger, 
        pFirebaseHandler,
        pAzureHandler, 
        pAdafruitHandler,
        pAppUtilities) { }

        // GET api/values/isHardwareConnected
        [Route("SendC2D/{pinName}/{value}")]
     
        [HttpPost("{pinName}/{value}")]
        public ReturnModel SendToDevice(string pinName,string value)
        {
            var devicePayload = new ReturnModel();
            try
            {
                devicePayload = PreparePayLoad(DeviceID);
                SendToTDevice(pinName,value);
                return devicePayload;
            }
            catch (Exception ex)
            {
                Response.StatusCode = StatusCodes.Status413PayloadTooLarge;
                ErrorLogger.LogError(ex, ex.Message);
                throw new ApplicationException(ex.Message);
            }
        }

        private void SendToTDevice(string pinName,string value)
        {
            AdfHandler.SendC2DPayload(pinName,value, DeviceID, AppConfiguration.MqttURL, AppConfiguration.MqttUserName, AppConfiguration.MqttKey);
        }
      
        private ReturnModel PreparePayLoad(string deviceID)
        {
            var result = FBHandler.GetDeviceData(deviceID,
                AppConfiguration.ServerURL,
                AppConfiguration.DatabaseServerUri,
                AppConfiguration.DatabaseServerUriPostFix,
                AppHttpClient);
                
            //result = FBHandler.CookDataBeforeSend(result);
            return result;
        }

    }
}
