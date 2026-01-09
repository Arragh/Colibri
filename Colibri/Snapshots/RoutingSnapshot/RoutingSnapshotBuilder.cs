using Colibri.Configuration;
using Colibri.Helpers;
using Colibri.Snapshots.RoutingSnapshot.Models;

namespace Colibri.Snapshots.RoutingSnapshot;

public class RoutingSnapshotBuilder
{
    public RoutingSnapshot Build(RoutingSettings settings)
    {
        var tempClusters = CreateTempClusters(settings);
        var downstreamChunksPath = CreateTempDownstreamChunks(tempClusters);
        var hosts = CreateHosts(tempClusters);
        var trie = CreateTrie(tempClusters, hosts);
        SortTrieRecursively(trie);
        var result1 = CreateTempSegments(trie);
        var result2 = CreateTempDownstreams(result1.tempSegments);
        
        var snapshot = CreateSnapshot(
            result1.tempSegments,
            result2.tempDownstreams,
            result1.segmentNames,
            result2.downstreamPaths,
            trie.ChildrenSegments.Count,
            hosts);

        return snapshot;
    }

    private List<TempCluster> CreateTempClusters(RoutingSettings settings)
    {
        var clusters = new List<TempCluster>();

        foreach (var cluster in settings.Clusters)
        {
            clusters.Add(new TempCluster
            {
                Protocol = cluster.Protocol,
                Hosts = cluster.Hosts,
                Routes = cluster.Routes.Select(r => new TempRoute
                {
                    Method = r.Method,
                    UpstreamSegments = r.UpstreamPattern.Split('/', StringSplitOptions.RemoveEmptyEntries),
                    DownstreamPattern = r.DownstreamPattern
                }).ToArray()
            });
        }
        
        return clusters;
    }

    private string CreateTempDownstreamChunks(List<TempCluster> clusters)
    {
        var downstreamChunksPath = string.Empty;
        
        foreach (var cluster in clusters)
        {
            foreach (var route in cluster.Routes)
            {
                var upstreamParams = route.UpstreamSegments
                    .Where(s => s.StartsWith('{') && s.EndsWith('}'))
                    .ToArray();
                var paramsMap = new Dictionary<string, byte>();

                for (int i = 0; i < upstreamParams.Length; i++)
                {
                    paramsMap.Add(upstreamParams[i], (byte)i);
                }
            
                var downstreamSegments = route.DownstreamPattern
                    .Split('/', StringSplitOptions.RemoveEmptyEntries);

                foreach (var segment in downstreamSegments)
                {
                    var downstreamChunk = new TempDownstreamChunk
                    {
                        Name = '/' + segment,
                        PathStartIndex = downstreamChunksPath.Length,
                        PathLength = (byte)(segment.Length + 1)
                    };
            
                    downstreamChunksPath += downstreamChunk.Name;
                
                    if (segment.StartsWith('{') && segment.EndsWith('}'))
                    {
                        downstreamChunk.IsParameter = true;
                        downstreamChunk.ParamIndex = paramsMap[segment];
                    }
                
                    route.TempDownstreamChunks.Add(downstreamChunk);
                }
            }
        }
        
        return downstreamChunksPath;
    }
    
    private List<string> CreateHosts(List<TempCluster> clustersData)
    {
        var hosts = new List<string>();
        
        foreach (var cluster in clustersData)
        {
            hosts.AddRange(cluster.Hosts);
        }
        
        return hosts;
    }
    
    private TrieNode CreateTrie(List<TempCluster> clustersData, List<string> hosts)
    {
        var root = new TrieNode();
        
        foreach (var cluster in clustersData)
        {
            foreach (var route in cluster.Routes)
            {
                fillTrieRecursively(route, root, cluster.Hosts);
            }
        }
        
        return root;
        
        void fillTrieRecursively(TempRoute route, TrieNode root, string[] clusterHosts)
        {
            var segment = root.ChildrenSegments
                .FirstOrDefault(t => t.SegmentName == route.UpstreamSegments[0]);
        
            if (segment == null)
            {
                segment = new TrieNode
                {
                    SegmentName = route.UpstreamSegments[0]
                };

                if (route.UpstreamSegments[0].StartsWith('{')
                    && route.UpstreamSegments[0].EndsWith('}'))
                {
                    segment.IsParameter = true;
                }
            
                root.ChildrenSegments.Add(segment);
            }
        
            route.UpstreamSegments = route.UpstreamSegments[1..];
        
            if (route.UpstreamSegments.Length == 0)
            {
                var indexes = new List<int>();
                for (int i = 0; i < hosts.Count; i++)
                {
                    if (clusterHosts.Contains(hosts[i]))
                    {
                        indexes.Add(i);
                    }
                }

                if (indexes.Count > 0)
                {
                    segment.HostStartIndex = indexes[0];
                    segment.HostsCount = indexes.Count;
                }
            
                segment.Methods.Add(route.Method, route.DownstreamPattern);
            
                return;
            }
        
            fillTrieRecursively(route, segment, clusterHosts);
        }
    }
    
    private void SortTrieRecursively(TrieNode segment)
    {
        segment.ChildrenSegments = segment.ChildrenSegments
            .OrderBy(s => s.IsParameter)
            .ThenByDescending(s => s.SegmentName!.Length)
            .ToList();

        foreach (var child in segment.ChildrenSegments)
        {
            SortTrieRecursively(child);
        }
    }

    private (List<TempSegment> tempSegments, string segmentNames) CreateTempSegments(TrieNode node)
    {
        var tempSegments = new List<TempSegment>();
        var segmentNames = string.Empty;
        
        сreateTempSegmentsRecursively(node);
        Console.WriteLine();

        void сreateTempSegmentsRecursively(TrieNode node)
        {
            foreach (var child in node.ChildrenSegments)
            {
                var tempSegment = new TempSegment();
            
                tempSegment.PathStartIndex = segmentNames.Length;
                segmentNames += '/' + child.SegmentName;
                tempSegment.PathLength = (short)(segmentNames.Length - tempSegment.PathStartIndex);
                tempSegment.Methods = child.Methods;
                tempSegment.HostStartIndex = (short)child.HostStartIndex;
                tempSegment.HostsCount = (byte)child.HostsCount;
                
                tempSegments.Add(tempSegment);
            }
            
            var justCreatedTempSegments = tempSegments
                .TakeLast(node.ChildrenSegments.Count)
                .ToList();

            for (int i = 0; i < node.ChildrenSegments.Count; i++)
            {
                justCreatedTempSegments[i].ChildrenCount = (short)node.ChildrenSegments[i].ChildrenSegments.Count;

                if (justCreatedTempSegments[i].ChildrenCount > 0)
                {
                    justCreatedTempSegments[i].FirstChildIndex = tempSegments.Count;
                }
                
                сreateTempSegmentsRecursively(node.ChildrenSegments[i]);
            }
        }
        
        return (tempSegments, segmentNames);
    }

    private (List<TempDownstream> tempDownstreams, string downstreamPaths) CreateTempDownstreams(List<TempSegment> tempSegments)
    {
        var downstreamPaths = string.Empty;
        var tempDownstreams = new List<TempDownstream>();
        
        foreach (var tempSegment in tempSegments)
        {
            foreach (var method in tempSegment.Methods)
            {
                var tempDownstream = new TempDownstream
                {
                    PathStartIndex = downstreamPaths.Length,
                    PathLength = (short)method.Value.Length,
                    MethodMask = HttpMethodMask.GetMask(method.Key),
                    HostStartIndex = (short)tempSegment.HostStartIndex,
                    HostsCount = (byte)tempSegment.HostsCount
                };
                
                downstreamPaths += method.Value;
                
                tempDownstreams.Add(tempDownstream);
                
                tempSegment.DownstreamStartIndex = (short)(tempDownstreams.Count - 1);
                tempSegment.DownstreamCount++;
                tempSegment.MethodMask |= HttpMethodMask.GetMask(method.Key);
            }
        }
        
        return (tempDownstreams, downstreamPaths);
    }
    
    private RoutingSnapshot CreateSnapshot(
        List<TempSegment> tempSegments,
        List<TempDownstream> tempDownstreams,
        string segmentNames,
        string downstreamPaths,
        int rootSegmentsCount,
        List<string> hosts)
    {
        var segments = new Segment[tempSegments.Count];
        var downstreams = new Downstream[tempDownstreams.Count];

        for (int i = 0; i < tempSegments.Count; i++)
        {
            segments[i] = new Segment(
                tempSegments[i].PathStartIndex,
                tempSegments[i].PathLength,
                tempSegments[i].FirstChildIndex,
                tempSegments[i].ChildrenCount,
                tempSegments[i].DownstreamStartIndex,
                tempSegments[i].DownstreamCount,
                tempSegments[i].MethodMask);
        }

        for (int i = 0; i < tempDownstreams.Count; i++)
        {
            downstreams[i] = new Downstream(
                tempDownstreams[i].PathStartIndex,
                tempDownstreams[i].PathLength,
                tempDownstreams[i].MethodMask,
                tempDownstreams[i].HostStartIndex,
                tempDownstreams[i].HostsCount);
        }

        var routingSnapshot = new RoutingSnapshot(
            rootSegmentsCount,
            segments,
            segmentNames.ToArray(),
            downstreams,
            downstreamPaths.ToArray(),
            hosts.Select(h => new Uri(h)).ToArray());
        
        return routingSnapshot;
    }
}