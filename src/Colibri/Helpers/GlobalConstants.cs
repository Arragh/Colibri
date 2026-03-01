using System.Collections.Immutable;

namespace Colibri.Helpers;

public static class GlobalConstants
{
    public const ushort ClustersMaxCount = 2000;
    public const ushort RoutesMaxCount = 2000;
    public const int SegmentMaxLength = 250;
    public const int ParamsMaxCount = 16;
    
    public static readonly ImmutableHashSet<string> Protocols =
    [
        "http",
        "ws"
    ];

    public static readonly ImmutableHashSet<string> AuthAlgorithms =
    [
        "rs256",
        "hs256",
        "es256"
    ];
    
    public static readonly ImmutableHashSet<string> HttpMethods =
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

    public static readonly ImmutableHashSet<string> LoadBalancerTypes =
    [
        "rr",
        "rnd"
    ];
}