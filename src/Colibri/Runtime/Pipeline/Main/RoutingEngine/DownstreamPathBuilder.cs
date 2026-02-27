using System.Text;
using Colibri.Runtime.Snapshots.Routing;

namespace Colibri.Runtime.Pipeline.Main.RoutingEngine;

public sealed class DownstreamPathBuilder
{
    public string Build(
        RoutingSnapshot routingSnapshot,
        ReadOnlySpan<char> normalizedPath,
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
                ? normalizedPath.Slice(routeParams[segment.ParamIndex].Start, routeParams[segment.ParamIndex].Length)
                : downstreamSegmentPaths.Slice(segment.PathStartIndex, segment.PathLength);
            
            stringBuilder.Append(pathChunk);
        }

        return stringBuilder.ToString();
    }
}