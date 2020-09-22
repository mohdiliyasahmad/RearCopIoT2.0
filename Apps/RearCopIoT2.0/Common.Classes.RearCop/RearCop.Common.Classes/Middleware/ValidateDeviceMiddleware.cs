using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace RearCop.Common
{
    public class ValidateDeviceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HttpClient httpClient;
        private readonly AppConfig appConfig;
        public ValidateDeviceMiddleware(RequestDelegate next,
                                        IHttpClientFactory httpClientFactory,
                                        IOptions<AppConfig> pAppConfig)
        {
            _next = next;
            httpClient = httpClientFactory.CreateClient();
            appConfig = pAppConfig.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
                var deviceToken = string.Empty;
                var actionUdateToken = string.Empty;
                var path = context.Request.Path.Value;
                var apiKey = string.Empty;
                
                apiKey= context.Request.Headers[appConfig.ServiceKey];

                if (apiKey != appConfig.ServiceAPIKey)
                {
                    context.Response.StatusCode =  StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("not authorized");
                    return;
                }

                deviceToken = context.Request.Headers[appConfig.DeviceKey];

                if (deviceToken !=string.Empty)
                {
                    context.Request.Headers.Remove(appConfig.DeviceKey);
                    context.Request.Headers.Remove(appConfig.ServiceKey);
                    
                    context.Request.Headers.Add(appConfig.ServiceKey, apiKey);
                    context.Request.Headers.Add(appConfig.DeviceKey, deviceToken);
                }

            await _next(context);
        }
    }
 }