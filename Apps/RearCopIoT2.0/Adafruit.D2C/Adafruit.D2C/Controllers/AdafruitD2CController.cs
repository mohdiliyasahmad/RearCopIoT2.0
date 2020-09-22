using System;
using Microsoft.AspNetCore.Mvc;
using RearCop.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Device.RearCop.Controllers
{
    [Route("[controller]")]
    public class AdafruitD2CController : ServiceControllerBase
    {
        //public ILogger<DeviceController> ErrorLogger { get; set; }
        public AdafruitD2CController(IOptions<AppConfig> pAppConfig,
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
        [HttpGet]
        [Route("ReadFromDevice")]
        public List<AdaFruitModel> ReadFromDevice()
        {
            var devicePayload = new List<AdaFruitModel>();
            try
            {
                devicePayload = ReadFromDevice(DeviceID);
                return devicePayload;
            }
            catch (Exception ex)
            {
                Response.StatusCode = StatusCodes.Status413PayloadTooLarge;
                ErrorLogger.LogError(ex, ex.Message);
                throw new ApplicationException(ex.Message);
            }
        }

        private List<AdaFruitModel> ReadFromDevice(string deviceId)
        {
            return AdfHandler.ReadFeedData(deviceId, AppConfiguration.MqttURL, AppConfiguration.MqttUserName, AppConfiguration.MqttKey,"d2c");
        }
    }
}
