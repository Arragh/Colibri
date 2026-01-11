using Colibri.Configuration;
using Colibri.Configuration.Models;

namespace Colibri.Runtime.Snapshots.Routing;

public class RoutingSnapshotBuilder
{
    
    public static RoutingSnapshot Build(ColibriSettings settings)
    {
        var tempClusters = BuildTempClusters(settings.Routing.Clusters, settings.Routing.Routes);
        SortDataInTempClusters(tempClusters);
        var preparedData = PrepareDataForRoutingSnapshot(tempClusters);
        
        throw new NotImplementedException();
    }
    
    private static PreparedData PrepareDataForRoutingSnapshot(List<TempCluster> tempClusters)
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

    private static void SortDataInTempClusters(List<TempCluster> tempClusters)
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