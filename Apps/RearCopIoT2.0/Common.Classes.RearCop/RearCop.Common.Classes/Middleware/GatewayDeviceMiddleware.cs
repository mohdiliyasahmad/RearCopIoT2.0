using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace RearCop.Common
{
    public class GatewayDeviceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HttpClient httpClient;

        private readonly GatewayConfig config;
        public GatewayDeviceMiddleware(RequestDelegate next,
            IHttpClientFactory httpClientFactory,
            IOptions<GatewayConfig> pConfig)
        {
            _next = next;
            httpClient = httpClientFactory.CreateClient();
            config =(GatewayConfig)pConfig.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
                var deviceToken = string.Empty;
                var actionUdateToken = string.Empty;
                var path = context.Request.Path.Value;
                var apiKey = string.Empty;
                
                if (context.Request.QueryString.HasValue)
                {
                    apiKey = context.Request.Query[config.ServiceKey].ToString();
                }
                
                if(apiKey == string.Empty)
                {
                 context.Response.StatusCode =  StatusCodes.Status401Unauthorized;
                 await context.Response.WriteAsync("API Key not detected");
                 return;
                }  

                if(apiKey != config.APIKey)
                {
                 context.Response.StatusCode =  StatusCodes.Status401Unauthorized;
                 await context.Response.WriteAsync("API Key does not match");
                 return;
                }  
              
                if (path.Split('/').Length > 2)
                {
                    deviceToken = path.Split('/')[1];
                    for (int i = 2; i < path.Split('/').Length; i++)
                    {
                       actionUdateToken = actionUdateToken + "/"+ path.Split('/')[i];
                    }
                    context.Request.Headers.Remove(config.DeviceKey);
                    context.Request.Headers.Remove(config.ServiceKey);
                    context.Request.Headers.Add(config.DeviceKey, deviceToken);
                    context.Request.Headers.Add(config.ServiceKey, apiKey);
                    context.Request.Path = actionUdateToken;
                }

            await _next(context);
        }
 
    }

 }