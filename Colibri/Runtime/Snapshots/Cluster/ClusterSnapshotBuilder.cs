using System.Collections.Immutable;
using Colibri.Configuration;
using Colibri.Runtime.Pipeline;
using Colibri.Runtime.Pipeline.CircuitBreaker;
using Colibri.Runtime.Pipeline.LoadBalancer;
using Colibri.Runtime.Pipeline.RateLimiter;
using Colibri.Runtime.Pipeline.Retrier;
using Colibri.Runtime.Pipeline.Terminal;

namespace Colibri.Runtime.Snapshots.Cluster;

public sealed class ClusterSnapshotBuilder
{
    public ClusterSnapshot Build(ColibriSettings settings)
    {
        var snpClusters = new List<ClusterSnp>();
        
        foreach (var cfgCluster in settings.Routing.Clusters)
        {
            if (!cfgCluster.Enabled)
            {
                continue;
            }
            
            List<IPipelineMiddleware> clusterMiddlewares = new();

            if (cfgCluster.RateLimit.Enabled)
            {
                clusterMiddlewares.Add(new RateLimiterMiddleware());
            }
            
            if (cfgCluster.LoadBalancing.Enabled)
            {
                clusterMiddlewares.Add(new LoadBalancerMiddleware());
            }

            if (cfgCluster.Retry.Enabled)
            {
                clusterMiddlewares.Add(new RetryMiddleware());
            }
            
            if (cfgCluster.CircuitBreaker.Enabled)
            {
                clusterMiddlewares.Add(new CircuitBreakerMiddleware());
            }
            
            switch (cfgCluster.Protocol.ToLower())
            {
                case "http":
                    clusterMiddlewares.Add(new HttpTerminalMiddleware(cfgCluster.Hosts));
                    break;
                
                default:
                    throw new ArgumentException($"Invalid protocol {cfgCluster.Protocol}");
            }
            
            var snpCluster = new ClusterSnp
            {
                ClusterId = cfgCluster.ClusterId,
                Protocol = cfgCluster.Protocol,
                Prefix = cfgCluster.Prefix,
                HostsCount = cfgCluster.Hosts.Length,
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