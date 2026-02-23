using System.Text.RegularExpressions;
using Colibri.Configuration.Models;
using Colibri.Helpers;

namespace Colibri.Services.ConfigValidator;

public sealed class ClusterValidator
{
    public bool ClusterIdIsNotEmpty(string clusterId)
    {
        return !string.IsNullOrWhiteSpace(clusterId);
    }

    public bool ClusterIdLengthIsValid(string clusterId)
    {
        return clusterId.Length <= GlobalConstants.SegmentMaxLength;
    }

    public bool ClusterIdIsValid(string clusterId)
    {
        var match = Regex.Match(clusterId, "^[a-z0-9]+$");
        return match.Success;
    }

    public bool ClusterIdIsUnique(ClusterCfg current, ClusterCfg[] clusters)
    {
        foreach (var cluster in clusters)
        {
            if (cluster.Equals(current))
            {
                continue;
            }

            if (current.ClusterId == cluster.ClusterId)
            {
                return false;
            }
        }
        
        return true;
    }
    
    public bool ProtocolIsNotEmpty(string protocol)
    {
        return !string.IsNullOrWhiteSpace(protocol);
    }

    public bool ProtocolIsValid(string protocol)
    {
        return GlobalConstants.ValidProtocols.Contains(protocol);
    }
    
    public bool PrefixIsNotEmpty(string prefix)
    {
        return !string.IsNullOrWhiteSpace(prefix);
    }

    public bool PrefixIsValid(string prefix)
    {
        if (prefix[0] != '/')
        {
            return false;
        }
        
        if (!PrefixIsMatch(prefix[1..]))
        {
            return false;
        }
        
        return true;
    }

    public bool HostsAreNotEmpty(string[] hosts)
    {
        return hosts.Length > 0;
    }

    private bool PrefixIsMatch(string name)
    {
        var match = Regex.Match(name, "^[a-z0-9_]+$");
        return match.Success;
    }
}