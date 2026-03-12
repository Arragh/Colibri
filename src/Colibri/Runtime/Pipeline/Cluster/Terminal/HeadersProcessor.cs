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

    public void CopyRequestHeaders(HttpRequest source, HttpRequestMessage target)
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
            
            if (header.Value.Count == 1)
            {
                if (!target.Headers.TryAddWithoutValidation(header.Key, header.Value[0]))
                {
                    target.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value[0]);
                }
            }
            else
            {
                var headerAsEnum = header.Value.AsEnumerable();
                if (!target.Headers.TryAddWithoutValidation(header.Key, headerAsEnum))
                {
                    target.Content?.Headers.TryAddWithoutValidation(header.Key, headerAsEnum);
                }
            }
        }
    }

    public void CopyResponseHeaders(HttpResponseMessage source, HttpResponse target)
    {
        foreach (var header in source.Headers)
        {
            if (IsHopByHopHeader(header.Key.AsSpan()))
            {
                continue;
            }
            
            target.Headers[header.Key] = header.Value.ToArray();
        }
        
        foreach (var header in source.Content.Headers)
        {
            if (IsHopByHopHeader(header.Key.AsSpan()))
            {
                continue;
            }
            
            target.Headers[header.Key] = header.Value.ToArray();
        }
    }
    
    private static bool IsHopByHopHeader(ReadOnlySpan<char> header)
    {
        return header.Length switch
        {
            2  => header.Equals("TE", StringComparison.OrdinalIgnoreCase),
            7  => header.Equals("Upgrade", StringComparison.OrdinalIgnoreCase)
                  || header.Equals("Trailer", StringComparison.OrdinalIgnoreCase),
            10 => header.Equals("Connection", StringComparison.OrdinalIgnoreCase)
                  || header.Equals("Keep-Alive", StringComparison.OrdinalIgnoreCase),
            17 => header.Equals("Transfer-Encoding", StringComparison.OrdinalIgnoreCase),
            18 => header.Equals("Proxy-Authenticate", StringComparison.OrdinalIgnoreCase),
            19 => header.Equals("Proxy-Authorization", StringComparison.OrdinalIgnoreCase),
            _  => false
        };
    }
}