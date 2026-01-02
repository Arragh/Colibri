namespace Colibri.Configuration;

internal static class AddColibriSettingsExtension
{
    public static void AddColibriSettings(this IServiceCollection services)
    {
        services
            .AddOptions<RoutingSettings>()
            .BindConfiguration("ColibriSettings:Routing")
            .Validate(settings =>
                    settings.Clusters is { Length: > 0 }
                    && settings.Clusters.All(cluster =>
                        cluster.Hosts is { Length: > 0 }
                        && cluster.Hosts.All(url => !string.IsNullOrWhiteSpace(url))
                        && cluster.Routes is { Length: > 0 }
                        && cluster.Routes.All(route =>
                            !string.IsNullOrWhiteSpace(route.Method)
                            && !string.IsNullOrWhiteSpace(route.UpstreamPattern)
                            && !string.IsNullOrWhiteSpace(route.DownstreamPattern))),
                "Invalid ColibriSettings configuration")
            .ValidateOnStart();
    }
}