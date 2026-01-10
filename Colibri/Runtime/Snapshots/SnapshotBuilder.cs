using System.Collections.Immutable;
using Colibri.Configuration;
using Colibri.Configuration.Models;
using Colibri.Runtime.Pipeline;
using Colibri.Runtime.Pipeline.CircuitBreaker;
using Colibri.Runtime.Pipeline.LoadBalancer;
using Colibri.Runtime.Pipeline.RateLimiter;
using Colibri.Runtime.Pipeline.Retrier;
using Colibri.Runtime.Snapshots.Cluster;

namespace Colibri.Runtime.Snapshots;

public sealed class SnapshotBuilder
{
    public GlobalSnapshot Build(ColibriSettings settings)
    {
        return new GlobalSnapshot
        {
            ClusterSnapshot = BuildClusterSnapshot(settings.Routing.Clusters)
        };
    }
    
    private ClusterSnapshot BuildClusterSnapshot(ClusterCfg[] cfgClusters)
    {
        var snpClusters = new List<ClusterSnp>();
        
        foreach (var cfgCluster in cfgClusters)
        {
            List<IPipelineMiddleware> clusterMiddlewares = new();

            if (cfgCluster.RateLimit.Enabled)
            {
                clusterMiddlewares.Add(new RateLimiterMiddleware());
            }

            if (cfgCluster.Retry.Enabled)
            {
                clusterMiddlewares.Add(new RetryMiddleware());
            }
            
            if (cfgCluster.CircuitBreaker.Enabled)
            {
                clusterMiddlewares.Add(new CircuitBreakerMiddleware());
            }
            
            if (cfgCluster.LoadBalancing.Enabled)
            {
                clusterMiddlewares.Add(new LoadBalancerMiddleware());
            }
            
            var snpCluster = new ClusterSnp
            {
                Enabled = cfgCluster.Enabled,
                ClusterId = cfgCluster.ClusterId,
                Protocol = cfgCluster.Protocol,
                Prefix = cfgCluster.Prefix,
                Hosts = cfgCluster.Hosts
                    .Select(h => new Uri(h))
                    .ToArray(),
                Pipeline = new PipelineSrv(clusterMiddlewares.ToArray())
            };
            
            snpClusters.Add(snpCluster);
        }

        return new ClusterSnapshot
        {
            Clusters = snpClusters.ToImmutableArray()
        };
    }
}