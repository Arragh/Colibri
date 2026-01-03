using System.Data;
using Colibri.Configuration;
using Colibri.Theory.Models;
using Colibri.Theory.Structs;
using Microsoft.Extensions.Options;

namespace Colibri.Theory;

public class TheorySnapshotProvider
{
    private readonly Dictionary<string, SegmentNode> _root = new();
    private readonly char[] _paths;
    private readonly Segment[] _segments;

    public TheorySnapshotProvider(IOptionsMonitor<RoutingSettings> monitor)
    {
        var trieDataCounts = CreateTrie(monitor.CurrentValue);

        _paths = new char[trieDataCounts.Item1];
        _segments = new Segment[trieDataCounts.Item2];
        
        FillDataArrays(_paths, _segments);

        Console.WriteLine();
    }

    private void FillDataArrays(char[] paths, Segment[] segments)
    {
        var pathStartIndex = 0;
        var segmentIndex = 0;
        var tempSegments = new TempSegment[segments.Length];

        CreateTempSegmentsRecursively(
            ref pathStartIndex,
            ref segmentIndex,
            paths,
            tempSegments,
            _root.Values.ToArray());

        for (int i = 0; i < tempSegments.Length; i++)
        {
            _segments[i] = new Segment(
                tempSegments[i].PathStartIndex,
                tempSegments[i].PathLength,
                tempSegments[i].FirstChildIndex,
                tempSegments[i].ChildrenCount);
        }
    }
    
    private int? CreateTempSegmentsRecursively(
        ref int pathStartIndex,
        ref int segmentIndex,
        char[] paths,
        TempSegment[] tempSegments,
        SegmentNode[] segmentsArray)
    {
        int? firstChildIndex = null;

        for (int i = 0; i < segmentsArray.Length; i++)
        {
            tempSegments[segmentIndex] = new TempSegment
            {
                PathStartIndex = pathStartIndex,
                ChildrenCount = segmentsArray[i].IncludedSegments.Count,
                PathLength = segmentsArray[i].SegmentName.Length
            };

            if (firstChildIndex == null)
            {
                firstChildIndex = segmentIndex;
            }
            
            foreach (var c in segmentsArray[i].SegmentName)
            {
                paths[pathStartIndex++] = c;
            }
            
            segmentIndex++;
        }
        
        for (int i = 0; i < segmentsArray.Length; i++)
        {
            var temp = CreateTempSegmentsRecursively(
                ref pathStartIndex,
                ref segmentIndex,
                paths,
                tempSegments,
                segmentsArray[i].IncludedSegments.Values.ToArray());

            if (temp != null)
            {
                tempSegments[firstChildIndex!.Value + i].FirstChildIndex = temp.Value;
            }
        }

        return firstChildIndex;
    }

    private (int, int) CreateTrie(RoutingSettings settings)
    {
        int charsCount = 0;
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
                    ref charsCount,
                    ref segmentsCount);
            }
        }

        return (charsCount, segmentsCount);
    }

    private void CreateTrieRecursively(
        string method,
        string[] segments,
        string downStreamPattern,
        Dictionary<string, SegmentNode> root,
        ref int charsLenght,
        ref int segmentsCount)
    {
        if (!root.ContainsKey(segments[0]))
        {
            root[segments[0]] = new SegmentNode
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