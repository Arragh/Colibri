using System.Text;
using Colibri.Runtime.Snapshots.Routing;

namespace Colibri.Runtime.Pipeline.RoutingEngine;

public sealed class DownstreamPathBuilder
{
    public string Build(
        RoutingSnapshot routingSnapshot,
        ReadOnlySpan<char> path,
        ParamValue[] routeParams,
        ushort downstreamFirstChildIndex,
        byte downstreamChildrenCount)
    {
        var segments = routingSnapshot.DownstreamSegments
            .Slice(downstreamFirstChildIndex, downstreamChildrenCount);

        var downstreamSegmentPaths = routingSnapshot.DownstreamSegmentPaths;

        var stringBuilder = new StringBuilder();

        for (int i = 0; i < segments.Length; i++)
        {
            ref readonly var segment = ref segments[i];

            var pathChunk = segment.IsParameter
                ? path.Slice(routeParams[segment.ParamIndex].Start, routeParams[segment.ParamIndex].Length)
                : downstreamSegmentPaths.Slice(segment.PathStartIndex, segment.PathLength);
            
            stringBuilder.Append(pathChunk);
        }

        return stringBuilder.ToString();
    }
}