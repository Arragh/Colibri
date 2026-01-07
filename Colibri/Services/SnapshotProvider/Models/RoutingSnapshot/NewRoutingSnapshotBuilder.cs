using Colibri.Configuration;

namespace Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

public class NewRoutingSnapshotBuilder
{
    private NewTrieSegment _root = new();
    private List<string> _hosts = new();
    
    public void Build(RoutingSettings settings)
    {
        var clusters = PrepareDataForTrie(settings);
        FillTrie(clusters);
        SortTrieRecursively(_root);

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

    private void FillTrie(List<NewTempCluster> clusters)
    {
        foreach (var cluster in clusters)
        {
            _hosts.AddRange(cluster.Hosts);
            
            foreach (var route in cluster.Routes)
            {
                fillTrieRecursively(route, _root, cluster.Hosts);
            }
        }
        
        void fillTrieRecursively(NewTempRoute route, NewTrieSegment root, string[] clusterHosts)
        {
            var segment = root.ChildrenSegments
                .FirstOrDefault(t => t.SegmentName == route.Upstream[0]);
        
            if (segment == null)
            {
                segment = new NewTrieSegment
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
                for (int i = 0; i < _hosts.Count; i++)
                {
                    if (clusterHosts.Contains(_hosts[i]))
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
    
    private void SortTrieRecursively(NewTrieSegment segment)
    {
        segment.ChildrenSegments = segment.ChildrenSegments
            .OrderBy(s => s.IsParameter)
            .ToList();

        foreach (var child in segment.ChildrenSegments)
        {
            SortTrieRecursively(child);
        }
    }
}