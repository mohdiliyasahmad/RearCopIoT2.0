using System;
using Microsoft.AspNetCore.Mvc;
using RearCop.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using System.Dynamic;

namespace Device.RearCop.Controllers
{
    [Route("[controller]")]
    public class AdafruitRegisterController : ServiceControllerBase
    {
    
        public AdafruitRegisterController(IOptions<AppConfig> pAppConfig,
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
        [Route("RegisterDevice")]
        public async Task RegisterDeviceAsync()
        {
            var errorCode=string.Empty;
            try
            {
                if (AdfHandler.RegisterDevice(DeviceID,AppConfiguration.MqttURL,AppConfiguration.MqttUserName,AppConfiguration.MqttKey))
                {
                        var objJSON = JsonSerializer.Serialize("0");
                        var url =AppConfiguration.DatabaseServerUri +"/Devices/"+ DeviceID + "/Transaction/V1" + AppConfiguration.DatabaseServerUriPostFix;
                        FBHandler.RegisterDeviceAsync(DeviceID);
                        await FBHandler.PutDataAsync(url, objJSON, AppHttpClient);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
