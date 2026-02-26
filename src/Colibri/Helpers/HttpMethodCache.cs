using System.Runtime.CompilerServices;

namespace Colibri.Helpers;

internal static class HttpMethodCache
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HttpMethod Get(string method)
    {
        return method switch
        {
            "GET" => HttpMethod.Get,
            "POST" => HttpMethod.Post,
            "PUT" => HttpMethod.Put,
            "DELETE" => HttpMethod.Delete,
            "HEAD" => HttpMethod.Head,
            "OPTIONS" => HttpMethod.Options,
            "TRACE" => HttpMethod.Trace,
            "PATCH" => HttpMethod.Patch,
            _ => new HttpMethod(method)
        };
    }
}