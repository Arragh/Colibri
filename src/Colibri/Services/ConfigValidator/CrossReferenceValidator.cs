using Colibri.Configuration.Models;

namespace Colibri.Services.ConfigValidator;

public sealed class CrossReferenceValidator
{
    public bool ClusterExists(string name, ClusterCfg[] clusters)
    {
        if (clusters.All(c => c.Name != name))
        {
            return false;
        }
        
        return true;
    }

    public bool TotalUpstreamPathsLengthIsValid(ClusterCfg[] clusters, RouteCfg[] routes)
    {
        int totalUpstreamPathLength = 0;
        
        foreach (var cluster in clusters)
        {
            totalUpstreamPathLength += cluster.Prefix.Length;
        }
        
        foreach (var route in routes)
        {
            totalUpstreamPathLength += route.UpstreamPattern.Length;
        }
        
        return totalUpstreamPathLength <= ushort.MaxValue;
    }
}