using System.Text.RegularExpressions;
using Colibri.Configuration.Models;
using Microsoft.Extensions.Options;

namespace Colibri.Configuration;

public static class ConfigValidator
{
    private const int SegmentMaxLength = 250;
    private static readonly HashSet<string> ValidHttpMethods = [ "GET", "POST", "PUT", "PATCH", "DELETE", "HEAD", "OPTIONS", "TRACE" ];

    /// <summary>
    /// Validates the Colibri configuration. Throws if invalid.
    /// </summary>
    public static bool Validate(ColibriSettings settings)
    {
        var errors = new List<string>();
        
        ValidateClusters(
            settings.Routing.Clusters,
            errors);
        
        ValidateRoutes(
            settings.Routing.Clusters,
            settings.Routing.Routes,
            errors);
        
        return true;
    }

    private static void ValidateClusters(
        ClusterCfg[] clusters,
        List<string> errors)
    {
        ValidateClustersCount();

        foreach (var cluster in clusters)
        {
            ValidateClusterIdIsNotEmpty(cluster);
            ValidatePrefixLength(cluster);
            ValidatePrefixName(cluster);
        }
        
        ValidateClusterIdIsUnique();

        void ValidateClustersCount()
        {
            if (clusters.Length > ushort.MaxValue)
            {
                errors.Add($"Too many clusters: {clusters.Length}");
            }
        }

        void ValidateClusterIdIsNotEmpty(ClusterCfg cluster)
        {
            if (string.IsNullOrWhiteSpace(cluster.ClusterId))
            {
                errors.Add($"ClusterId can't be empty");
            }
        }
        
        void ValidateClusterIdIsUnique()
        {
            for (int i = 0; i < clusters.Length; i++)
            {
                for (int j = i + 1; j < clusters.Length; j++)
                {
                    if (clusters[i].ClusterId.Equals(clusters[j].ClusterId))
                    {
                        errors.Add($"ClusterId '{clusters[i].ClusterId}' must be unique");
                    }
                }
            }
        }
        
        void ValidatePrefixLength(ClusterCfg cluster)
        {
            if (cluster.Prefix.Length > SegmentMaxLength)
            {
                errors.Add($"Cluster prefix '{cluster.Prefix}' is longer than {SegmentMaxLength} characters");
            }
        }

        void ValidatePrefixName(ClusterCfg cluster)
        {
            var prefix = cluster.Prefix;
            
            if (prefix[0] == '/')
            {
                prefix =  prefix[1..];
            }
            
            if (!SegmentOrPrefixNameIsValid(prefix))
            {
                errors.Add($"Cluster prefix '{prefix}' is invalid");
            }
        }
    }

    private static void ValidateRoutes(
        ClusterCfg[] clusters,
        RouteCfg[] routes,
        List<string> errors)
    {
        foreach (var route in routes)
        {
            ValidateRouteClusterId(route);
            ValidateRoutePathsFormat(route);
            ValidateSegmentsNotEmpty(route);
            ValidateStaticSegmentsLength(route);
            ValidateStaticSegmentNames(route);
            ValidateParamsPatternCurlyBraces(route);
            ValidateParamsNotEmpty(route);
            ValidateParamsPatternLength(route);
            ValidateParamName(route);
            ValidateParamsCount(route);
            ValidateParamsCountEqual(route);
            CheckForParamsDuplicates(route);
            ValidateHttpMethods(route);
        }
        
        CheckForMethodsDuplicates();

        if (errors.Count > 0)
        {
            throw new OptionsValidationException(
                nameof(RoutingSettings),
                typeof(RoutingSettings),
                errors.ToArray());
        }

        void ValidateRouteClusterId(RouteCfg route)
        {
            if (clusters.All(c => c.ClusterId != route.ClusterId))
            {
                errors.Add($"There is no cluster with ClusterId '{route.ClusterId}'");
            }
        }

        void ValidateRoutePathsFormat(RouteCfg route)
        {
            if (!route.UpstreamPattern.StartsWith('/'))
            {
                errors.Add($"Upstream '{route.UpstreamPattern}' must start with '/'");
            }

            if (!route.DownstreamPattern.StartsWith('/'))
            {
                errors.Add($"Downstream '{route.DownstreamPattern}' must start with '/'");
            }

            if (route.UpstreamPattern.Length > 1
                && route.UpstreamPattern.EndsWith('/'))
            {
                errors.Add($"Upstream '{route.UpstreamPattern}' must not end with '/'");
            }

            if (route.DownstreamPattern.Length > 1
                && route.DownstreamPattern.EndsWith('/'))
            {
                errors.Add($"Downstream '{route.DownstreamPattern}' must not end with '/'");
            }
        }
        
        void ValidateSegmentsNotEmpty(RouteCfg route)
        {
            var emptyUpstreamSegments = ExtractEmptySegments(route.UpstreamPattern);

            if (emptyUpstreamSegments.Length > 0)
            {
                errors.Add($"Upstream '{route.UpstreamPattern}' has empty segments in route");
            }
        
            var emptyDownstreamSegments = ExtractEmptySegments(route.DownstreamPattern);

            if (emptyDownstreamSegments.Length > 0)
            {
                errors.Add($"Downstream '{route.DownstreamPattern}' has empty segments in route");
            }

            string[] ExtractEmptySegments(string pattern)
            {
                var localPattern = pattern;

                if (localPattern[0] == '/')
                {
                    localPattern = localPattern[1..];
                }
                
                return localPattern
                    .Split('/')
                    .Where(s => string.IsNullOrWhiteSpace(s))
                    .ToArray();
            }
        }

        void ValidateStaticSegmentsLength(RouteCfg route)
        {
            var upstreamStaticSegments = ExtractStaticSegments(route.UpstreamPattern);
            foreach (var segment in upstreamStaticSegments)
            {
                if (segment.Length > SegmentMaxLength)
                {
                    errors.Add($"Segment '{segment}' in upstream {route.UpstreamPattern} is longer than {SegmentMaxLength} characters");
                }
            }
            
            var downstreamStaticSegments = ExtractStaticSegments(route.DownstreamPattern);
            foreach (var segment in downstreamStaticSegments)
            {
                if (segment.Length > SegmentMaxLength)
                {
                    errors.Add($"Segment '{segment}' in downstream {route.DownstreamPattern} is longer than {SegmentMaxLength} characters");
                }
            }
        }

        void ValidateStaticSegmentNames(RouteCfg route)
        {
            var upstreamStaticSegments = ExtractStaticSegments(route.UpstreamPattern);
            foreach (var segment in upstreamStaticSegments)
            {
                if (!SegmentOrPrefixNameIsValid(segment))
                {
                    errors.Add($"Segment name '{segment}' in  upstream {route.UpstreamPattern} is invalid");
                }
            }
            
            var downstreamStaticSegments = ExtractStaticSegments(route.DownstreamPattern);
            foreach (var segment in downstreamStaticSegments)
            {
                if (!SegmentOrPrefixNameIsValid(segment))
                {
                    errors.Add($"Segment name '{segment}' in downstream {route.DownstreamPattern} is invalid");
                }
            }
        }
        
        void ValidateParamsPatternCurlyBraces(RouteCfg route)
        {
            var upstreamSegments = route.UpstreamPattern
                .Split('/', StringSplitOptions.RemoveEmptyEntries);
            
            for (int i = 0; i < upstreamSegments.Length; ++i)
            {
                if ((upstreamSegments[i].StartsWith('{') && !upstreamSegments[i].EndsWith('}'))
                    || (upstreamSegments[i].EndsWith('}') && !upstreamSegments[i].StartsWith('{')))
                {
                    errors.Add($"'{upstreamSegments[i]}' is not a valid route segment in upstream '{route.UpstreamPattern}'");
                }
            }
            
            var downstreamSegments = route.DownstreamPattern
                .Split('/', StringSplitOptions.RemoveEmptyEntries);
            
            for (int i = 0; i < downstreamSegments.Length; ++i)
            {
                if ((downstreamSegments[i].StartsWith('{') && !downstreamSegments[i].EndsWith('}'))
                    || (downstreamSegments[i].EndsWith('}') && !downstreamSegments[i].StartsWith('{')))
                {
                    errors.Add($"'{downstreamSegments[i]}' is not a valid route segment in downstream '{route.DownstreamPattern}'");
                }
            }
        }

        void ValidateParamsNotEmpty(RouteCfg route)
        {
            var upstreamParams = ExtractParamSegments(route.UpstreamPattern)
                .Where(p => string.IsNullOrWhiteSpace(p[1..^1]))
                .ToArray();

            if (upstreamParams.Length > 0)
            {
                errors.Add($"Upstream '{route.UpstreamPattern}' has an empty params");
            }
            
            var downstreamParams = ExtractParamSegments(route.DownstreamPattern)
                .Where(p => string.IsNullOrWhiteSpace(p[1..^1]))
                .ToArray();

            if (downstreamParams.Length > 0)
            {
                errors.Add($"Downstream '{route.DownstreamPattern}' has an empty params");
            }
        }
        
        void ValidateParamsPatternLength(RouteCfg route)
        {
            var upstreamParams = ExtractParamSegments(route.UpstreamPattern);
            
            foreach (var param in upstreamParams)
            {
                if (param.Length > SegmentMaxLength)
                {
                    errors.Add($"Parameter '{param}' in upstream {route.UpstreamPattern} is longer than {SegmentMaxLength} characters");
                }
            }
            
            var downstreamParams = ExtractParamSegments(route.DownstreamPattern);

            foreach (var param in downstreamParams)
            {
                if (param.Length > SegmentMaxLength)
                {
                    errors.Add($"Parameter '{param}' in downstream {route.DownstreamPattern} is longer than {SegmentMaxLength} characters");
                }
            }
        }

        void ValidateParamName(RouteCfg route)
        {
            var upstreamParams = ExtractParamSegments(route.UpstreamPattern);

            foreach (var param in upstreamParams)
            {
                if (!SegmentOrPrefixNameIsValid(param[1..^1]))
                {
                    errors.Add($"Route parameter '{param}' in upstream {route.UpstreamPattern} is invalid");
                }
            }
            
            var  downstreamParams = ExtractParamSegments(route.DownstreamPattern);

            foreach (var param in downstreamParams)
            {
                if (!SegmentOrPrefixNameIsValid(param[1..^1]))
                {
                    errors.Add($"Route parameter '{param}' in downstream {route.DownstreamPattern} is invalid");
                }
            }
        }
        
        void ValidateParamsCount(RouteCfg route)
        {
            var upstreamParams = ExtractParamSegments(route.UpstreamPattern);
            
            if (upstreamParams.Length > 16)
            {
                errors.Add($"Too many parameters ({upstreamParams.Length}) in upstream '{route.UpstreamPattern}'");
            }
        
            var downstreamParams = ExtractParamSegments(route.DownstreamPattern);
            
            if (downstreamParams.Length > 16)
            {
                errors.Add($"Too many parameters ({downstreamParams.Length}) in downstream '{route.DownstreamPattern}'");
            }
        }

        void ValidateParamsCountEqual(RouteCfg route)
        {
            var upstreamParams = ExtractParamSegments(route.UpstreamPattern);
            var downstreamParams = ExtractParamSegments(route.DownstreamPattern);
            
            if (upstreamParams.Length != downstreamParams.Length)
            {
                errors.Add($"Parameters count mismatch between {route.UpstreamPattern} and {route.DownstreamPattern}");
            }
        }

        void CheckForParamsDuplicates(RouteCfg route)
        {
            CheckForDuplicates(route.UpstreamPattern, "upstream");
            CheckForDuplicates(route.DownstreamPattern, "downstream");

            void CheckForDuplicates(string pattern, string patternType)
            {
                var paramsArray = ExtractParamSegments(pattern);
                
                for (int i = 0; i < paramsArray.Length; ++i)
                {
                    for (int j = i + 1; j < paramsArray.Length; ++j)
                    {
                        if (paramsArray[i] == paramsArray[j])
                        {
                            errors.Add($"Duplicate parameter {paramsArray[i]} in {patternType} {pattern}");
                        }
                    }
                }
            }
        }
        
        void ValidateHttpMethods(RouteCfg route)
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
        
        void CheckForMethodsDuplicates()
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
                            errors.Add($"Duplicate method '{method}' in upstream '{route1.UpstreamPattern}'");
                        }
                    }
                }
            }
        }

        string[] ExtractStaticSegments(string pattern)
        {
            return pattern
                .Split('/', StringSplitOptions.RemoveEmptyEntries)
                .Where(s => !s.StartsWith('{') && !s.EndsWith('}'))
                .ToArray();
        }
        
        string[] ExtractParamSegments(string pattern)
        {
            return pattern
                .Split('/', StringSplitOptions.RemoveEmptyEntries)
                .Where(s => s.StartsWith('{') && s.EndsWith('}'))
                .ToArray();
        }
    }

    private static bool SegmentOrPrefixNameIsValid(string name)
    {
        var match = Regex.Match(name, "^[a-zA-Z0-9_]+$");
        return match.Success;
    }
}