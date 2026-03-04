using Microsoft.AspNetCore.Http;

namespace Colibri.Runtime.Pipeline.Cluster.Terminal;

public sealed class HeadersProcessor
{
    private static readonly string[] HopByHopHeaders =
    [
        "Connection",
        "Keep-Alive",
        "Proxy-Authenticate",
        "Proxy-Authorization",
        "TE",
        "Trailer",
        "Transfer-Encoding",
        "Upgrade"
    ];

    public void CopyHeaders(IHeaderDictionary source, HttpRequestMessage target)
    {
        foreach (var header in source)
        {
            if (IsHopByHopHeader(header.Key.AsSpan()))
            {
                continue;
            }

            if (!target.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
            {
                target.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }
        }
    }
    
    private static bool IsHopByHopHeader(ReadOnlySpan<char> header)
    {
        foreach (var hipHop in HopByHopHeaders)
        {
            if (header.Length == hipHop.Length
                && header.StartsWith(hipHop, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        
        return false;
    }
}