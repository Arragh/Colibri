using System.Collections.Immutable;
using Colibri.Configuration;
using Colibri.Configuration.Models;
using Colibri.Runtime.Pipeline;
using Colibri.Runtime.Pipeline.CircuitBreaker;
using Colibri.Runtime.Pipeline.LoadBalancer;
using Colibri.Runtime.Pipeline.RateLimiter;
using Colibri.Runtime.Pipeline.Retrier;
using Colibri.Runtime.Pipeline.Terminal;
using Colibri.Runtime.Snapshots.Cluster;

namespace Colibri.Runtime.Snapshots;

public sealed class SnapshotBuilder
{
    public GlobalSnapshot Build(ColibriSettings settings)
    {
        var tempClusters = BuildTempClusters(settings.Routing.Clusters, settings.Routing.Routes);
        
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

            switch (cfgCluster.Protocol)
            {
                case "Http":
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

    private List<TempCluster> BuildTempClusters(ClusterCfg[] cfgClusters, RouteCfg[] cfgRoutes)
    {
        var tempClusters = new List<TempCluster>();
        
        foreach (var cluster in cfgClusters)
        {
            var clusterRoutes = cfgRoutes
                .Where(r => r.ClusterId == cluster.ClusterId)
                .ToArray();

            var tempCluster = new TempCluster
            {
                Prefix = cluster.Prefix
            };

            foreach (var route in clusterRoutes)
            {
                foreach (var method in route.Methods)
                {
                    tempCluster.Routes.Add(new TempRoute
                    {
                        Method = method,
                        UpstreamPattern = route.UpstreamPattern.Split('/', StringSplitOptions.RemoveEmptyEntries),
                        DownstreamPattern = route.DownstreamPattern.Split('/', StringSplitOptions.RemoveEmptyEntries),
                    });
                }
            }

            foreach (var tempRoute in tempCluster.Routes)
            {
                fillTrieRecursively(tempCluster.ChildrenNodes, tempRoute);
            }
            
            tempClusters.Add(tempCluster);
        }

        return tempClusters;

        void fillTrieRecursively(List<TrieNode> childrenNodes, TempRoute tempRoute)
        {
            var node = childrenNodes
                .FirstOrDefault(n => n.Name ==  tempRoute.UpstreamPattern[0]);

            if (node == null)
            {
                node = new TrieNode
                {
                    Name = tempRoute.UpstreamPattern[0],
                    IsParameter = tempRoute.UpstreamPattern[0].StartsWith('{')
                                  && tempRoute.UpstreamPattern[0].EndsWith('}')
                };
                
                childrenNodes.Add(node);
            }

            tempRoute.UpstreamPattern = tempRoute.UpstreamPattern[1..];

            if (tempRoute.UpstreamPattern.Length == 0)
            {
                node.Methods.Add(tempRoute.Method, tempRoute.DownstreamPattern);
                return;
            }
            
            fillTrieRecursively(node.ChildrenNodes, tempRoute);
        }
    }

    private sealed class TempCluster
    {
        public required string Prefix { get; init; }
        public List<TempRoute> Routes { get; } = [];
        public List<TrieNode> ChildrenNodes { get; } = [];
    }

    private sealed class TempRoute
    {
        public required string Method { get; init; }
        public required string[] UpstreamPattern { get; set; }
        public required string[] DownstreamPattern { get; init; }
    }

    private sealed class TrieNode
    {
        public required string? Name { get; init; }
        public required bool IsParameter { get; init; }
        public List<TrieNode> ChildrenNodes { get; } = [];
        public Dictionary<string, string[]> Methods { get; } = [];
    }
}