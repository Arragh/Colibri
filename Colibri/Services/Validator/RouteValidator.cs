using System.Text.RegularExpressions;
using Colibri.Configuration.Models;
using Colibri.Helpers;

namespace Colibri.Services.Validator;

public sealed class RouteValidator
{
    public bool ClusterExists(string clusterId, ClusterCfg[] clusters)
    {
        if (clusters.All(c => c.ClusterId != clusterId))
        {
            return false;
        }
        
        return true;
    }
    
    public bool PatternFormatIsValid(string pattern)
    {
        if (!pattern.StartsWith('/'))
        {
            return false;
        }
        
        if (pattern.Length > 1
            && pattern.EndsWith('/'))
        {
            return  false;
        }
        
        return true;
    }
    
    public bool PatternSegmentsNotEmpty(string pattern)
    {
        var patternSegments = ExtractEmptySegments(pattern);
        if (patternSegments.Length > 0)
        {
            return false;
        }

        return true;
    }
    
    public bool PatternStaticSegmentsLengthIsValid(string pattern)
    {
        var patternSegments = ExtractStaticSegments(pattern);
        foreach (var segment in patternSegments)
        {
            if (segment.Length > GlobalConstants.SegmentMaxLength)
            {
                return false;
            }
        }
        
        return true;
    }
    
    public bool PatternStaticSegmentNamesIsValid(string pattern)
    {
        var patternSegments = ExtractStaticSegments(pattern);
        foreach (var segment in patternSegments)
        {
            if (!SegmentNameIsValid(segment))
            {
                return false;
            }
        }
        
        return true;
    }
    
    public bool PatternParamsCurlyBracesIsValid(string pattern)
    {
        var patternSegments = pattern
            .Split('/', StringSplitOptions.RemoveEmptyEntries);
            
        for (int i = 0; i < patternSegments.Length; ++i)
        {
            if ((patternSegments[i].StartsWith('{') && !patternSegments[i].EndsWith('}'))
                || (patternSegments[i].EndsWith('}') && !patternSegments[i].StartsWith('{')))
            {
                return false;
            }
        }
        
        return true;
    }
    
    public bool PatternParamNamesNotEmpty(string pattern)
    {
        var patternParams = ExtractParamSegments(pattern)
            .Where(p => string.IsNullOrWhiteSpace(p[1..^1]))
            .ToArray();

        if (patternParams.Length > 0)
        {
            return false;
        }
        
        return true;
    }
    
    public bool PatternParamsLengthIsValid(string pattern)
    {
        var patternParams = ExtractParamSegments(pattern);
        foreach (var param in patternParams)
        {
            if (param.Length > GlobalConstants.SegmentMaxLength)
            {
                return false;
            }
        }
        
        return true;
    }
    
    public bool ParamNamesIsValid(string pattern)
    {
        var patternParams = ExtractParamSegments(pattern);
        foreach (var param in patternParams)
        {
            if (!SegmentNameIsValid(param[1..^1]))
            {
                return false;
            }
        }
        
        return true;
    }
    
    public bool ParamsCountIsValid(string pattern)
    {
        var patternParams = ExtractParamSegments(pattern);
        if (patternParams.Length > 16)
        {
            return false;
        }
        
        return true;
    }
    
    public bool ParamsCountIsEqual(string upstreamPattern, string downstreamPattern)
    {
        var upstreamParams = ExtractParamSegments(upstreamPattern);
        var downstreamParams = ExtractParamSegments(downstreamPattern);
        
        if (upstreamParams.Length != downstreamParams.Length)
        {
            return false;
        }

        return true;
    }
    
    public bool HasNoDuplicateParameters(string pattern)
    {
        var paramsArray = ExtractParamSegments(pattern);
        for (int i = 0; i < paramsArray.Length; ++i)
        {
            for (int j = i + 1; j < paramsArray.Length; ++j)
            {
                if (paramsArray[i] == paramsArray[j])
                {
                    return false;
                }
            }
        }
        
        return true;
    }
    
    public bool HttpMethodsIsValid(string[] methods)
    {
        foreach (var method in methods)
        {
            if (!GlobalConstants.ValidHttpMethods.Contains(method))
            {
                return false;
            }
        }
        
        return true;
    }

    public bool HasNoDuplicateMethods(string[] methods)
    {
        var methodsWithoutDuplicates = methods
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (methodsWithoutDuplicates.Length != methods.Length)
        {
            return false;
        }
        
        return true;
    }
    
    public bool HasNoDuplicateMethodsBetweenSamePatterns(RouteCfg current, RouteCfg[] routes)
    {
        var methods = current.Methods;

        foreach (var route in routes)
        {
            if (current.Equals(route))
            {
                continue;
            }

            if (current.UpstreamPattern != route.UpstreamPattern)
            {
                continue;
            }

            var duplicates = methods
                .Intersect(route.Methods, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (duplicates.Length > 0)
            {
                return false;
            }
        }

        return true;
    }
    
    private string[] ExtractStaticSegments(string pattern)
    {
        return pattern
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Where(s => !s.StartsWith('{') && !s.EndsWith('}'))
            .ToArray();
    }
        
    private string[] ExtractParamSegments(string pattern)
    {
        return pattern
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Where(s => s.StartsWith('{') && s.EndsWith('}'))
            .ToArray();
    }
    
    private string[] ExtractEmptySegments(string pattern)
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
    
    private bool SegmentNameIsValid(string name)
    {
        var match = Regex.Match(name, "^[a-z0-9_]+$");
        return match.Success;
    }
}