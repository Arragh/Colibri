namespace Colibri.Configuration;

public static class ConfigValidator
{
    public static bool Validate(ColibriSettings settings)
    {
        var routingValidateResult = ValidateRouting(settings.Routing);
        var authorizationValidateResult = ValidateAuthorization(settings.Authorization);

        if (!routingValidateResult)
        {
            return false;
        }
        
        foreach (var cluster in settings.Routing.Clusters)
        {
            if (cluster.Authorize != null && cluster.Authorize.Required)
            {
                if (!authorizationValidateResult)
                {
                    return false;
                }
                    
                var policy = settings.Authorization.Policies
                    .FirstOrDefault(p => p.PolicyId == cluster.Authorize.PolicyId);

                if (policy == null)
                {
                    return false;
                }
            }
        }
        
        return true;
    }

    private static bool ValidateRouting(RoutingSettings routingSettings)
    {
        if (routingSettings == null)
        {
            return false;
        }

        if (routingSettings.Clusters == null || routingSettings.Clusters.Length == 0)
        {
            return false;
        }

        foreach (var cluster in routingSettings.Clusters)
        {
            if (cluster.Hosts == null || cluster.Hosts.Length == 0)
            {
                return false;
            }

            foreach (var host in cluster.Hosts)
            {
                if (string.IsNullOrWhiteSpace(host))
                {
                    return false;
                }
            }

            if (cluster.Routes == null || cluster.Routes.Length == 0)
            {
                return false;
            }

            foreach (var route in cluster.Routes)
            {
                if (string.IsNullOrWhiteSpace(route.Method))
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(route.UpstreamPattern))
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(route.DownstreamPattern))
                {
                    return false;
                }
            }
            
        }

        return true;
    }

    private static bool ValidateAuthorization(AuthorizationSettings authSettings)
    {
        if (authSettings is null)
        {
            return false;
        }

        if (authSettings.Policies == null || authSettings.Policies.Length == 0)
        {
            return false;
        }

        foreach (var policy in authSettings.Policies)
        {
            if (string.IsNullOrWhiteSpace(policy.PolicyId))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(policy.TokenKey))
            {
                return false;
            }
        }

        return true;
    }
}