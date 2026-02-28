using Colibri.Configuration;
using Colibri.Configuration.Models;
using Colibri.Helpers;
using Microsoft.Extensions.Options;

namespace Colibri.Services.ConfigValidator;

public sealed class ValidatorService : IValidateOptions<ColibriSettings>
{
    private readonly GlobalValidator _globalValidator = new();
    
    /// <summary>
    /// Validates the Colibri configuration. Returns fail if invalid.
    /// </summary>
    public ValidateOptionsResult Validate(string? name, ColibriSettings options)
    {
        var validateClustersResult = ValidateClusters(options.Routing.Clusters);
        var validateRoutesResult = ValidateRoutes(options.Routing.Routes);
        var validateCrossResult = ValidateCrossReferences(options.Routing.Clusters, options.Routing.Routes);
        
        if (!validateClustersResult.Succeeded)
        {
            return validateClustersResult;
        }

        if (!validateRoutesResult.Succeeded)
        {
            return validateRoutesResult;
        }

        if (!validateCrossResult.Succeeded)
        {
            return validateCrossResult;
        }

        return ValidateOptionsResult.Success;
    }

    private ValidateOptionsResult ValidateClusters(ClusterCfg[] clusters)
    {
        foreach (var cluster in clusters)
        {
            if (!_globalValidator.Clusters.NameIsNotEmpty(cluster.Name))
            {
                return ValidateOptionsResult
                    .Fail($"Cluster's name '{cluster.Name}' is empty");
            }

            if (!_globalValidator.Clusters.NameLengthIsValid(cluster.Name))
            {
                return ValidateOptionsResult
                    .Fail($"Cluster's name '{cluster.Name}' length is invalid");
            }

            if (!_globalValidator.Clusters.NameIsUnique(cluster, clusters))
            {
                return ValidateOptionsResult
                    .Fail($"Cluster's name '{cluster.Name}' must be unique");
            }

            if (!_globalValidator.Clusters.NameIsValid(cluster.Name))
            {
                return ValidateOptionsResult
                    .Fail($"Cluster's name '{cluster.Name}' is invalid");
            }

            if (!_globalValidator.Clusters.ProtocolIsNotEmpty(cluster.Protocol))
            {
                return ValidateOptionsResult
                    .Fail($"Cluster {cluster.Name} has an empty protocol");
            }

            if (!_globalValidator.Clusters.ProtocolIsValid(cluster.Protocol))
            {
                return ValidateOptionsResult
                    .Fail($"Cluster '{cluster.Name}' has an invalid protocol '{cluster.Protocol}'");
            }

            if (!_globalValidator.Clusters.PrefixIsNotEmpty(cluster.Prefix))
            {
                return ValidateOptionsResult
                    .Fail($"Cluster '{cluster.Name}' has an empty prefix");
            }

            if (!_globalValidator.Clusters.PrefixIsValid(cluster.Prefix))
            {
                return ValidateOptionsResult
                    .Fail($"Cluster '{cluster.Name}' has an invalid prefix {cluster.Prefix}");
            }

            if (!_globalValidator.Clusters.HostsAreNotEmpty(cluster.Hosts))
            {
                return ValidateOptionsResult
                    .Fail($"Cluster's '{cluster.Name}' hosts are empty");
            }

            if (cluster.LoadBalancer?.Enabled == true
                && !_globalValidator.Clusters.LoadBalancerTypeIsNotEmpty(cluster.LoadBalancer?.Type))
            {
                return ValidateOptionsResult
                    .Fail($"Cluster '{cluster.Name}' has an empty load balancer type");
            }

            if (cluster.LoadBalancer?.Enabled == true
                && !_globalValidator.Clusters.LoadBalancerTypeIsValid(cluster.LoadBalancer.Type))
            {
                return ValidateOptionsResult
                    .Fail($"Cluster '{cluster.Name}' has an invalid load balancer type '{cluster.LoadBalancer?.Type}'");
            }

            if (cluster.Authorization?.Enabled == true
                && !_globalValidator.Clusters.AuthAlgorithmIsNotEmpty(cluster.Authorization?.Algorithm))
            {
                return ValidateOptionsResult
                    .Fail($"Cluster '{cluster.Name}' has an empty auth algorithm");
            }

            if (cluster.Authorization?.Enabled == true
                && !_globalValidator.Clusters.AuthAlgorithmIsValid(cluster.Authorization.Algorithm))
            {
                return ValidateOptionsResult
                    .Fail($"Cluster '{cluster.Name}' has an invalid auth algorithm");
            }
        }
        
        return ValidateOptionsResult.Success;
    }

    private ValidateOptionsResult ValidateRoutes(RouteCfg[] routes)
    {
        foreach (var route in routes)
        {
            if (!_globalValidator.Routes.PatternFormatIsValid(route.UpstreamPattern))
            {
                return ValidateOptionsResult
                    .Fail($"Upstream format in route {route.UpstreamPattern} is invalid");
            }

            if (!_globalValidator.Routes.PatternFormatIsValid(route.DownstreamPattern))
            {
                return ValidateOptionsResult
                    .Fail($"Downstream format in route {route.DownstreamPattern} is invalid");
            }

            if (!_globalValidator.Routes.PatternSegmentsNotEmpty(route.UpstreamPattern))
            {
                return ValidateOptionsResult
                    .Fail($"Upstream '{route.UpstreamPattern}' has empty segments in pattern");
            }

            if (!_globalValidator.Routes.PatternSegmentsNotEmpty(route.DownstreamPattern))
            {
                return ValidateOptionsResult
                    .Fail($"Downstream '{route.DownstreamPattern}' has empty segments in pattern"); 
            }

            if (!_globalValidator.Routes.PatternStaticSegmentsLengthIsValid(route.UpstreamPattern))
            {
                return ValidateOptionsResult
                    .Fail($"Static segments length in upstream {route.UpstreamPattern} is invalid");
            }

            if (!_globalValidator.Routes.PatternStaticSegmentsLengthIsValid(route.DownstreamPattern))
            {
                return ValidateOptionsResult
                    .Fail($"Static segments length in downstream {route.DownstreamPattern} is invalid");
            }

            if (!_globalValidator.Routes.PatternStaticSegmentNamesIsValid(route.UpstreamPattern))
            {
                return ValidateOptionsResult
                    .Fail($"Static segment names in upstream {route.UpstreamPattern} is invalid");
            }

            if (!_globalValidator.Routes.PatternStaticSegmentNamesIsValid(route.DownstreamPattern))
            {
                return ValidateOptionsResult
                    .Fail($"Static segment names in downstream {route.DownstreamPattern} is invalid");
            }

            if (!_globalValidator.Routes.PatternParamsCurlyBracesIsValid(route.UpstreamPattern))
            {
                return ValidateOptionsResult
                    .Fail($"Parameter segment names in upstream {route.UpstreamPattern} is invalid");
            }

            if (!_globalValidator.Routes.PatternParamsCurlyBracesIsValid(route.DownstreamPattern))
            {
                return ValidateOptionsResult
                    .Fail($"Parameter segment names in downstream {route.DownstreamPattern} is invalid");
            }

            if (!_globalValidator.Routes.PatternParamNamesNotEmpty(route.UpstreamPattern))
            {
                return ValidateOptionsResult
                    .Fail($"Upstream '{route.UpstreamPattern}' has an empty param names");
            }

            if (!_globalValidator.Routes.PatternParamNamesNotEmpty(route.DownstreamPattern))
            {
                return ValidateOptionsResult
                    .Fail($"Downstream '{route.DownstreamPattern}' has an empty param names");
            }

            if (!_globalValidator.Routes.PatternParamsLengthIsValid(route.UpstreamPattern))
            {
                return ValidateOptionsResult
                    .Fail($"Parameters in upstream {route.UpstreamPattern} is longer than {GlobalConstants.SegmentMaxLength} characters");
            }

            if (!_globalValidator.Routes.PatternParamsLengthIsValid(route.DownstreamPattern))
            {
                return ValidateOptionsResult
                    .Fail($"Parameters in downstream {route.DownstreamPattern} is longer than {GlobalConstants.SegmentMaxLength} characters");
            }

            if (!_globalValidator.Routes.ParamNamesIsValid(route.UpstreamPattern))
            {
                return ValidateOptionsResult
                    .Fail($"Parameter names in upstream {route.UpstreamPattern} is invalid");
            }

            if (!_globalValidator.Routes.ParamNamesIsValid(route.DownstreamPattern))
            {
                return ValidateOptionsResult
                    .Fail($"Parameter names in downstream {route.DownstreamPattern} is invalid");
            }

            if (!_globalValidator.Routes.ParamsCountIsValid(route.UpstreamPattern))
            {
                return ValidateOptionsResult
                    .Fail($"Upstream '{route.UpstreamPattern}' has more than {GlobalConstants.ParamsMaxCount} parameters");
            }

            if (!_globalValidator.Routes.ParamsCountIsValid(route.DownstreamPattern))
            {
                return ValidateOptionsResult
                    .Fail($"Downstream '{route.DownstreamPattern}' has more than {GlobalConstants.ParamsMaxCount} parameters");
            }

            if (!_globalValidator.Routes.ParamsCountIsEqual(route.UpstreamPattern, route.DownstreamPattern))
            {
                return ValidateOptionsResult
                    .Fail($"Parameters count mismatch between {route.UpstreamPattern} and {route.DownstreamPattern}");
            }

            if (!_globalValidator.Routes.HasNoDuplicateParameters(route.UpstreamPattern))
            {
                return ValidateOptionsResult
                    .Fail($"Upstream '{route.UpstreamPattern}' has duplicate parameters");
            }

            if (!_globalValidator.Routes.HasNoDuplicateParameters(route.DownstreamPattern))
            {
                return ValidateOptionsResult
                    .Fail($"Downstream '{route.DownstreamPattern}' has duplicate parameters");
            }

            if (!_globalValidator.Routes.HttpMethodsIsValid(route.Methods.ToArray()))
            {
                return ValidateOptionsResult
                    .Fail($"Upstream '{route.UpstreamPattern}' has an invalid HTTP-methods");
            }

            if (!_globalValidator.Routes.HasNoDuplicateMethods(route.Methods))
            {
                return ValidateOptionsResult
                    .Fail($"Upstream '{route.UpstreamPattern}' has duplicate methods");
            }

            if (!_globalValidator.Routes.MethodUpstreamCombinationIsUnique(route, routes))
            {
                return ValidateOptionsResult
                    .Fail($"Same upstreams '{route.UpstreamPattern}' have duplicate methods");
            }
        }
        
        if (!_globalValidator.Routes.TotalDownstreamPathsLengthIsValid(routes))
        {
            return ValidateOptionsResult
                .Fail($"Total length of all downstream patterns exceeds the maximum allowed size {ushort.MaxValue}");
        }
        
        return ValidateOptionsResult.Success;
    }

    private ValidateOptionsResult ValidateCrossReferences(ClusterCfg[] clusters, RouteCfg[] routes)
    {
        foreach (var route in routes)
        {
            if (!_globalValidator.CrossReferences.ClusterExists(route.ClusterName, clusters))
            {
                return ValidateOptionsResult
                    .Fail($"ClusterId '{route.ClusterName}' in route {route.UpstreamPattern} is invalid");
            }
        }
        
        if (!_globalValidator.CrossReferences.TotalUpstreamPathsLengthIsValid(clusters, routes))
        {
            return ValidateOptionsResult
                .Fail($"Total length of all prefix+upstream patterns exceeds the maximum allowed size {ushort.MaxValue}");
        }

        return ValidateOptionsResult.Success;
    }
}