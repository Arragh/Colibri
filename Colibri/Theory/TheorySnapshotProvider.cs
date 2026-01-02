using System.Data;
using Colibri.Configuration;
using Colibri.Theory.Models;
using Colibri.Theory.Structs;
using Microsoft.Extensions.Options;

namespace Colibri.Theory;

public class TheorySnapshotProvider
{
    private readonly Dictionary<string, TempSegment> _root = new();
    private readonly char[] _chars;
    private readonly Segment[] _segments;

    public TheorySnapshotProvider(IOptionsMonitor<RoutingSettings> monitor)
    {
        var lol = CreateTrie(monitor.CurrentValue);

        _chars = new char[lol.Item1];
        _segments = new Segment[lol.Item2];

        Console.WriteLine();
    }

    private (int, int) CreateTrie(RoutingSettings settings)
    {
        int charsLenght = 0;
        int segmentsCount = 0;
        
        foreach (var cluster in settings.Clusters)
        {
            foreach (var route in cluster.Routes)
            {
                var upstreamSegmentsArray = route.UpstreamPattern.Split('/', StringSplitOptions.RemoveEmptyEntries);
                CreateTrieRecursively(
                    route.Method,
                    upstreamSegmentsArray,
                    route.DownstreamPattern,
                    _root,
                    ref charsLenght,
                    ref segmentsCount);
            }
        }

        return (charsLenght, segmentsCount);
    }

    private void CreateTrieRecursively(
        string method,
        string[] segments,
        string downStreamPattern,
        Dictionary<string, TempSegment> root,
        ref int charsLenght,
        ref int segmentsCount)
    {
        if (!root.ContainsKey(segments[0]))
        {
            root[segments[0]] = new TempSegment
            {
                SegmentName = segments[0]
            };
            
            charsLenght += segments[0].Length;
            segmentsCount++;
        }
        
        if (segments.Length > 1)
        {
            CreateTrieRecursively(
                method,
                segments.Skip(1).ToArray(),
                downStreamPattern,
                root[segments[0]].IncludedSegments,
                ref charsLenght,
                ref segmentsCount);
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
}