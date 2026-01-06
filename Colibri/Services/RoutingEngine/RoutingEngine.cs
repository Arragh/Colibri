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
                
                var segmentSpan = segmentNames
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

                Console.WriteLine();

                if (IsFinalStep(requestPath, requestPathIndex))
                {
                    if (TryMatchDownstream(downstreams, methodMask, segment, out result))
                    {
                        return true;
                    }
                    
                    break;
                }

                if (segment.ChildrenCount > 0)
                {
                    j = segment.FirstChildIndex - 1;
                    trololo = segment.FirstChildIndex + segment.ChildrenCount;
                }
            }
        }

        result = null;
        return false;
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

    private bool IsFinalStep(ReadOnlySpan<char> requestPath, int requestPathIndex)
    {
        return requestPathIndex >= requestPath.Length;
    }

    private bool TryMatchDownstream(
        ReadOnlySpan<Downstream> downstreams,
        byte methodMask,
        in Segment segment,
        out Downstream? result)
    {
        if (segment.DownstreamCount > 0)
        {
            var segmentDownstreams = downstreams
                .Slice(segment.DownstreamStartIndex, segment.DownstreamCount);
        
            for (int k = 0; k < segmentDownstreams.Length; k++)
            {
                ref readonly var downstream = ref segmentDownstreams[k];
                if ((downstream.MethodMask | methodMask) != 0)
                {
                    result = downstream;
                    return true;
                }
            }
        }

        result = null;
        return false;
    }
}