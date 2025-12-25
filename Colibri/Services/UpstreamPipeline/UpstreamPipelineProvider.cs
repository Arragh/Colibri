using System.Runtime.CompilerServices;
using Colibri.Services.UpstreamPipeline.Interfaces;
using Colibri.Services.UpstreamPipeline.Enums;

namespace Colibri.Services.UpstreamPipeline;

public class UpstreamPipelineProvider : IUpstreamPipelineProvider
{
    private readonly IUpstreamPipeline _http;

    public UpstreamPipelineProvider(IUpstreamPipeline http)
    {
        _http = http;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IUpstreamPipeline GetPipeline(Protocol protocol)
    {
        return protocol switch
        {
            Protocol.Http => _http,
            _ => throw new ArgumentOutOfRangeException(nameof(protocol))
        };
    }
}