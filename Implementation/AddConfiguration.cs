using Implementation.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Implementation;

public static class AddConfigurationExtension
{
    public static void AddConfiguration(this IServiceCollection services)
    {
        services.AddOptions<EndpointsSettings>().BindConfiguration("Configuration");
    }
}