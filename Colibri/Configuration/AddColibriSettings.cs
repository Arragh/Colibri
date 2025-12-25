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
                        !string.IsNullOrWhiteSpace(cluster.Prefix)
                        && cluster.BaseUrls is { Length: > 0 }
                        && cluster.BaseUrls.All(url => !string.IsNullOrWhiteSpace(url))
                        && cluster.Endpoints is { Length: > 0 }
                        && cluster.Endpoints.All(endpoint =>
                            !string.IsNullOrWhiteSpace(endpoint.Method)
                            && !string.IsNullOrWhiteSpace(endpoint.Downstream)
                            && !string.IsNullOrWhiteSpace(endpoint.Upstream))),
                "Invalid ColibriSettings configuration")
            .ValidateOnStart();

        services
            .AddOptions<TestSettings>()
            .BindConfiguration("ColibriSettings:Test");
    }
}