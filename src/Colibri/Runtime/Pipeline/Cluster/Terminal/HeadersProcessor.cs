using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

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

    public void ProcessHeaders(HttpRequest source, HttpRequestMessage target)
    {
        var sourceHeaders = source.Headers;
        
        if (source.ContentLength > 0
            || !StringValues.IsNullOrEmpty(sourceHeaders["Transfer-Encoding"]))
        {
            target.Content = new StreamContent(source.Body);
        }
        
        foreach (var header in sourceHeaders)
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
        
        target.Headers.ExpectContinue = false;
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