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
        SortDataInTempClusters(tempClusters);
        var preparedData = PrepareDataForRoutingSnapshot(tempClusters);
        
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

    private PreparedData PrepareDataForRoutingSnapshot(List<TempCluster> tempClusters)
    {
        List<TempPrefix> tempPrefixes = [];
        List<char> prefixesChars = [];

        for (int i = 0; i < tempClusters.Count; i++)
        {
            var tempPrefix = new TempPrefix
            {
                ClusterIndex = (short)i,
                PrefixStartIndex = prefixesChars.Count,
                PrefixLength = (byte)tempClusters[i].Prefix.Length
            };
            prefixesChars.AddRange(tempClusters[i].Prefix.ToArray());
            tempPrefixes.Add(tempPrefix);
        }

        return new PreparedData
        {
            TempPrefixes = tempPrefixes.ToArray(),
            PrefixesChars = prefixesChars.ToArray()
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

            var nextTempRoute = tempRoute with { UpstreamPattern = tempRoute.UpstreamPattern[1..] };

            if (nextTempRoute.UpstreamPattern.Length == 0)
            {
                node.Methods.Add(nextTempRoute.Method, nextTempRoute.DownstreamPattern);
                return;
            }
            
            fillTrieRecursively(node.ChildrenNodes, nextTempRoute);
        }
    }

    private void SortDataInTempClusters(List<TempCluster> tempClusters)
    {
        foreach (var tempCluster in tempClusters)
        {
            sortNodesInCluster(tempCluster);
        }
        
        void sortNodesInCluster(TempCluster tempCluster)
        {
            tempCluster.ChildrenNodes = tempCluster.ChildrenNodes
                .OrderBy(n => n.IsParameter)
                .ThenByDescending(n => n.Name!.Length)
                .ToList();
            
            foreach (var child in tempCluster.ChildrenNodes)
            {
                sortNodesRecursively(child);
            }
            
            void sortNodesRecursively(TrieNode node)
            {
                node.ChildrenNodes = node.ChildrenNodes
                    .OrderBy(s => s.IsParameter)
                    .ThenByDescending(s => s.Name!.Length)
                    .ToList();
            
                foreach (var child in node.ChildrenNodes)
                {
                    sortNodesRecursively(child);
                }
            }
        }
    }

    private sealed class TempCluster
    {
        public required string Prefix { get; init; }
        public List<TempRoute> Routes { get; } = [];
        public List<TrieNode> ChildrenNodes { get; set; } = [];
    }

    private sealed record TempRoute
    {
        public required string Method { get; init; }
        public required string[] UpstreamPattern { get; set; }
        public required string[] DownstreamPattern { get; init; }
    }

    private sealed class TrieNode
    {
        public required string? Name { get; init; }
        public required bool IsParameter { get; init; }
        public List<TrieNode> ChildrenNodes { get; set; } = [];
        public Dictionary<string, string[]> Methods { get; } = [];
    }
    
    private sealed class TempPrefix
    {
        public short ClusterIndex { get; set; }
        public int PrefixStartIndex { get; set; }
        public byte PrefixLength { get; set; }
    }

    private sealed class PreparedData
    {
        public required TempPrefix[] TempPrefixes { get; init; }
        public required char[] PrefixesChars { get; init; }
    }
}