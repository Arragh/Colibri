using Colibri.Helpers;

namespace Colibri.Runtime.Snapshots.Routing;

public class SnapshotDataBuilder
{
    public SnapshotData Build(TrieNode root)
    {
        List<TempUpstreamSegment> upstreamSegments = [];
        List<char> upstreamSegmentsPaths = [];
        
        List<TempDownstream> downstreams = [];
        List<TempDownstreamSegment> downstreamSegments = [];
        List<char> downstreamSegmentsPaths = [];

        FillTrieRecursively(root);

        void FillTrieRecursively(TrieNode root)
        {
            foreach (var child in root.Children)
            {
                var segmentName = '/' + child.Name;
                var upstreamSegment = new TempUpstreamSegment
                {
                    PathStartIndex = upstreamSegmentsPaths.Count,
                    PathLength = (byte)segmentName.Length,
                    IsParameter = child.IsParameter,
                    ParamIndex = (byte)child.ParamIndex
                };
                upstreamSegmentsPaths.AddRange(segmentName);
                upstreamSegments.Add(upstreamSegment);
            }

            var createdSegments = upstreamSegments
                .TakeLast(root.Children.Count)
                .ToArray();
            
            for (int i = 0; i < createdSegments.Length; i++)
            {
                if (root.Children[i].Methods.Count > 0)
                {
                    createdSegments[i].HasDownstream = true;
                    createdSegments[i].DownstreamStartIndex = (ushort)downstreams.Count;
                    createdSegments[i].DownstreamsCount = (byte)root.Children[i].Methods.Count;

                    foreach (var method in root.Children[i].Methods)
                    {
                        var chunksHolder = method.Value;
                        var tempDownstream = new TempDownstream
                        {
                            ClusterId = (ushort)chunksHolder.ClusterId,
                            MethodMask = HttpMethodMask.GetMask(method.Key),
                            FirstChildIndex = (ushort)downstreamSegments.Count,
                            ChildrenCount = (byte)chunksHolder.Chunks.Count
                        };
                        downstreams.Add(tempDownstream);

                        foreach (var chunk in chunksHolder.Chunks)
                        {
                            var segmentName = '/' + chunk.Name;
                            var downstreamSegment = new TempDownstreamSegment
                            {
                                PathStartIndex = (ushort)downstreamSegmentsPaths.Count,
                                PathLength = (byte)segmentName.Length,
                                IsParameter = chunk.IsParameter,
                                ParamIndex = (byte)chunk.ParamIndex
                            };
                            downstreamSegmentsPaths.AddRange(segmentName);
                            downstreamSegments.Add(downstreamSegment);
                        }
                    }
                }
                
                if (root.Children[i].Children.Count > 0)
                {
                    createdSegments[i].FirstChildIndex = (ushort)upstreamSegments.Count;
                    createdSegments[i].ChildrenCount = (ushort)root.Children[i].Children.Count;
                    FillTrieRecursively(root.Children[i]);
                }
            }
        }
        
        return new SnapshotData
        {
            TempUpstreamSegments = upstreamSegments.ToArray(),
            TempUpstreamSegmentPaths = upstreamSegmentsPaths.ToArray(),
            TempDownstreams = downstreams.ToArray(),
            TempDownstreamSegments = downstreamSegments.ToArray(),
            TempDownstreamSegmentPaths = downstreamSegmentsPaths.ToArray()
        };
    }
}

public sealed class TempUpstreamSegment
{
    public int PathStartIndex { get; set; }
    public byte PathLength { get; set; }
    public ushort FirstChildIndex { get; set; }
    public ushort ChildrenCount { get; set; }
    public bool IsParameter { get; set; }
    public byte ParamIndex { get; set; }
    public bool HasDownstream { get; set; }
    public ushort DownstreamStartIndex { get; set; }
    public byte DownstreamsCount { get; set; }
}

public sealed class TempDownstream
{
    public ushort ClusterId { get; set; }
    public ushort FirstChildIndex { get; set; }
    public byte ChildrenCount { get; set; }
    public byte MethodMask { get; set; }
}

public sealed class TempDownstreamSegment
{
    public int PathStartIndex { get; set; }
    public byte PathLength { get; set; }
    public bool IsParameter { get; set; }
    public byte ParamIndex { get; set; }
}

public sealed class SnapshotData
{
    public required TempUpstreamSegment[] TempUpstreamSegments { get; init; }
    public required char[] TempUpstreamSegmentPaths { get; init; }
    public required TempDownstream[] TempDownstreams { get; init; }
    public required TempDownstreamSegment[] TempDownstreamSegments { get; init; }
    public required char[] TempDownstreamSegmentPaths { get; init; }
}