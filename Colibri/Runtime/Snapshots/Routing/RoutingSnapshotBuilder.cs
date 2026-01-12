using Colibri.Configuration;
using Colibri.Configuration.Models;

namespace Colibri.Runtime.Snapshots.Routing;

public sealed class RoutingSnapshotBuilder
{
    
    public static RoutingSnapshot Build(ColibriSettings settings)
    {
        var tempClusters = BuildTempClusters(settings.Routing.Clusters, settings.Routing.Routes);
        SortDataInTempClusters(tempClusters);
        var trie = BuildTrieFromClusters(tempClusters);

        List<TempPrefix> prefixes = [];
        List<char> prefixesChars = [];
        List<TempUpstreamSegment> segments = [];
        List<char> segmentsChars = [];
        
        Trololo1(trie, segments, segmentsChars,  prefixes, prefixesChars);
        
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
                fillTrieRecursively(tempCluster.Children, tempRoute);
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
            
            fillTrieRecursively(node.Children, nextTempRoute);
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
            tempCluster.Children = tempCluster.Children
                .OrderBy(n => n.IsParameter)
                .ThenByDescending(n => n.Name!.Length)
                .ToList();
            
            foreach (var child in tempCluster.Children)
            {
                sortNodesRecursively(child);
            }
            
            void sortNodesRecursively(TrieNode node)
            {
                node.Children = node.Children
                    .OrderBy(s => s.IsParameter)
                    .ThenByDescending(s => s.Name!.Length)
                    .ToList();
            
                foreach (var child in node.Children)
                {
                    sortNodesRecursively(child);
                }
            }
        }
    }

    private static TrieNode BuildTrieFromClusters(List<TempCluster> tempClusters)
    {
        var trie = new TrieNode();

        foreach (var tempCluster in tempClusters)
        {
            trie.Children.Add(new TrieNode
            {
                Name = tempCluster.Prefix,
                Children = tempCluster.Children
            });
        }
        
        return trie;
    }
    
    static void Trololo1(
        TrieNode trie,
        List<TempUpstreamSegment> segments,
        List<char> segmentsChars,
        List<TempPrefix> prefixes,
        List<char> prefixesChars)
    {
        if (prefixes.Count == 0)
        {
            foreach (var child in trie.Children)
            {
                var prefix = new TempPrefix
                {
                    PrefixStartIndex = prefixesChars.Count,
                    PrefixLength = child.Name!.Length
                };

                prefixesChars.AddRange(child.Name!);
                prefixes.Add(prefix);
            }

            for (int i = 0; i < trie.Children.Count; i++)
            {
                prefixes[i].ChildrenCount = (short)trie.Children[i].Children.Count;

                if (prefixes[i].ChildrenCount > 0)
                {
                    prefixes[i].FirstChildIndex = segments.Count;
                }
                
                Trololo1(trie.Children[i], segments, segmentsChars, prefixes, prefixesChars);
            }
        }
        else
        {
            foreach (var child in trie.Children)
            {
                var segment = new TempUpstreamSegment
                {
                    PathStartIndex = segmentsChars.Count,
                    PathLength = child.Name!.Length + 1
                };

                segmentsChars.AddRange('/' + child.Name!);
                segments.Add(segment);
                
                // foreach (var method in  child.Methods)
                // {
                //     var tempDownstream = new TempDownstream();
                //
                //     tempDownstream.PathStartIndex = downstreamPaths.Length;
                //     tempDownstream.PathLength = (short)method.Value.Length;
                //     downstreamPaths += method.Value;
                //     tempDownstream.MethodMask = HttpMethodMask.GetMask(method.Key);
                //     tempDownstream.HostStartIndex = (short)child.HostStartIndex;
                //     tempDownstream.HostsCount = (byte)child.HostsCount;
                //     
                //     tempDownstreams.Add(tempDownstream);
                //
                //     tempSegment.DownstreamStartIndex = (short)(tempDownstreams.Count - 1);
                //     tempSegment.DownstreamCount++;
                //     tempSegment.MethodMask |= HttpMethodMask.GetMask(method.Key);
                // }
            }
            
            var createdTempSegments = segments
                .TakeLast(trie.Children.Count)
                .ToList();
     
            for (int i = 0; i < trie.Children.Count; i++)
            {
                createdTempSegments[i].ChildrenCount = (short)trie.Children[i].Children.Count;

                if (createdTempSegments[i].ChildrenCount > 0)
                {
                    createdTempSegments[i].FirstChildIndex = segments.Count;
                }
            
                Trololo1(trie.Children[i], segments, segmentsChars, prefixes, prefixesChars);
            }
        }
    }
    
    private sealed class TempCluster
    {
        public required string Prefix { get; init; }
        public List<TempRoute> Routes { get; } = [];
        public List<TrieNode> Children { get; set; } = [];
    }

    private sealed record TempRoute
    {
        public required string Method { get; init; }
        public required string[] UpstreamPattern { get; set; }
        public required string[] DownstreamPattern { get; init; }
    }

    private sealed class TrieNode
    {
        public string? Name { get; init; }
        public bool IsParameter { get; init; }
        public List<TrieNode> Children { get; set; } = [];
        public Dictionary<string, string[]> Methods { get; } = [];
    }
    
    private sealed class TempPrefix
    {
        public int ClusterIndex { get; set; }
        public int PrefixStartIndex { get; set; }
        public int PrefixLength { get; set; }
        public int FirstChildIndex { get; set; }
        public int ChildrenCount { get; set; }
    }

    private sealed class TempUpstreamSegment
    {
        public int PathStartIndex { get; set; }
        public int PathLength { get; set; }
        public int FirstChildIndex { get; set; }
        public int ChildrenCount { get; set; }
    }
}