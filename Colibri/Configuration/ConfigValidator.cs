namespace Colibri.Configuration;

public static class ConfigValidator
{
    public static bool Validate(RoutingSettings settings)
    {
        return settings.Clusters is { Length: > 0 }
            && settings.Clusters.All(cluster =>
                cluster.Hosts is { Length: > 0 }
                && cluster.Hosts.All(url => !string.IsNullOrWhiteSpace(url))
                && cluster.Routes is { Length: > 0 }
                && cluster.Routes.All(route =>
                    !string.IsNullOrWhiteSpace(route.Method)
                    && !string.IsNullOrWhiteSpace(route.UpstreamPattern)
                    && !string.IsNullOrWhiteSpace(route.DownstreamPattern)));
    }
}