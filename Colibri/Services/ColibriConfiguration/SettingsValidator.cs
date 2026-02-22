using Colibri.Configuration;
using Colibri.Helpers;
using Microsoft.Extensions.Options;

namespace Colibri.Services.ColibriConfiguration;

public sealed class SettingsValidator : IValidateOptions<ColibriSettings>
{
    private readonly GlobalValidator _globalValidator = new();
    
    /// <summary>
    /// Validates the Colibri configuration. Returns fail if invalid.
    /// </summary>
    public ValidateOptionsResult Validate(string? name, ColibriSettings options)
    {
        var routes = options.Routing.Routes;

        foreach (var route in routes)
        {
            if (!_globalValidator.Routes.ClusterExists(route.ClusterId, options.Routing.Clusters))
            {
                return ValidateOptionsResult
                    .Fail($"ClusterId '{route.ClusterId}' in route {route.UpstreamPattern} is invalid");
            }

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

            if (!_globalValidator.Routes.HasNoDuplicateMethodsBetweenSamePatterns(route, routes))
            {
                return ValidateOptionsResult
                    .Fail($"Same upstreams '{route.UpstreamPattern}' have duplicate methods");
            }
        }
        
        return ValidateOptionsResult.Success;
    }
}