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
            int currentSegmentIndex = 0;
            
            ref readonly var segment = ref segments[i];

            while (true)
            {
                if (!TryMatchSegment(requestPath, segmentNames, ref requestPathIndex, in segment))
                {
                    break;
                }

                if (segment.ChildrenCount > 0)
                {
                    for (int j = 0; j < segment.ChildrenCount; j++)
                    {
                        ref readonly var childSegment = ref segments[segment.FirstChildIndex + j];
                        
                        if (!TryMatchSegment(requestPath, segmentNames, ref requestPathIndex, in childSegment))
                        {
                            break;
                        }

                        if (IsFinalStepInPath(requestPath, requestPathIndex))
                        {
                            if (!TryMatchDownstream(downstreams, childSegment, methodMask, out result))
                            {
                                return false;
                            }

                            return true;
                        }
                    }
                }
                
                if (IsFinalStepInPath(requestPath, requestPathIndex))
                {
                    if (!TryMatchDownstream(downstreams, segment, methodMask, out result))
                    {
                        return false;
                    }

                    return true;
                }
            }
        }
        
        result = default;
        return false;
    }

    private bool TryMatchSegment(
        ReadOnlySpan<char> requestPath,
        ReadOnlySpan<char> segmentNames,
        ref int requestPathIndex,
        in Segment segment)
    {
        var segmentNameSpan = segmentNames.Slice(segment.PathStartIndex, segment.PathLength);

        if (segmentNameSpan[1] == '{'
            && segmentNameSpan[^1] == '}'
            && TryMatchParameterSegment(requestPath, ref requestPathIndex, out var parameter))
        {
            return true;
        }
        
        if (TryMatchStaticSegment(requestPath, segmentNameSpan, ref requestPathIndex))
        {
            return true;
        }

        return false;
    }

    private bool TryMatchStaticSegment(
        ReadOnlySpan<char> requestPath,
        ReadOnlySpan<char> segmentNameSpan,
        ref int requestPathIndex)
    {
        // Сразу отметаем лишнее, не совпадающее по длине
        if (requestPath.Length - requestPathIndex < segmentNameSpan.Length)
        {
            return false;
        }

        /*
         * Если оставшийся RequestPath длиннее той части, что мы будем проверять, то нужно убедиться,
         * что следующим символом за границами проверяемого сегмента пути будет '/',
         * иначе маршрут не верен. На случай, если RequestPath == "delete", а SegmentName = "del".
         * Без такой проверки произойдет мэтч, хотя по факту маршрут не верный.
         */
        if (requestPath.Length - requestPathIndex > segmentNameSpan.Length)
        {
            var nextChar = requestPath[requestPathIndex + segmentNameSpan.Length];
            
            if (nextChar != '/')
            {
                return false;
            }
        }
        
        // Если сегменты не совпадают, то сразу выходим из метода.
        var requestPathSlice  = requestPath.Slice(requestPathIndex, segmentNameSpan.Length);
        if (!requestPathSlice.SequenceEqual(segmentNameSpan))
        {
            return false;
        }
        
        requestPathIndex += segmentNameSpan.Length;

        return true;
    }

    private bool TryMatchParameterSegment(
        ReadOnlySpan<char> requestPath,
        ref int requestPathStartIndex,
        out ReadOnlySpan<char> parameter)
    {
        int start = requestPathStartIndex;
        
        while (requestPathStartIndex < requestPath.Length && requestPath[requestPathStartIndex] != '/')
        {
            requestPathStartIndex++;
        }
            
        parameter = requestPath.Slice(start, requestPathStartIndex - start);
        
        return parameter.Length > 0;
    }

    private bool IsFinalStepInPath(
        ReadOnlySpan<char> requestPath,
        int requestPathIndex)
    {
        return requestPathIndex == requestPath.Length;
    }

    private bool TryMatchDownstream(
        ReadOnlySpan<Downstream> downstreams,
        in Segment segment,
        byte methodMask,
        out Downstream? result)
    {
        if (segment.DownstreamCount > 0)
        {
            var segmentDownstreams = downstreams.Slice(segment.DownstreamStartIndex, segment.DownstreamCount);
                                
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