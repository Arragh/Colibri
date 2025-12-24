using Colibri.Services.UpstreamPipeline.Models.Abstract;

namespace Colibri.Services.UpstreamPipeline.Interfaces;

public interface IUpstreamPipeline
{
    Task<UpstreamResponse> ExecuteAsync();
}