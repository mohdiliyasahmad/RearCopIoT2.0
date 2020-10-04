using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;

namespace RearCop.Common
{
    public class GatewayControllerBase : ControllerBase, IActionFilter
    {
        public string ServiceKey { get; set; }
        public string DeviceID { get; set; }
        public string PinName { get; set; }
        public string PinValue { get; set; }
        public string ReturnResult { get; set; }
        public IHttpClientFactory HttpClients { get; set; }
        public ILogger<GatewayControllerBase> ErrorLogger { get; set; }
        public GatewayConfig AppConfig { get; set; }
        public AppConfig AppConfiguration { get; set; }
        public HttpClient ClientHttp {get;set;}
        public FirebaseHandler FBHandler{get;set;}
        public Utilitities AppUtilities { get; set; }
       
        public GatewayControllerBase(IHttpClientFactory pclientFactory,
        ILogger<GatewayControllerBase> errorLogger,
        IOptions<GatewayConfig> pConfig,
        IOptions<AppConfig> pAppConfig,
        FirebaseHandler pFirebaseHandler,
        Utilitities pAppUtilities)
        {
            HttpClients =pclientFactory;
            ErrorLogger= errorLogger;
            AppConfig = (GatewayConfig)pConfig.Value;
            ClientHttp = HttpClients.CreateClient();
            AppConfiguration = pAppConfig.Value;
            FBHandler = pFirebaseHandler;
            AppUtilities =pAppUtilities;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            DetectDeviceId();
            Response.StatusCode = StatusCodes.Status200OK;
            FBHandler.ValidateDeviceAsync(DeviceID);
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }
        private void DetectDeviceId()
        {
            ReturnResult = string.Empty;
            Response.StatusCode = StatusCodes.Status200OK;

            DeviceID = HttpContext.Request.Headers["deviceid"];
            ServiceKey = HttpContext.Request.Headers["apikey"];

            if (string.IsNullOrEmpty(DeviceID))
            {
                ReturnResult = "device id not found";
            }
        }
    }
}
