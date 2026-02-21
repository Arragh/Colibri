using Colibri.Configuration.Models;
using Microsoft.Extensions.Options;

namespace Colibri.Configuration;

public static class ConfigValidator
{
    private const int SegmentLenght = 250;
    private static readonly HashSet<string> ValidHttpMethods = [ "GET", "POST", "PUT", "PATCH", "DELETE", "HEAD", "OPTIONS", "TRACE" ];

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

        foreach (var cluster in clusters)
        {
            if (cluster.Prefix.Length > SegmentLenght)
            {
                errors.Add($"Cluster prefix '{cluster.Prefix}' is longer than {SegmentLenght} characters");
            }
        }
    }

    private static void ValidateRoutes(RouteCfg[] routes, List<string> errors)
    {
        foreach (var route in routes)
        {
            ValidateStaticSegments(route, errors);
            ValidateParams(route, errors);
            ValidateMethods(route, errors);
        }
        
        CheckForDuplicates(routes, errors);

        if (errors.Count > 0)
        {
            throw new OptionsValidationException(
                nameof(RoutingSettings),
                typeof(RoutingSettings),
                errors.ToArray());
        }

        return;

        void ValidateStaticSegments(RouteCfg route, List<string> errors)
        {
            var upstreamSegments = route.UpstreamPattern
                .Split('/', StringSplitOptions.RemoveEmptyEntries)
                .Where(s => !s.StartsWith('{') && !s.EndsWith('}'));
            
            var downstreamSegments = route.DownstreamPattern
                .Split('/', StringSplitOptions.RemoveEmptyEntries)
                .Where(s => !s.StartsWith('{') && !s.EndsWith('}'));

            foreach (var segment in upstreamSegments)
            {
                if (segment.Length > SegmentLenght)
                {
                    errors.Add($"Segment '{segment}' in upstream {route.UpstreamPattern} is longer than {SegmentLenght} characters");
                }
            }
            
            foreach (var segment in downstreamSegments)
            {
                if (segment.Length > SegmentLenght)
                {
                    errors.Add($"Segment '{segment}' in downstream {route.DownstreamPattern} is longer than {SegmentLenght} characters");
                }
            }
        }

        void ValidateParams(RouteCfg route, List<string> errors)
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
                    errors.Add($"'{upstreamSegments[i]}' is not a valid route segment in upstream '{route.UpstreamPattern}'");
                }
            }
            
            var upstreamParams = upstreamSegments
                .Where(s => s.StartsWith('{') && s.EndsWith('}'))
                .ToArray();
            
            var downstreamParams = downstreamSegments
                .Where(s => s.StartsWith('{') && s.EndsWith('}'))
                .ToArray();

            foreach (var param in upstreamParams)
            {
                if (param.Length > SegmentLenght)
                {
                    errors.Add($"Parameter '{param}' in upstream {route.UpstreamPattern} is longer than {SegmentLenght} characters");
                }
            }

            foreach (var param in downstreamParams)
            {
                if (param.Length > SegmentLenght)
                {
                    errors.Add($"Parameter '{param}' in downstream {route.DownstreamPattern} is longer than {SegmentLenght} characters");
                }
            }
            
            if (upstreamParams.Length > 16)
            {
                errors.Add($"Too many parameters ({upstreamParams.Length}) in upstream '{route.UpstreamPattern}'");
            }

            if (downstreamParams.Length > 16)
            {
                errors.Add($"Too many parameters ({downstreamParams.Length}) in downstream '{route.DownstreamPattern}'");
            }

            if (upstreamParams.Length != downstreamParams.Length)
            {
                errors.Add($"Different number of parameters in upstream {route.UpstreamPattern}");
            }

            for (int i = 0; i < upstreamParams.Length; ++i)
            {
                for (int j = i + 1; j < upstreamParams.Length; ++j)
                {
                    if (upstreamParams[i] == upstreamParams[j])
                    {
                        errors.Add($"Duplicate parameter {upstreamParams[i]} in upstream {route.UpstreamPattern}");
                    }

                    if (downstreamParams[i] == downstreamParams[j])
                    {
                        errors.Add($"Duplicate parameter {downstreamParams[i]} in downstream {route.DownstreamPattern}");
                    }
                }
            }
        }

        void ValidateMethods(RouteCfg route, List<string> errors)
        {
            foreach (var method in route.Methods)
            {
                if (!ValidHttpMethods.Contains(method))
                {
                    var upper = method.ToUpperInvariant();

                    if (ValidHttpMethods.Contains(upper))
                    {
                        errors.Add($"HTTP method '{method}' in upstream '{route.UpstreamPattern}' must be uppercase");
                    }
                    else
                    {
                        errors.Add($"HTTP method '{method}' is not a valid method in upstream '{route.UpstreamPattern}'");
                    }
                }
            }
        }
        
        void CheckForDuplicates(RouteCfg[] routes, List<string> errors)
        {
            for (int i = 0; i < routes.Length; ++i)
            {
                var route1 = routes[i];

                for (int j = i + 1; j < routes.Length; j++)
                {
                    var route2 = routes[j];
                    
                    if (route1.UpstreamPattern == route2.UpstreamPattern)
                    {
                        var duplicates = route1.Methods
                            .Intersect(route2.Methods, StringComparer.OrdinalIgnoreCase)
                            .ToArray();

                        foreach (var method in duplicates)
                        {
                            errors.Add($"Duplicate method {method} in upstream {route1.UpstreamPattern}");
                        }
                    }
                }
            }
        }
    }
}