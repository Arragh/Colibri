using System.Collections.Immutable;
using Colibri.Configuration.Models;
using Colibri.Runtime.Pipeline;
using Colibri.Runtime.Pipeline.Cluster.Authorization;
using Colibri.Runtime.Pipeline.Cluster.CircuitBreaker;
using Colibri.Runtime.Pipeline.Cluster.LoadBalancer;
using Colibri.Runtime.Pipeline.Cluster.Retrier;
using Colibri.Runtime.Pipeline.Cluster.Terminal;

namespace Colibri.Runtime.Snapshots.Cluster;

public sealed class ClusterSnapshotBuilder
{
    public ClusterSnapshot Build(ClusterCfg[] cfgClusters)
    {
        var snpClusters = new List<ClusterSnp>();
        
        foreach (var cfgCluster in cfgClusters)
        {
            if (!cfgCluster.Enabled)
            {
                continue;
            }
            
            List<IPipelineMiddleware> clusterMiddlewares = new();
            
            var hostsCount = cfgCluster.Hosts.Length;

            if (cfgCluster.Authorization?.Enabled == true)
            {
                clusterMiddlewares.Add(new AuthorizationMiddleware(
                    cfgCluster.Authorization.Algorithm,
                    cfgCluster.Authorization.Key));
            }
            
           
            if (cfgCluster.Retry?.Enabled == true)
            {
                clusterMiddlewares.Add(new RetryMiddleware(cfgCluster.Retry.Attempts));
            }
            
            if (cfgCluster.LoadBalancer?.Enabled == true)
            {
                ILoadBalancer loadBalancer = cfgCluster.LoadBalancer.Type switch
                {
                    "rr" => new RoundRobinBalancer(hostsCount),
                    "rnd" => new RandomBalancer(hostsCount)
                };
                
                clusterMiddlewares.Add(new LoadBalancerMiddleware(loadBalancer));
            }
            
            if (cfgCluster.CircuitBreaker?.Enabled == true)
            {
                var breaker = new CircuitBreaker(
                    hostsCount,
                    cfgCluster.CircuitBreaker.Failures,
                    cfgCluster.CircuitBreaker.Timeout);
                
                clusterMiddlewares.Add(new CircuitBreakerMiddleware(breaker));
            }
            
            switch (cfgCluster.Protocol.ToLower())
            {
                case "http":
                    clusterMiddlewares.Add(new HttpTerminalMiddleware(cfgCluster.Hosts));
                    break;
                
                case "ws":
                    clusterMiddlewares.Add(new WebsocketTerminalMiddleware(cfgCluster.Hosts));
                    break;
                
                default:
                    throw new ArgumentException($"Invalid protocol {cfgCluster.Protocol}");
            }
            
            var snpCluster = new ClusterSnp
            {
                Name = cfgCluster.Name,
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