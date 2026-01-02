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

        MakePathAsCharsArray(monitor.CurrentValue);

        Console.WriteLine();
    }

    private void MakePathAsCharsArray(RoutingSettings settings)
    {
        foreach (var cluster in settings.Clusters)
        {
            foreach (var route in cluster.Routes)
            {
                var upstreamSegmentsArray = route.UpstreamPattern.Split('/', StringSplitOptions.RemoveEmptyEntries);
                CreateTrieRecursively(upstreamSegmentsArray, _root);
            }
        }

        Console.WriteLine(_root);
    }

    private void CreateTrieRecursively(string[] segments, Dictionary<string, TempSegment> root)
    {
        if (root.ContainsKey(segments[0]))
        {
            CreateTrieRecursively(segments.Skip(1).ToArray(), root[segments[0]].IncludedSegments!);
        }
        else
        {
            root[segments[0]] = new TempSegment
            {
                SegmentName = segments[0]
            };

            if (segments.Length > 1)
            {
                CreateTrieRecursively(segments.Skip(1).ToArray(), root[segments[0]].IncludedSegments);
            }
            else
            {
                root[segments[0]].IsEndpoint = true;
            }
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