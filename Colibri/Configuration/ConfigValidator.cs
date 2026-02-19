using Colibri.Configuration.Models;
using Microsoft.Extensions.Options;

namespace Colibri.Configuration;

public static class ConfigValidator
{
    public static bool Validate(ColibriSettings settings)
    {
        var errors = new List<string>();
        
        ValidateRouting(settings.Routing, errors);
        
        return true;
    }

    private static void ValidateRouting(RoutingSettings routingSettings, List<string> errors)
    {
        foreach (var route in routingSettings.Routes)
        {
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

    private static void ValidateParams(RouteCfg route, List<string> errors)
    {
        var upstreamSegments = route.UpstreamPattern
            .Split('/');
        
        var downstreamSegments = route.DownstreamPattern
            .Split('/');

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
                    errors.Add($"Duplicate parameters in route {route.UpstreamPattern}");
                }

                if (downstreamParams[i] == downstreamParams[j])
                {
                    errors.Add($"Duplicate parameters in route {route.DownstreamPattern}");
                }
            }
        }
    }
}