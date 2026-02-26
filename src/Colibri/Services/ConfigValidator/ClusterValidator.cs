using System.Text.RegularExpressions;
using Colibri.Configuration.Models;
using Colibri.Helpers;

namespace Colibri.Services.ConfigValidator;

public sealed class ClusterValidator
{
    public bool NameIsNotEmpty(string name)
    {
        return !string.IsNullOrWhiteSpace(name);
    }

    public bool NameLengthIsValid(string name)
    {
        return name.Length <= GlobalConstants.SegmentMaxLength;
    }

    public bool NameIsValid(string name)
    {
        var match = Regex.Match(name, "^[a-z0-9]+$");
        return match.Success;
    }

    public bool NameIsUnique(ClusterCfg current, ClusterCfg[] clusters)
    {
        foreach (var cluster in clusters)
        {
            if (cluster.Equals(current))
            {
                continue;
            }

            if (current.Name == cluster.Name)
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

    public bool LoadBalancerTypeIsNotEmpty(string loadBalancerType)
    {
        return !string.IsNullOrWhiteSpace(loadBalancerType);
    }

    public bool LoadBalancerTypeIsValid(string loadBalancerType)
    {
        return GlobalConstants.LoadBalancerTypes.Contains(loadBalancerType);
    }
    
    private bool PrefixIsMatch(string name)
    {
        var match = Regex.Match(name, "^[a-z0-9_-]+$");
        return match.Success;
    }
}