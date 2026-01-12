using Colibri.Configuration;
using Colibri.Configuration.Models;

namespace Colibri.Runtime.Snapshots.Routing;

public sealed class RoutingSnapshotBuilder
{
    
    public static RoutingSnapshot Build(ColibriSettings settings)
    {
        var tempClusters = BuildTempClusters(settings.Routing.Clusters, settings.Routing.Routes);
        var trie = BuildTrie(tempClusters);

        throw new NotImplementedException();
    }
    
    private static List<TempCluster> BuildTempClusters(ClusterCfg[] cfgClusters, RouteCfg[] cfgRoutes)
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
                var tempRoute = new TempRoute
                {
                    Methods = route.Methods,
                    UpstreamPattern = route.UpstreamPattern
                        .Split('/', StringSplitOptions.RemoveEmptyEntries),
                    DownstreamPattern = route.DownstreamPattern
                        .Split('/', StringSplitOptions.RemoveEmptyEntries),
                };

                var paramIndex = 0;
                foreach (var segment in tempRoute.UpstreamPattern)
                {
                    var upstreamChunk = new UpstreamChunk
                    {
                        Name = segment
                    };
                    
                    if (segment.StartsWith('{') && segment.EndsWith('}'))
                    {
                        upstreamChunk.IsParameter = true;
                        upstreamChunk.ParamIndex = paramIndex++;
                    }
                    
                    tempRoute.UpstreamChunks.Add(upstreamChunk);
                }

                foreach (var segment in tempRoute.DownstreamPattern)
                {
                    var downstreamChunk = new DownstreamChunk
                    {
                        Name = segment
                    };
                    
                    var upstreamChunk = tempRoute.UpstreamChunks
                        .FirstOrDefault(c => c.Name == segment);
                    
                    if (upstreamChunk != null)
                    {
                        downstreamChunk.IsParameter = upstreamChunk.IsParameter;
                        downstreamChunk.ParamIndex = upstreamChunk.ParamIndex;
                    }
                    
                    tempRoute.DownstreamChunks.Add(downstreamChunk);
                }

                tempCluster.Routes.Add(tempRoute);
            }

            tempClusters.Add(tempCluster);
        }

        return tempClusters;
    }

    private static TrieNode BuildTrie(List<TempCluster> tempClusters)
    {
        var root = new TrieNode();

        foreach (var tempCluster in tempClusters)
        {
            var node = new TrieNode
            {
                Name = tempCluster.Prefix
            };
            
            root.Children.Add(node);
        }

        for (int i = 0; i < tempClusters.Count; i++)
        {
            foreach (var tempRoute in tempClusters[i].Routes)
            {
                createChildrenRecursively(root.Children[i], tempRoute);
            }
        }
        
        orderTrieRecursively(root);

        return root;

        void createChildrenRecursively(TrieNode root, TempRoute tempRoute)
        {
            var segment = tempRoute.UpstreamPattern[0];
            
            var child = root.Children
                .FirstOrDefault(c => c.Name == segment);

            if (child == null)
            {
                child = new TrieNode
                {
                    Name = segment,
                    IsParameter = segment.StartsWith('{') && segment.EndsWith('}')
                };
                
                root.Children.Add(child);
            }
            
            var nextRoute = tempRoute with { UpstreamPattern = tempRoute.UpstreamPattern[1..] };
            if (nextRoute.UpstreamPattern.Length > 0)
            {
                createChildrenRecursively(child, nextRoute);
            }
            else
            {
                foreach (var method in tempRoute.Methods)
                {
                    child.Methods.Add(method, tempRoute.DownstreamChunks);
                }
            }
        }

        void orderTrieRecursively(TrieNode root)
        {
            root.Children = root.Children
                .OrderBy(c => c.IsParameter)
                .ThenByDescending(c => c.Name!.Length)
                .ToList();

            foreach (var child in root.Children)
            {
                orderTrieRecursively(child);
            }
        }
    }
    
    private sealed class TempCluster
    {
        public string Prefix { get; set; }
        public List<TempRoute> Routes { get; } = [];
    }

    private sealed record TempRoute
    {
        public string[] Methods { get; set; }
        public string[] UpstreamPattern { get; set; }
        public string[] DownstreamPattern { get; set; }
        public List<UpstreamChunk> UpstreamChunks { get; } = [];
        public List<DownstreamChunk> DownstreamChunks { get; } = [];
    }

    private sealed class TrieNode
    {
        public string? Name { get; set; }
        public bool IsParameter { get; set; }
        public List<TrieNode> Children { get; set; } = [];
        public Dictionary<string, List<DownstreamChunk>> Methods { get; } = [];
    }
    
    private sealed class UpstreamChunk
    {
        public string Name { get; set; }
        public bool IsParameter { get; set; }
        public int ParamIndex { get; set; }
    }
    
    private sealed class DownstreamChunk
    {
        public string Name { get; set; }
        public bool IsParameter { get; set; }
        public int ParamIndex { get; set; }
    }
}