using Colibri.Configuration;
using Colibri.Configuration.Models;
using Colibri.Helpers;

namespace Colibri.Runtime.Snapshots.Routing;

public sealed class RoutingSnapshotBuilder
{
    
    public static RoutingSnapshot Build(ColibriSettings settings)
    {
        var tempClusters = BuildTempClusters(settings.Routing.Clusters, settings.Routing.Routes);
        var trie = BuildTrie(tempClusters);
        var snapshotData = BuildDataForSnapshot(trie);

        var clusterSegments = new ClusterSegment[snapshotData.TempClusterSegments.Length];
        var upstreamSegments = new UpstreamSegment[snapshotData.TempUpstreamSegments.Length];
        var downstreams = new Downstream[snapshotData.TempDownstreams.Length];
        var downstreamSegments = new DownstreamSegment[snapshotData.TempDownstreamSegments.Length];

        for (int i = 0; i < snapshotData.TempClusterSegments.Length; i++)
        {
            clusterSegments[i] = new ClusterSegment(
                snapshotData.TempClusterSegments[i].PathStartIndex,
                snapshotData.TempClusterSegments[i].PathLength,
                snapshotData.TempClusterSegments[i].FirstChildIndex,
                snapshotData.TempClusterSegments[i].ChildrenCount);
        }

        for (int i = 0; i < snapshotData.TempUpstreamSegments.Length; i++)
        {
            upstreamSegments[i] = new UpstreamSegment(
                snapshotData.TempUpstreamSegments[i].PathStartIndex,
                snapshotData.TempUpstreamSegments[i].PathLength,
                snapshotData.TempUpstreamSegments[i].FirstChildIndex,
                snapshotData.TempUpstreamSegments[i].ChildrenCount,
                snapshotData.TempUpstreamSegments[i].IsParameter,
                snapshotData.TempUpstreamSegments[i].ParamIndex,
                snapshotData.TempUpstreamSegments[i].DownstreamStartIndex,
                snapshotData.TempUpstreamSegments[i].DownstreamsCount);
        }

        for (int i = 0; i < snapshotData.TempDownstreams.Length; i++)
        {
            downstreams[i] = new Downstream(
                snapshotData.TempDownstreams[i].FirstChildIndex,
                snapshotData.TempDownstreams[i].ChildrenCount,
                snapshotData.TempDownstreams[i].MethodMask);
        }

        for (int i = 0; i < snapshotData.TempDownstreamSegments.Length; i++)
        {
            downstreamSegments[i] = new DownstreamSegment(
                snapshotData.TempDownstreamSegments[i].PathStartIndex,
                snapshotData.TempDownstreamSegments[i].PathLength,
                snapshotData.TempDownstreamSegments[i].IsParameter,
                snapshotData.TempDownstreamSegments[i].ParamIndex);
        }

        return new RoutingSnapshot(
            clusterSegments,
            snapshotData.TempClusterPaths,
            upstreamSegments,
            snapshotData.TempUpstreamSegmentPaths,
            downstreams,
            downstreamSegments,
            snapshotData.TempDownstreamSegmentPaths);
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
            var chunk = tempRoute.UpstreamChunks[0];
            
            var child = root.Children
                .FirstOrDefault(c => c.Name == chunk.Name);

            if (child == null)
            {
                child = new TrieNode
                {
                    Name = chunk.Name,
                    IsParameter = chunk.IsParameter,
                    ParamIndex = chunk.ParamIndex
                };
                
                root.Children.Add(child);
            }
            
            var nextRoute = tempRoute with { UpstreamChunks = tempRoute.UpstreamChunks[1..] };
            if (nextRoute.UpstreamChunks.Count > 0)
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

    private static DataForSnapshot BuildDataForSnapshot(TrieNode root)
    {
        List<TempClusterSegment> clustersSegments = [];
        List<char> clustersPaths = [];
        
        List<TempUpstreamSegment> upstreamSegments = [];
        List<char> upstreamSegmentsPaths = [];
        
        List<TempDownstream> downstreams = [];
        List<TempDownstreamSegment> downstreamSegments = [];
        List<char> downstreamSegmentsPaths = [];

        foreach (var cluster in root.Children)
        {
            var clasterSegment = new TempClusterSegment
            {
                PathStartIndex = clustersPaths.Count,
                PathLength = cluster.Name!.Length,
                FirstChildIndex = upstreamSegments.Count,
                ChildrenCount = cluster.Children.Count
            };
                
            clustersPaths.AddRange(cluster.Name);
            clustersSegments.Add(clasterSegment);

            trololo1(cluster);
        }

        void trololo1(TrieNode root)
        {
            foreach (var child in root.Children)
            {
                var segmentName = '/' + child.Name;
                var upstreamSegment = new TempUpstreamSegment
                {
                    PathStartIndex = upstreamSegmentsPaths.Count,
                    PathLength = segmentName.Length,
                    IsParameter = child.IsParameter,
                    ParamIndex = child.ParamIndex,
                    ChildrenCount = child.Children.Count
                };
                
                upstreamSegmentsPaths.AddRange(segmentName);
                upstreamSegments.Add(upstreamSegment);
            }

            var justCreatedUpstreamSegments = upstreamSegments
                .TakeLast(root.Children.Count)
                .ToArray();

            for (int i = 0; i < root.Children.Count; i++)
            {
                if (root.Children[i].Children.Count > 0)
                {
                    justCreatedUpstreamSegments[i].FirstChildIndex = upstreamSegments.Count;
                    trololo1(root.Children[i]);
                }
                else
                {
                    justCreatedUpstreamSegments[i].DownstreamStartIndex = downstreams.Count;

                    foreach (var method in root.Children[i].Methods)
                    {
                        var tempDownstream = new TempDownstream
                        {
                            FirstChildIndex = downstreamSegments.Count,
                            ChildrenCount = method.Value.Count,
                            MethodMask = HttpMethodMask.GetMask(method.Key),
                        };

                        foreach (var chunk in method.Value)
                        {
                            var segmentName = '/' + chunk.Name;
                            var tempDownstreamSegment = new TempDownstreamSegment
                            {
                                PathStartIndex = downstreamSegmentsPaths.Count,
                                PathLength = segmentName.Length,
                                IsParameter = chunk.IsParameter,
                                ParamIndex = chunk.ParamIndex,
                            };
                            
                            downstreamSegmentsPaths.AddRange(segmentName);
                            downstreamSegments.Add(tempDownstreamSegment);
                        }
                        
                        downstreams.Add(tempDownstream);
                    }
                    
                    justCreatedUpstreamSegments[i].DownstreamsCount = downstreams.Count - justCreatedUpstreamSegments[i].DownstreamStartIndex;
                }
            }
        }

        return new DataForSnapshot
        {
            TempClusterSegments = clustersSegments.ToArray(),
            TempClusterPaths = clustersPaths.ToArray(),
            TempUpstreamSegments = upstreamSegments.ToArray(),
            TempUpstreamSegmentPaths = upstreamSegmentsPaths.ToArray(),
            TempDownstreams = downstreams.ToArray(),
            TempDownstreamSegments = downstreamSegments.ToArray(),
            TempDownstreamSegmentPaths = downstreamSegmentsPaths.ToArray()
        };
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
        public List<UpstreamChunk> UpstreamChunks { get; set; } = [];
        public List<DownstreamChunk> DownstreamChunks { get; set; } = [];
    }

    private sealed class TrieNode
    {
        public string? Name { get; set; }
        public bool IsParameter { get; set; }
        public int ParamIndex { get; set; }
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

    private sealed class TempClusterSegment
    {
        public int PathStartIndex { get; set; }
        public int PathLength { get; set; }
        public int FirstChildIndex { get; set; }
        public int ChildrenCount { get; set; }
    }

    private sealed class TempUpstreamSegment
    {
        public int PathStartIndex { get; set; }
        public int PathLength { get; set; }
        public int FirstChildIndex { get; set; }
        public int ChildrenCount { get; set; }
        public bool IsParameter { get; set; }
        public int ParamIndex { get; set; }
        public int DownstreamStartIndex { get; set; }
        public int DownstreamsCount { get; set; }
    }
    
    private sealed class TempDownstream
    {
        public int FirstChildIndex { get; set; }
        public int ChildrenCount { get; set; }
        public byte MethodMask { get; set; }
    }

    private sealed class TempDownstreamSegment
    {
        public int PathStartIndex { get; set; }
        public int PathLength { get; set; }
        public bool IsParameter { get; set; }
        public int ParamIndex { get; set; }
    }
    
    private sealed class DataForSnapshot
    {
        public required TempClusterSegment[] TempClusterSegments { get; init; }
        public required char[] TempClusterPaths { get; init; }
        public required TempUpstreamSegment[] TempUpstreamSegments { get; init; }
        public required char[] TempUpstreamSegmentPaths { get; init; }
        public required TempDownstream[] TempDownstreams { get; init; }
        public required TempDownstreamSegment[] TempDownstreamSegments { get; init; }
        public required char[] TempDownstreamSegmentPaths { get; init; }
    }
}