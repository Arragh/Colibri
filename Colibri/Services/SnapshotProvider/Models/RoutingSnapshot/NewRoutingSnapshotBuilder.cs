using Colibri.Configuration;
using Colibri.Helpers;

namespace Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

public class NewRoutingSnapshotBuilder
{
    public void Build(RoutingSettings settings)
    {
        var clustersData = PrepareDataForTrie(settings);
        var hosts = CreateHosts(clustersData);
        var trie = CreateTrie(clustersData, hosts);
        SortTrieRecursively(trie);

        var lol = CreateTempSegments(trie);

        Console.WriteLine();
    }

    private List<NewTempCluster> PrepareDataForTrie(RoutingSettings settings)
    {
        var clusters = new List<NewTempCluster>();

        foreach (var cluster in settings.Clusters)
        {
            clusters.Add(new NewTempCluster
            {
                Protocol = cluster.Protocol,
                Hosts = cluster.Hosts,
                Routes = cluster.Routes.Select(r => new NewTempRoute
                {
                    Method = r.Method,
                    Upstream = r.UpstreamPattern.Split('/', StringSplitOptions.RemoveEmptyEntries),
                    Downstream = r.DownstreamPattern
                }).ToArray()
            });
        }
        
        return clusters;
    }

    private List<string> CreateHosts(List<NewTempCluster> clustersData)
    {
        var hosts = new List<string>();
        
        foreach (var cluster in clustersData)
        {
            hosts.AddRange(cluster.Hosts);
        }
        
        return hosts;
    }
    
    private NewTrieNode CreateTrie(List<NewTempCluster> clustersData, List<string> hosts)
    {
        var root = new NewTrieNode();
        
        foreach (var cluster in clustersData)
        {
            foreach (var route in cluster.Routes)
            {
                fillTrieRecursively(route, root, cluster.Hosts);
            }
        }
        
        return root;
        
        void fillTrieRecursively(NewTempRoute route, NewTrieNode root, string[] clusterHosts)
        {
            var segment = root.ChildrenSegments
                .FirstOrDefault(t => t.SegmentName == route.Upstream[0]);
        
            if (segment == null)
            {
                segment = new NewTrieNode
                {
                    SegmentName = route.Upstream[0]
                };

                if (route.Upstream[0].StartsWith('{')
                    && route.Upstream[0].EndsWith('}'))
                {
                    segment.IsParameter = true;
                }
            
                root.ChildrenSegments.Add(segment);
            }
        
            route.Upstream = route.Upstream[1..];
        
            if (route.Upstream.Length == 0)
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
            
                segment.Methods.Add(route.Method, route.Downstream);
            
                return;
            }
        
            fillTrieRecursively(route, segment, clusterHosts);
        }
    }
    
    private void SortTrieRecursively(NewTrieNode segment)
    {
        segment.ChildrenSegments = segment.ChildrenSegments
            .OrderBy(s => s.IsParameter)
            .ToList();

        foreach (var child in segment.ChildrenSegments)
        {
            SortTrieRecursively(child);
        }
    }

    private (List<TempSegment>, List<TempDownstream>) CreateTempSegments(NewTrieNode root)
    {
        var tempSegments = new List<TempSegment>();
        string segmentNames = string.Empty;
        
        var tempDownstreams = new List<TempDownstream>();
        string downstreamPaths = string.Empty;

        createTempSegmentsRecursively(
            tempSegments,
            tempDownstreams,
            root,
            ref segmentNames,
            ref downstreamPaths);
        
        return (tempSegments, tempDownstreams);
        
        void createTempSegmentsRecursively(
            List<TempSegment> tempSegments,
            List<TempDownstream> tempDownstreams,
            NewTrieNode node,
            ref string segmentNames,
            ref string downstreamPaths)
        {
            var tempSegment = new TempSegment();

            if (node.SegmentName != null)
            {
                tempSegment.PathStartIndex = segmentNames.Length;
                tempSegment.PathLength = (short)node.SegmentName.Length;
                segmentNames += '/' + node.SegmentName;
                
                tempSegments.Add(tempSegment);
                
                if (node.ChildrenSegments.Count > 0)
                {
                    tempSegment.FirstChildIndex = tempSegments.Count;
                    tempSegment.ChildrenCount = (short)node.ChildrenSegments.Count;
                }
                
                foreach (var method in  node.Methods)
                {
                    var tempDownstream = new TempDownstream();
                
                    tempDownstream.PathStartIndex = downstreamPaths.Length;
                    tempDownstream.PathLength = (short)method.Value.Length;
                    downstreamPaths += method.Value;
                    tempDownstream.MethodMask = HttpMethodMask.GetMask(method.Key);
                    tempDownstream.HostStartIndex = (short)node.HostStartIndex;
                    tempDownstream.HostsCount = (byte)node.HostsCount;
                    
                    tempDownstreams.Add(tempDownstream);

                    tempSegment.DownstreamStartIndex = (short)tempDownstreams.Count;
                    tempSegment.DownstreamCount++;
                    tempSegment.MethodMask |= HttpMethodMask.GetMask(method.Key);
                }
            }

            foreach (var child in node.ChildrenSegments)
            {
                createTempSegmentsRecursively(
                    tempSegments,
                    tempDownstreams,
                    child,
                    ref segmentNames,
                    ref downstreamPaths);
            }
        }
    }
    
}