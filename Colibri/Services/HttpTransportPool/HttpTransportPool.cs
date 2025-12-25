using System.Collections.Concurrent;
using System.Net;
using System.Runtime.CompilerServices;
using Colibri.Services.HttpTransportPool.Interfaces;

namespace Colibri.Services.HttpTransportPool;

public class HttpTransportPool : IHttpTransportPool
{
    private ConcurrentDictionary<string, HttpMessageInvoker?> _invokers = new();
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HttpMessageInvoker GetHttpInvoker(string baseUrl)
    {
        /*
         * Отдает транспорт для определенного кластера по ключу BaseUrl
         * Можно реализовать в стиле "GetOrCreate", когда, если нет транспорта, то создаем и отдаем.
         * После создания транспорта его обязательно где-то закэшировать по ключу "BaseUrl" для следующих запросов.
         */
        
        return GetOrCreate(baseUrl);
    }

    private HttpMessageInvoker GetOrCreate(string baseUrl)
    {
        if (!_invokers.TryGetValue(baseUrl, out var invoker))
        {
            var handler = new SocketsHttpHandler
            {
                MaxConnectionsPerServer = 2000,
                PooledConnectionIdleTimeout = TimeSpan.FromSeconds(30),
                PooledConnectionLifetime = TimeSpan.FromMinutes(2),
                ConnectTimeout = TimeSpan.FromSeconds(5),
                KeepAlivePingPolicy = HttpKeepAlivePingPolicy.Always,
                KeepAlivePingDelay = TimeSpan.FromSeconds(30),
                KeepAlivePingTimeout = TimeSpan.FromSeconds(5),
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                EnableMultipleHttp2Connections = true
            };
                
            invoker = new HttpMessageInvoker(handler);
            _invokers.TryAdd(baseUrl, invoker);
        }
        
        return invoker!;
    }
}