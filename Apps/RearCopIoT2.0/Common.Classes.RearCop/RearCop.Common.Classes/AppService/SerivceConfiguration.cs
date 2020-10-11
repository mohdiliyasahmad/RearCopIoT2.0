using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace RearCop.Common
{
    public static class SerivceConfiguration
    {
        public static IServiceCollection ConfigureAppServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddSingleton<AppConfig>(ctx => ctx.GetService<IOptions<AppConfig>>().Value);
            services.AddTransient<FirebaseHandler>();
            services.AddTransient<Utilitities>();
            services.AddTransient<AzureHandler>();
            services.AddTransient<AdafruitHandler>();
            services.AddMemoryCache();
            return services;
        }
    }
}