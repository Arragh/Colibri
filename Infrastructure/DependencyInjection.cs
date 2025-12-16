using Infrastructure.Configuration;
using Infrastructure.Services.Http;
using Interfaces.Services.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static void AddColibriInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IHttpTransportProvider, HttpTransportProvider>();
    }
    
    public static void AddColibriConfiguration(this IServiceCollection services)
    {
        services
            .AddOptions<ClusterSetting>()
            .BindConfiguration("ClusterSetting");
    }
}