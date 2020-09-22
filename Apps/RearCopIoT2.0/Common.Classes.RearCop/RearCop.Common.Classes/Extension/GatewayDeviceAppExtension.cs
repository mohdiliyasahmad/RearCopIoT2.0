using Microsoft.AspNetCore.Builder;

namespace RearCop.Common
{
    public static class GatewayMiddlewareExtensions
    {
        public static IApplicationBuilder UseDevice(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GatewayDeviceMiddleware>();
        }
    }
}