using System;
using Microsoft.AspNetCore.Mvc;
using RearCop.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Threading;

namespace Device.RearCop.Controllers
{
    [Route("[controller]")]
    public class AdafruitDeviceStatusController : ServiceControllerBase
    {
        //public ILogger<AdafruitC2DController> ErrorLogger { get; set; }
        public AdafruitDeviceStatusController(IOptions<AppConfig> pAppConfig,
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

        [HttpGet]
        [Route("IsHardwareConnected")]
        public bool IsHardwareConnected()
        {
            try
            {
                var devicePayload =new ReturnModel();
                var returnValue=false;
                devicePayload.defaultValue ="IsHardwareConnected";
                AdfHandler.SendC2DPayload(devicePayload, DeviceID, AppConfiguration.MqttURL, AppConfiguration.MqttUserName, AppConfiguration.MqttKey,"IsHardwareConnected");
                Thread.Sleep(500);
                AdaFruitModel obj = AdfHandler.ReadPingFeedLastData(DeviceID, AppConfiguration.MqttURL, AppConfiguration.MqttUserName, AppConfiguration.MqttKey,"rcdeviceping");
                var diffMin =(DateTime.Now.ToUniversalTime() - DateTime.Parse(obj.created_at).ToUniversalTime()).TotalSeconds;
                if(diffMin<5 & obj.value == DeviceID +":IsHardwareConnected")
                {
                    returnValue = true;
                }
                return returnValue;
            }
            catch (Exception ex)
            {
                Response.StatusCode =  StatusCodes.Status413PayloadTooLarge;
                ErrorLogger.LogError(ex, ex.Message);
                throw new ApplicationException(ex.Message);
            }
        }
     }
}
