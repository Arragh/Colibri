using System.Collections.Immutable;

namespace Colibri.Helpers;

/// <summary>
/// Global constants using across application
/// </summary>
public static class GlobalConstants
{
    /// <summary>
    /// Maximum count of clusters
    /// </summary>
    public const ushort ClustersMaxCount = 2000;
    
    /// <summary>
    /// Maximum count of routes
    /// </summary>
    public const ushort RoutesMaxCount = 2000;
    
    /// <summary>
    /// Maximum count of chars in a single segment
    /// </summary>
    public const int SegmentMaxLength = 250;
    
    /// <summary>
    /// Maximum count of parameters in a single route
    /// </summary>
    public const int ParamsMaxCount = 16;
    
    /// <summary>
    /// Maximum length of a single parameter in request
    /// </summary>
    public const ushort RequestParamMaxLength = 1000;
    
    /// <summary>
    /// Available protocols for TerminalMiddleware
    /// </summary>
    public static readonly ImmutableHashSet<string> Protocols =
    [
        "http",
        "ws"
    ];

    /// <summary>
    /// Available key algorithms for AuthorizationMiddleware
    /// </summary>
    public static readonly ImmutableHashSet<string> AuthAlgorithms =
    [
        "rs256",
        "hs256",
        "es256"
    ];
    
    /// <summary>
    /// Available http-methods for routes
    /// </summary>
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

    /// <summary>
    /// Available types for LoadBalancerMiddleware
    /// </summary>
    public static readonly ImmutableHashSet<string> LoadBalancerTypes =
    [
        "rr",
        "rnd"
    ];
}