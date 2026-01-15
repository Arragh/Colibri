using Colibri.Runtime.Snapshots.Routing;

namespace Colibri.Runtime.Pipeline.RoutingEngine;

public sealed class UpstreamMatcher
{
    public bool TryMatch(
        RoutingSnapshot routingSnapshot,
        ReadOnlySpan<char> path,
        byte methodMask,
        ushort firstUpstreamIndex,
        ushort upstreamsCount,
        out ParamValue[] routeParams,
        out ushort downstreamFirstChildIndex,
        out byte downstreamChildrenCount)
    {
        var upstreams = routingSnapshot.UpstreamSegments;
        var upstreamPaths = routingSnapshot.UpstreamSegmentPaths;

        var start = firstUpstreamIndex;
        var limiter = firstUpstreamIndex + upstreamsCount;
        
        var localPath = path;
        var totalSlice = 0;
        
        routeParams = new ParamValue[16];
        
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
                        matched = true;
                            
                        if (localPath.Length > segmentPath.Length
                            && localPath[segmentPath.Length] == '/')
                        {
                            start = upstreamSegment.FirstChildIndex;
                            limiter = upstreamSegment.FirstChildIndex + upstreamSegment.ChildrenCount;
                            localPath = localPath[segmentPath.Length..];
                            totalSlice += segmentPath.Length;
                            break;
                        }

                        if (localPath.Length == segmentPath.Length)
                        {
                            ref readonly var downstream = ref routingSnapshot.Downstreams[upstreamSegment.DownstreamIndex];

                            if ((downstream.MethodMask & methodMask) != 0)
                            {
                                downstreamFirstChildIndex = downstream.FirstChildIndex;
                                downstreamChildrenCount = downstream.ChildrenCount;
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    matched = true;
                    
                    var paramStart = totalSlice;
                    byte paramCount = 0;

                    for (int j = paramStart; j < path.Length; j++)
                    {
                        if (j == paramStart && path[j] == '/')
                        {
                            paramCount++;
                            continue;
                        }
                        
                        if (path[j] != '/')
                        {
                            paramCount++;
                            continue;
                        }
                        
                        break;
                    }
                    
                    var paramValue = path.Slice(paramStart, paramCount);

                    routeParams[upstreamSegment.ParamIndex] = new ParamValue((ushort)paramStart, paramCount);
                    
                    start = upstreamSegment.FirstChildIndex;
                    limiter = upstreamSegment.FirstChildIndex + upstreamSegment.ChildrenCount;
                    localPath = localPath[paramCount..];
                    totalSlice += paramCount;

                    if (localPath.Length == 0)
                    {
                        ref readonly var downstream = ref routingSnapshot.Downstreams[upstreamSegment.DownstreamIndex];

                        if ((downstream.MethodMask & methodMask) != 0)
                        {
                            downstreamFirstChildIndex = downstream.FirstChildIndex;
                            downstreamChildrenCount = downstream.ChildrenCount;
                            return true;
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