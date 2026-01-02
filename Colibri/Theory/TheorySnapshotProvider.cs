using System.Data;
using Colibri.Configuration;
using Colibri.Theory.Models;
using Microsoft.Extensions.Options;

namespace Colibri.Theory;

public class TheorySnapshotProvider
{
    private TheorySnapshot _theorySnapshot;
    private readonly Dictionary<string, TempSegment> _root = new();

    public TheorySnapshotProvider(IOptionsMonitor<RoutingSettings> monitor)
    {
        var pathLenght = CountCharsLength(monitor.CurrentValue);
        // _theorySnapshot = new TheorySnapshot(1, 1);

        CreateTrie(monitor.CurrentValue);

        Console.WriteLine();
    }

    private void CreateTrie(RoutingSettings settings)
    {
        foreach (var cluster in settings.Clusters)
        {
            foreach (var route in cluster.Routes)
            {
                var upstreamSegmentsArray = route.UpstreamPattern.Split('/', StringSplitOptions.RemoveEmptyEntries);
                CreateTrieRecursively(
                    route.Method,
                    upstreamSegmentsArray,
                    route.DownstreamPattern,
                    _root);
            }
        }

        Console.WriteLine(_root);
    }

    private void CreateTrieRecursively(string method, string[] segments, string downStreamPattern, Dictionary<string, TempSegment> root)
    {
        if (!root.ContainsKey(segments[0]))
        {
            root[segments[0]] = new TempSegment
            {
                SegmentName = segments[0]
            };
        }
        
        if (segments.Length > 1)
        {
            CreateTrieRecursively(
                method,
                segments.Skip(1).ToArray(),
                downStreamPattern,
                root[segments[0]].IncludedSegments);
        }
        else
        {
            if (root[segments[0]].Methods.TryGetValue(method, out _))
            {
                throw new DuplicateNameException($"Duplicate method type: {method}");
            }
                
            root[segments[0]].Methods.Add(method, downStreamPattern);
        }
    }

    private char[] CountCharsLength(RoutingSettings settings)
    {
        List<char> charsList = new();

        foreach (var cluster in settings.Clusters)
        {
            foreach (var route in cluster.Routes)
            {
                var upstreamSegments = route.UpstreamPattern.Split('/', StringSplitOptions.RemoveEmptyEntries);

                foreach (var s in upstreamSegments)
                {
                    charsList.AddRange(s.ToCharArray());
                }
                
            }
        }

        return charsList.ToArray();
    }
}