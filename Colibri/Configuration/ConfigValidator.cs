using Colibri.Configuration.Models;
using Microsoft.Extensions.Options;

namespace Colibri.Configuration;

public static class ConfigValidator
{
    public static bool Validate(ColibriSettings settings)
    {
        var errors = new List<string>();
        
        ValidateClusters(settings.Routing.Clusters, errors);
        ValidateRoutes(settings.Routing.Routes, errors);
        
        return true;
    }

    private static void ValidateClusters(ClusterCfg[] clusters, List<string> errors)
    {
        if (clusters.Length > ushort.MaxValue)
        {
            errors.Add($"Too many clusters: {clusters.Length}");
        }
    }

    private static void ValidateRoutes(RouteCfg[] routes, List<string> errors)
    {
        foreach (var route in routes)
        {
            ValidateStaticSegments(route, errors);
            ValidateParams(route, errors);
        }

        if (errors.Count > 0)
        {
            throw new OptionsValidationException(
                nameof(RoutingSettings),
                typeof(RoutingSettings),
                errors.ToArray());
        }
    }

    private static void ValidateStaticSegments(RouteCfg route, List<string> errors)
    {
        var upstreamSegments = route.UpstreamPattern
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Where(s => !s.StartsWith('{') && !s.EndsWith('}'));
        
        var downstreamSegments = route.DownstreamPattern
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Where(s => !s.StartsWith('{') && !s.EndsWith('}'));

        foreach (var segment in upstreamSegments)
        {
            if (segment.Length > 250)
            {
                errors.Add($"Segment '{segment}' in route {route.UpstreamPattern} is longer than 250 characters");
            }
        }
        
        foreach (var segment in downstreamSegments)
        {
            if (segment.Length > 250)
            {
                errors.Add($"Segment '{segment}' in route {route.DownstreamPattern} is longer than 250 characters");
            }
        }
    }

    private static void ValidateParams(RouteCfg route, List<string> errors)
    {
        var upstreamSegments = route.UpstreamPattern
            .Split('/', StringSplitOptions.RemoveEmptyEntries);
        
        var downstreamSegments = route.DownstreamPattern
            .Split('/', StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < upstreamSegments.Length; ++i)
        {
            if ((upstreamSegments[i].StartsWith('{') && !upstreamSegments[i].EndsWith('}'))
                || (upstreamSegments[i].EndsWith('}') && !upstreamSegments[i].StartsWith('{')))
            {
                errors.Add($"'{upstreamSegments[i]}' is not a valid route segment in '{route.UpstreamPattern}'");
            }
        }
        
        var upstreamParams = upstreamSegments
            .Where(s => s.StartsWith('{') && s.EndsWith('}'))
            .ToArray();
        
        var downstreamParams = downstreamSegments
            .Where(s => s.StartsWith('{') && s.EndsWith('}'))
            .ToArray();
        
        if (upstreamParams.Length > 16)
        {
            errors.Add($"Too many parameters ({upstreamParams.Length}) in route '{route.UpstreamPattern}'");
        }

        if (downstreamParams.Length > 16)
        {
            errors.Add($"Too many parameters ({downstreamParams.Length}) in route '{route.DownstreamPattern}'");
        }

        if (upstreamParams.Length != downstreamParams.Length)
        {
            errors.Add($"Different number of parameters in route {route.UpstreamPattern}");
        }

        for (int i = 0; i < upstreamParams.Length; ++i)
        {
            for (int j = i + 1; j < upstreamParams.Length; ++j)
            {
                if (upstreamParams[i] == upstreamParams[j])
                {
                    errors.Add($"Duplicate parameter {upstreamParams[i]} in route {route.UpstreamPattern}");
                }

                if (downstreamParams[i] == downstreamParams[j])
                {
                    Console.WriteLine($"Duplicate parameter {downstreamParams[i]} in route {route.DownstreamPattern}");
                    errors.Add($"Duplicate parameter {downstreamParams[i]} in route {route.DownstreamPattern}");
                }
            }
        }
    }
}