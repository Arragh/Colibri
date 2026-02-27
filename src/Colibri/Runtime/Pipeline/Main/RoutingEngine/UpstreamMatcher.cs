using Colibri.Runtime.Snapshots.Routing;

namespace Colibri.Runtime.Pipeline.Main.RoutingEngine;

public sealed class UpstreamMatcher
{
    private const int MaxParamLength = 1000;
    
    public bool TryMatch(
        RoutingSnapshot routingSnapshot,
        ReadOnlySpan<char> normalizedPath,
        byte methodMask,
        int rootSegmentsCount,
        out ushort clusterId,
        out ParamValue[] routeParams,
        out ushort downstreamFirstChildIndex,
        out byte downstreamChildrenCount)
    {
        var upstreams = routingSnapshot.UpstreamSegments;
        var upstreamPaths = routingSnapshot.UpstreamSegmentPaths;

        var start = 0;
        var limiter = rootSegmentsCount;
        
        var localPath = normalizedPath;
        var totalSlice = 0;
        
        routeParams = new ParamValue[16];
        clusterId = 0;
        downstreamFirstChildIndex = 0;
        downstreamChildrenCount = 0;

        while (start < limiter)
        {
            bool matched = false;
            
            for (int i = start; i < limiter; i++)
            {
                ref readonly var upstreamSegment = ref upstreams[i];
                
                var segmentPath = upstreamPaths
                    .Slice(upstreamSegment.PathStartIndex, upstreamSegment.PathLength);

                if (!upstreamSegment.IsParameter)
                {
                    if (localPath.StartsWith(segmentPath))
                    {
                        if (localPath.Length > segmentPath.Length)
                        {
                            if (localPath[segmentPath.Length] == '/')
                            {
                                start = upstreamSegment.FirstChildIndex;
                                limiter = upstreamSegment.FirstChildIndex + upstreamSegment.ChildrenCount;
                                localPath = localPath[segmentPath.Length..];
                                matched = true;
                                totalSlice += segmentPath.Length;
                                break;
                            }
                        }
                        else if (upstreamSegment.HasDownstream)
                        {
                            var downstreams = routingSnapshot.Downstreams
                                .Slice(upstreamSegment.DownstreamStartIndex, upstreamSegment.DownstreamsCount);

                            foreach (var downstream in downstreams)
                            {
                                if (downstream.MethodMask == methodMask)
                                {
                                    clusterId = downstream.ClusterId;
                                    downstreamFirstChildIndex = downstream.FirstChildIndex;
                                    downstreamChildrenCount = downstream.ChildrenCount;
                                    return true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    int paramStart = totalSlice;
                    ushort paramCount = 0;

                    for (int j = paramStart; j < normalizedPath.Length; j++)
                    {
                        if (paramCount > MaxParamLength)
                        {
                            return false;
                        }

                        char c = normalizedPath[j];
                        
                        if (j == paramStart && c == '/')
                        {
                            paramCount++;
                            continue;
                        }
                        
                        if (c != '/')
                        {
                            paramCount++;
                            continue;
                        }

                        break;
                    }
                    
                    routeParams[upstreamSegment.ParamIndex] = new ParamValue(paramStart, paramCount);
                    
                    start = upstreamSegment.FirstChildIndex;
                    limiter = upstreamSegment.FirstChildIndex + upstreamSegment.ChildrenCount;
                    localPath = localPath[paramCount..];
                    matched = true;
                    totalSlice += paramCount;

                    if (localPath.Length == 0
                        && upstreamSegment.HasDownstream)
                    {
                        var downstreams = routingSnapshot.Downstreams
                            .Slice(upstreamSegment.DownstreamStartIndex, upstreamSegment.DownstreamsCount);

                        foreach (var downstream in downstreams)
                        {
                            if (downstream.MethodMask == methodMask)
                            {
                                clusterId = downstream.ClusterId;
                                downstreamFirstChildIndex = downstream.FirstChildIndex;
                                downstreamChildrenCount = downstream.ChildrenCount;
                                return true;
                            }
                        }
                    }

                    break;
                }
            }
            
            if (!matched)
            {
                return false;
            }
        }

        return false;
    }
}