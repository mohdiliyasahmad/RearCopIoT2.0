using Microsoft.AspNetCore.Builder;

namespace RearCop.Common
{
    public static class ValidateDeviceMiddlewareExtensions
    {
        public static IApplicationBuilder UseValidateDevice(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ValidateDeviceMiddleware>();
        }
    }
}