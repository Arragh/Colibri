using Colibri.Services.RoutingEngine.Interfaces;
using Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

namespace Colibri.Services.RoutingEngine;

public sealed class RoutingEngine : IRoutingEngine
{
    public bool TryMatch(
        RoutingSnapshot snapshot,
        ReadOnlySpan<char> requestPath,
        byte methodMask,
        out Downstream? result)
    {
        var rootSegmentsCount = snapshot.RootSegmentsCount;
        var segments = snapshot.Segments;
        var segmentNames = snapshot.SegmentNames;
        var downstreams = snapshot.Downstreams;

        for (int i = 0; i < rootSegmentsCount; i++)
        {
            int requestPathIndex = 0;

            // while (true)
            int trololo = i + 1;
            for (int j = i; j < trololo; j++)
            {
                int requestPathIndexBackup = requestPathIndex;
                
                ref readonly var segment = ref segments[j];
                
                int start = requestPathIndex;
                requestPathIndex++;

                while (requestPathIndex < requestPath.Length
                       && requestPath[requestPathIndex] != '/')
                {
                    requestPathIndex++;
                }
                
                var requestSpan = requestPath
                    .Slice(start, requestPathIndex - start);
                
                var segmentSpan = snapshot.SegmentNames
                    .Slice(segment.PathStartIndex, segment.PathLength);

                if (IsParameterPattern(segmentSpan))
                {
                    // TODO: Написать логику обработки параметра из requestSegment
                }
                
                if (!segmentSpan.SequenceEqual(requestSpan))
                {
                    requestPathIndex = requestPathIndexBackup;
                    continue;
                }

                if (segment.ChildrenCount > 0)
                {
                    // var childrenSpan = snapshot.Segments
                    //     .Slice(segment.FirstChildIndex, segment.ChildrenCount);

                    j = segment.FirstChildIndex - 1;
                    trololo = segment.FirstChildIndex + segment.ChildrenCount;
                }
            }
        }

        Console.WriteLine();
        throw new NotImplementedException();
    }

    private bool IsParameterPattern(ReadOnlySpan<char> segmentName)
    {
        if (segmentName[1] == '{'
            && segmentName[^1] == '}')
        {
            return true;
        }

        return false;
    }
}