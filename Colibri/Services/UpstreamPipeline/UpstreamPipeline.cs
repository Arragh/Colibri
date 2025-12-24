using Colibri.Services.UpstreamPipeline.Interfaces;
using Colibri.Services.UpstreamPipeline.Models.Abstract;

namespace Colibri.Services.UpstreamPipeline;

public sealed class UpstreamPipeline : IUpstreamPipeline
{
    public Task<UpstreamResponse> ExecuteAsync()
    {
        throw new NotImplementedException();
    }
}