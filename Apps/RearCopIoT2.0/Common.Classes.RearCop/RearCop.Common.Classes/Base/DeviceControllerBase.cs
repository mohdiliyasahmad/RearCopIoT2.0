using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;

namespace RearCop.Common
{
    public class DeviceControllerBase : ControllerBase, IActionFilter
    {
        public string DeviceID { get; set; }
        public string PinName { get; set; }
        public string PinValue { get; set; }
        public string ReturnResult { get; set; }
        public HttpClient AppHttpClient { get; set; }
        public ILogger<ControllerBase> ErrorLogger { get; set; }
        public AppConfig AppConfiguration { get; set; }
        public FirebaseHandler FBHandler { get; set; }
        public AzureHandler AzureAppHandler { get; set; }
        public Utilitities AppUtilities { get; set; }
                
        public DeviceControllerBase(IOptions<AppConfig> pAppConfig,
        IHttpClientFactory pclientFactory,
        ILogger<DeviceControllerBase> errorLogger,
        FirebaseHandler pFirebaseHandler,
        AzureHandler pAzureHandler,
        Utilitities pAppUtilities)
        {
            AppConfiguration =(AppConfig)pAppConfig.Value;
            AppHttpClient =pclientFactory.CreateClient();
            ErrorLogger= errorLogger;
            FBHandler = pFirebaseHandler;
            AzureAppHandler =pAzureHandler;
            AppUtilities =pAppUtilities;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            DetectDeviceId();
            Response.StatusCode = StatusCodes.Status200OK;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }
        private void DetectDeviceId()
        {
            ReturnResult = string.Empty;

            DeviceID = HttpContext.Request.Headers["DeviceID"];

            if (string.IsNullOrEmpty(DeviceID))
            {
                ReturnResult = "device id not found";
            }
        }
    }
}
