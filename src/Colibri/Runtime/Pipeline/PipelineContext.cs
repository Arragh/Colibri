using System.Net;
using System.Runtime.CompilerServices;
using Colibri.Runtime.Snapshots;
using Microsoft.AspNetCore.Http;

namespace Colibri.Runtime.Pipeline;

public sealed class PipelineContext(GlobalSnapshot snapshot, HttpContext httpContext)
{
    private bool _isCommited = false;
    
    public readonly GlobalSnapshot GlobalSnapshot = snapshot;
    public readonly HttpContext HttpContext = httpContext;
    public readonly CancellationToken CancellationToken = httpContext.RequestAborted;
    
    public bool IsHandled { get; private set; }
    public int ClusterId { get; private set; }
    public string DownstreamPath { get; private set; } = string.Empty;
    public int HostIdx { get; private set; }
    public int StatusCode { get; private set; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MarkAsHandled()
    {
        IsHandled = true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetClusterId(int clusterId)
    {
        ClusterId = clusterId;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetDownstreamPath(string pathUrl)
    {
        DownstreamPath = pathUrl;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetHostIdx(int idx)
    {
        HostIdx = idx;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetStatusCode(HttpStatusCode statusCode)
    {
        StatusCode = (int)statusCode;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CommitStatusCode()
    {
        if (_isCommited)
        {
            return;
        }
        
        HttpContext.Response.StatusCode = StatusCode;
        _isCommited = true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsIdempotent()
    {
        var method = HttpContext.Request.Method;
        
        return method is "GET"
            or "PUT"
            or "DELETE"
            or "HEAD"
            or "OPTIONS"
            or "TRACE";
    }
}