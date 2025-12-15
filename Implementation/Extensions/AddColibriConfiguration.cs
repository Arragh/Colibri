using Implementation.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Implementation.Extensions;

public static class AddColibriConfigurationExtension
{
    public static void AddColibriConfiguration(this IServiceCollection services)
    {
        services
            .AddOptions<ClusterSetting>()
            .BindConfiguration("ClusterSetting");
    }
}