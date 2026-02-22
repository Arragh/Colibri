using System.Collections.Immutable;

namespace Colibri.Helpers;

internal static class GlobalConstants
{
    public const int SegmentMaxLength = 250;
    public const int ParamsMaxCount = 16;
    
    public static readonly ImmutableHashSet<string> ValidProtocols =
        [
            "http",
            "ws"
        ];
    
    public static readonly ImmutableHashSet<string> ValidHttpMethods =
        [
            "GET",
            "POST",
            "PUT",
            "PATCH",
            "DELETE",
            "HEAD",
            "OPTIONS",
            "TRACE"
        ];
}