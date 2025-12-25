using Colibri.Services.UpstreamPipeline.Enums;

namespace Colibri.Services.UpstreamPipeline.Interfaces;

public interface IUpstreamPipelineProvider
{
    IUpstreamPipeline GetPipeline(Protocol protocol);
}