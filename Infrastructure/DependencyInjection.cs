using Core.Models.Configuration;
using Core.Interfaces.Services.Http;
using Infrastructure.Services.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static void AddColibriInfrastructure(this IServiceCollection services)
    {
        services
            .AddOptions<ClusterSetting>()
            .BindConfiguration("ClusterSetting");
        
        services.AddSingleton<IHttpTransportProvider, HttpTransportProvider>();
    }
}